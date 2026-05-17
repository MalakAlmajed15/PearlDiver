using UnityEngine;

public class GoldenPearl : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Find AirMeterController and add a life
            AirMeterController air = other.GetComponent<AirMeterController>();
            if (air == null)
                air = other.GetComponentInParent<AirMeterController>();

            if (air != null)
            {
                air.lives++;
                Debug.Log("Extra life! Lives: " + air.lives);

                // Tell UIManager to update hearts
                if (UIManager.Instance != null)
                    UIManager.Instance.AddLife();
            }

            Destroy(gameObject); // Remove golden pearl after collecting
        }
    }
}