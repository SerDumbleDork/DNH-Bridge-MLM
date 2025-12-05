using UnityEngine;

public class FitnessHandler : MonoBehaviour
{
    
    public float GoalReward = 40f;
    public float CheckpointReward = 10f;
    public float ProgressReward = 30f;

    public float StabilityReward = 12f;
    public float ProperConnectionReward = 20f;
    public float AnchorUseReward = 18f;

    public float EarlyFallPenalty = -10f;
    public float LowStabilityPenalty = -20f;
    public float SagPenaltyMultiplier = -15f;
    public float OOBPenaltyMultiplier = -6f;
    public float ShortBarPenaltyMultiplier = -8f;
    public float TooLowPenalty = -30f;
    public float TooHighPenalty = -50f;

    public float NoStructurePenalty = -50f;

    public int TotalAnchors = 4;
    public float MinBarLength = 1f;

    public static FitnessHandler instance;
    void Awake() { instance = this; }
}
