using UnityEngine;

public class SkyCollider: MonoBehaviour
{
    public bool tooHigh = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Car"))
        {
            tooHigh = true;
            Debug.Log("Car crossed bridge successfully!");
        }
    }
}

