using UnityEngine;

public class GoldenPearl : MonoBehaviour
{
    [Header("Sound")]
    public AudioClip collectSound;
    public float volume = 1f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Play golden pearl collect sound
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position, volume);
            }

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