using UnityEngine;
using System.Collections.Generic;

public static class BridgeFitness
{
    // ==========================================================
    // FITNESS CALCULATION
    // ==========================================================
    public static float Evaluate(
        Car[] cars,
        BridgeGene[] genes,
        Goal goal,
        Checkpoint checkpoint,
        SkyCollider sc,
        VoidCollider vc,
        BarCreator bc)
    {
        if (bc == null) return 0f;

        // Grab fitness handler instance
        var f = FitnessHandler.instance;
        if (f == null)
        {
            Debug.LogError("FitnessHandler.instance is NULL! Add FitnessHandler to scene.");
            return 0f;
        }

        // ==========================================================
        // WORLD INFO
        // ==========================================================
        float leftX = bc.leftBound.position.x;
        float rightX = bc.rightBound.position.x;
        float spanWidth = Mathf.Abs(rightX - leftX);

        // ==========================================================
        // 1. CAR PROGRESS
        // ==========================================================
        float forwardProgress = 0f;
        bool carFellEarly = false;

        if (cars != null)
        {
            foreach (var c in cars)
            {
                if (c == null) continue;

                float dist = c.transform.position.x - leftX;
                forwardProgress = Mathf.Max(forwardProgress, dist);

                if (c.transform.position.y < -1f && dist < spanWidth * 0.2f)
                    carFellEarly = true;
            }
        }

        float progress01 = spanWidth > 0.5f
            ? Mathf.Clamp01(forwardProgress / spanWidth)
            : 0f;

        // ==========================================================
        // 2. STRUCTURE ANALYSIS
        // ==========================================================
        RoadBar[] roads = Object.FindObjectsOfType<RoadBar>();
        BeamBar[] beams = Object.FindObjectsOfType<BeamBar>();

        int totalBars = 0;
        int connectedBars = 0;
        int properNodeConnections = 0;

        float sagTotal = 0f;
        float oobBars = 0f;
        float shortBarPenalty = 0f;

        const float sagLimit = 0.6f;

        HashSet<Point> uniqueAnchoredNodes = new HashSet<Point>();

        void ScoreBar(Point a, Point b, DistanceJoint2D ja, DistanceJoint2D jb)
        {
            if (a == null || b == null) return;

            totalBars++;

            bool connected = (ja != null && jb != null);
            if (connected)
            {
                connectedBars++;

                bool properA = !a.isFloating;
                bool properB = !b.isFloating;

                if (properA && properB)
                    properNodeConnections++;
            }

            if (a.isAnchored) uniqueAnchoredNodes.Add(a);
            if (b.isAnchored) uniqueAnchoredNodes.Add(b);

            Vector2 mid = (a.rb.position + b.rb.position) * 0.5f;
            if (mid.y < -3f)
                sagTotal += Mathf.Abs(mid.y + 3f);

            if (mid.x < leftX - 1.5f || mid.x > rightX + 1.5f)
                oobBars++;

            float barLength = Vector2.Distance(a.rb.position, b.rb.position);
            if (barLength < f.MinBarLength)
                shortBarPenalty += (f.MinBarLength - barLength);
        }

        foreach (var r in roads) ScoreBar(r.nodeA, r.nodeB, r.jointA, r.jointB);
        foreach (var b in beams) ScoreBar(b.nodeA, b.nodeB, b.jointA, b.jointB);

        float stability = 0f;
        float properConnectionRatio = 0f;
        float anchorUse = 0f;
        float sagPenalty = 0f;

        if (totalBars > 0)
        {
            stability = Mathf.Clamp01(connectedBars / (float)totalBars);
            properConnectionRatio = Mathf.Clamp01(properNodeConnections / (float)totalBars);

            float anchorCount = uniqueAnchoredNodes.Count;
            anchorUse = Mathf.Clamp01(anchorCount / Mathf.Max(1f, f.TotalAnchors));

            sagPenalty = Mathf.Clamp01(sagTotal / (totalBars * sagLimit));
        }

        // ==========================================================
        // 3. CHECKPOINT / GOAL
        // ==========================================================
        float checkpointScore =
            (checkpoint != null && checkpoint.checkpointReached) ? 1f : 0f;

        float goalScore =
            (goal != null && goal.endReached) ? 1f : 0f;

        // ==========================================================
        // 4. FINAL FITNESS
        // ==========================================================
        float fitness = 0f;

        // Success rewards
        fitness += goalScore      * f.GoalReward;
        fitness += checkpointScore * f.CheckpointReward;
        fitness += progress01     * f.ProgressReward;

        // Structure rewards
        fitness += stability             * f.StabilityReward;
        fitness += properConnectionRatio * f.ProperConnectionReward;
        fitness += anchorUse             * f.AnchorUseReward;

        // Penalties
        if (carFellEarly) fitness += f.EarlyFallPenalty;
        if (stability < 0.35f && totalBars > 0)
            fitness += f.LowStabilityPenalty;

        fitness += sagPenalty * f.SagPenaltyMultiplier;
        fitness += (oobBars / Mathf.Max(1f, totalBars)) * f.OOBPenaltyMultiplier;
        fitness += shortBarPenalty * f.ShortBarPenaltyMultiplier;

        if (totalBars == 0)
            fitness += f.NoStructurePenalty;

        if (sc != null && sc.tooHigh)
            fitness += f.TooHighPenalty;

        if (vc != null && vc.tooLow)
            fitness += f.TooLowPenalty;

        return Mathf.Clamp(fitness, -999f, 999f);
    }
}
