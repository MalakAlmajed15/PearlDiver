using UnityEngine;

public class DiverAnimationController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Check if player is moving
        bool isMoving = rb.linearVelocity.magnitude > 0.1f;

        // Switch between Idle and Swim
        animator.SetBool("isSwimming", isMoving);
    }

    // Call this when diver gets hit
    public void TriggerHit()
    {
        animator.SetTrigger("isHit");
    }
}