using UnityEngine;

public class CoralBoundary : MonoBehaviour
{
    [SerializeField] private float pushStrength = 50f;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 pushDirection = other.transform.position - transform.position;
            if (pushDirection == Vector3.zero)
                pushDirection = Vector3.up;
            pushDirection.Normalize();
            other.transform.position += pushDirection * pushStrength * Time.deltaTime;
        }
    }
}