using UnityEngine;

public class DiverController : MonoBehaviour
{
    public float swimSpeed = 5f;
    public float verticalSpeed = 3f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = 3f;          // replaces linearDamping
        rb.angularDamping = 0.05f;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(h, 0, v).normalized;

        if (inputDir.magnitude > 0.1f)
        {
            // Move relative to diver�s facing direction
            Vector3 move = transform.TransformDirection(inputDir) * swimSpeed;

            if (Input.GetKey(KeyCode.Space)) move.y = verticalSpeed;
            else if (Input.GetKey(KeyCode.LeftShift)) move.y = -verticalSpeed;

            rb.linearVelocity = move;

            // Smoothly rotate diver toward movement direction
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                Quaternion.LookRotation(new Vector3(move.x, 0, move.z)),
                Time.deltaTime * 5f
            );
        }
        else
        {
            rb.linearVelocity = Vector3.zero;
        }
    }
}
