using UnityEngine;

public class GoldenPearl : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Give extra life through UIManager
            if (UIManager.Instance != null)
            {
                UIManager.Instance.AddLife();
                Debug.Log("Extra life collected!");
            }
            else
            {
                Debug.Log("UIManager not found!");
            }

            // Remove golden pearl after collecting
            Destroy(gameObject);
        }
    }
}