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
            // Underwater swim animation
            animator.SetBool("isSwimming", isMoving);
        }
        else
        {
            // On land walk animation plays automatically
            // since Walk is the default state
            animator.SetBool("isSwimming", false);
        }
    }

    // Called when diver enters water
    public void EnterWater()
    {
        isUnderwater = true;
        animator.SetBool("isSwimming", true);
    }

    // Called when diver exits water
    public void ExitWater()
    {
        isUnderwater = false;
        animator.SetBool("isSwimming", false);
    }

    // Called when diver gets hit by enemy
    public void TriggerHit()
    {
        animator.SetTrigger("isHit");
    }
}