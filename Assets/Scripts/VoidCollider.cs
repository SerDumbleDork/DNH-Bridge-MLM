using UnityEngine;

public class VoidCollider : MonoBehaviour
{
    public bool tooLow;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Car"))
        {
            tooLow = true;
            Debug.Log("-30 Fitness");
        }
    }
}
