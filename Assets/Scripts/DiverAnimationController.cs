using UnityEngine;

public class DiverAnimationController : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    private bool isUnderwater = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Check if player is moving
        bool isMoving = rb.linearVelocity.magnitude > 0.1f;

        if (isUnderwater)
        {
            // Underwater — use swim animation
            animator.SetBool("isSwimming", isMoving);
            animator.SetBool("isWalking", false);
        }
        else
        {
            // On land — use walk animation
            animator.SetBool("isWalking", isMoving);
            animator.SetBool("isSwimming", false);
        }
    }

    // Called when entering water
    public void EnterWater()
    {
        isUnderwater = true;
        animator.SetBool("isSwimming", true);
        animator.SetBool("isWalking", false);
    }

    // Called when exiting water
    public void ExitWater()
    {
        isUnderwater = false;
        animator.SetBool("isSwimming", false);
        animator.SetBool("isWalking", true);
    }

    // Called when diver gets hit
    public void TriggerHit()
    {
        animator.SetTrigger("isHit");
    }
}