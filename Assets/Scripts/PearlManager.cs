using UnityEngine;

public class PearlManager : MonoBehaviour
{
    [Header("Settings")]
    public float rotationSpeed = 50f;
    public AudioClip collectSound;
    public float floatAmplitude = 0.1f; // How high it jumps
    public float floatFrequency = 1f;   // How fast it bobs

    private Vector3 startPosition;

    void Start()
    {
        // Record the starting position so it bobs relative to where you placed it
        startPosition = transform.position;
    }
    void Update()
    {
        // Simple visual rotation
        transform.Rotate(new Vector3(0, 0, 1), rotationSpeed * Time.deltaTime);

        // 2. Handle Bobbing (Up and Down)
        // Math.Sin creates a wave that goes between -1 and 1
        float newY = startPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    private void OnTriggerEnter(Collider other)
{
        // This tells us exactly what part of the diver touched the pearl
        Debug.Log("CONTACT: Pearl touched by " + other.name);

        // Look for the "Player" tag on the object itself OR any of its parents
        bool isPlayer = other.CompareTag("Player") ||
                        other.transform.root.CompareTag("Player") ||
                        other.GetComponentInParent<UnderwaterSwimController>() != null;

        if (isPlayer)
        {
            Debug.Log("PLAYER CONFIRMED: Collecting...");
            Collect();
        }
        else
        {
            Debug.Log("REJECTED: The object " + other.name + " is not tagged Player.");
        }
    }

    private void Collect()
    {
        // 1. Try to find the UIManager if the static Instance is null
        if (UIManager.Instance == null)
        {
            UIManager.Instance = Object.FindFirstObjectByType<UIManager>();
        }

        // 2. Double check if we found it
        if (UIManager.Instance != null)
        {
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }

            UIManager.Instance.AddPearl();

            // 3. Destroy the pearl
            Destroy(gameObject);
        }
        else
        {
            // This will tell us if the UI scene isn't actually loaded yet
            Debug.LogError("FAIL: Pearl collected but UIManager not found in ANY loaded scene. Is 'UI Scene' visible in the Hierarchy?");
        }
    }
}
