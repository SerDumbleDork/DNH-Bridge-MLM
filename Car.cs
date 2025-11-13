using UnityEngine;

public class Car : MonoBehaviour
{
    public float rollForce = 10f;
    public float maxSpeed = 3f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (rb.angularVelocity > -maxSpeed * 50f)
            rb.AddTorque(-rollForce, ForceMode2D.Force);
        if (rb.linearVelocity.magnitude < maxSpeed)
            rb.AddForce(Vector2.right * rollForce * 0.2f, ForceMode2D.Force);
    }
}
