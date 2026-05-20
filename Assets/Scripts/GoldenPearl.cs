using UnityEngine;

public class GoldenPearl : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Tell the UIManager directly to add a life and update the hearts
            if (UIManager.Instance != null)
            {
                UIManager.Instance.AddLife();
            }

            Destroy(gameObject); // Remove golden pearl after collecting
        }
    }
}