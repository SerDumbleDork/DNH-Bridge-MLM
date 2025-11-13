using UnityEngine;
using System.Collections;

public class Sim : MonoBehaviour
{
    public bool simulationRunning = false;
    public GameObject carPrefab;
    private GameObject carInstance;
    private Vector3 carStartPos = new Vector3(-20f, 5f, 0f);

    void Start()
    {
        SpawnCar();
        SetSimulationState(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            simulationRunning = !simulationRunning;
            SetSimulationState(simulationRunning);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StopAllCoroutines();
            ResetEverything();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();
        }
    }

    // ----------------------------------------------------------------------
    // UPDATED PHYSICS CONTROL (car only, no nodes/bars touched)
    // ----------------------------------------------------------------------
    void SetSimulationState(bool runPhysics)
    {
        Rigidbody2D[] allBodies = FindObjectsOfType<Rigidbody2D>();

        foreach (Rigidbody2D rb in allBodies)
        {
            if (rb == null) continue;

            // Ignore terrain
            if (rb.CompareTag("Terrain"))
                continue;

            // Ignore nodes
            if (rb.GetComponent<Point>() != null)
                continue;

            // Ignore roads
            if (rb.GetComponent<Road>() != null)
                continue;

            // Ignore beams => BEAMS MUST SIMULATE!
            if (rb.GetComponent<Beam>() != null)
                continue;

            // The ONLY thing left is the CAR
            if (!runPhysics)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0;
                rb.gravityScale = 0;
                rb.isKinematic = true;
                rb.simulated = false;
            }
            else
            {
                rb.isKinematic = false;
                rb.simulated = true;
                rb.gravityScale = 1f;
            }
        }

        Debug.Log(runPhysics ? "Simulation started" : "Simulation stopped");
    }

    // ----------------------------------------------------------------------
    // RESET BRIDGE + CAR
    // ----------------------------------------------------------------------
    public void ResetEverything()
    {
        StopAllCoroutines();

        // Destroy RoadBars and BeamBars
        foreach (var r in FindObjectsOfType<Road>())
            if (r != null) Destroy(r.gameObject);

        // Destroy nodes (except anchored)
        foreach (Point node in FindObjectsOfType<Point>())
        {
            if (node != null && node.destructable)
                Destroy(node.gameObject);
        }

        // Reset car
        if (carInstance != null)
            Destroy(carInstance);

        SpawnCar();

        simulationRunning = false;
        SetSimulationState(false);

        Debug.Log("Bridge and car reset.");
    }

    private void SpawnCar()
    {
        if (carPrefab != null)
            carInstance = Instantiate(carPrefab, carStartPos, Quaternion.identity);
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 40, 400, 30),
            simulationRunning
                ? "Mode: SIMULATION (Press Space to Stop, R to Reset, Q to Quit)"
                : "Mode: BUILD (Press Space to Simulate, R to Reset, Q to Quit)");
    }
}
