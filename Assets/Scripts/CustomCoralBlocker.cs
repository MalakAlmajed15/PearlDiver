using UnityEngine;

public class CustomCoralBlocker : MonoBehaviour
{
    [Header("Settings")]
    public float blockRadius = 3.5f;

    private Transform playerTransform;
    private UnderwaterSwimController playerController;

    private void Start()
    {
        playerController = Object.FindAnyObjectByType<UnderwaterSwimController>();
        if (playerController != null)
            playerTransform = playerController.transform;
    }

    private void LateUpdate()
    {
        if (playerTransform == null || playerController == null)
        {
            Debug.LogError($"{gameObject.name}: missing player reference!");
            return;
        }

        Vector3 pushDir = playerTransform.position - transform.position;
        float currentDistance = pushDir.magnitude;

        Debug.Log($"{gameObject.name}: player distance = {currentDistance:F2}, blockRadius = {blockRadius}");

        if (currentDistance < blockRadius)
        {
            if (currentDistance < 0.001f)
                pushDir = Vector3.up;

            float penetration = blockRadius - currentDistance;
            Vector3 pushVector = pushDir.normalized * (penetration + 0.05f);

            Debug.Log($"{gameObject.name}: PUSHING player by {pushVector}");
            playerController.PushOut(pushVector);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, blockRadius);
    }

}