using UnityEngine;

public class SkyCollider: MonoBehaviour
{
    public bool tooHigh = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Car"))
        {
            tooHigh = true;
            Debug.Log("Too High Fitness set to 0");
        }
    }
}

