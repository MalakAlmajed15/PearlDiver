using UnityEngine;

public class CustomCoralBlocker : MonoBehaviour
{
    [Header("Settings")]
    public float blockRadius = 3.5f;

    private Transform playerTransform;
    private UnderwaterSwimController playerController;

    private void Start()
    {
        // Find the player automatically in the scene
        playerController = Object.FindAnyObjectByType<UnderwaterSwimController>();
        if (playerController != null)
        {
            playerTransform = playerController.transform;
        }
    }

    private void Update()
    {
        if (playerTransform == null) return;

        // Calculate horizontal distance between the diver and this specific coral
        Vector3 playerPos = playerTransform.position;
        Vector3 coralPos = transform.position;

        Vector3 pushDir = new Vector3(playerPos.x - coralPos.x, 0f, playerPos.z - coralPos.z);
        float currentDistance = pushDir.magnitude;

        // If the player penetrates our radius, calculate the step-back and push them out
        if (currentDistance < blockRadius)
        {
            float penetration = blockRadius - currentDistance;
            playerTransform.position += pushDir.normalized * (penetration + 0.05f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, blockRadius);
    }
}