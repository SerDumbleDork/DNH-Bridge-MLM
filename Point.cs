using UnityEngine;

public class Point : MonoBehaviour
{
    public bool isAnchored = false;
    public bool destructable = true;

    [HideInInspector] public Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Nodes should NEVER move unless the joint breaks
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
        rb.freezeRotation = true;

        // Anchored nodes are just kinematic too, but they never move
        if (isAnchored)
        {
            rb.bodyType = RigidbodyType2D.Static;
        }
    }
}
