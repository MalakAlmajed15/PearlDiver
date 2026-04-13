using UnityEngine;
public class DiverController : MonoBehaviour
{
    public float swimSpeed = 5f;
    public float verticalSpeed = 3f;
    public ThirdPersonCamera cam;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = 3f;
        rb.angularDamping = 0.05f;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (cam != null)
            transform.rotation = Quaternion.Euler(0, cam.yaw, 0);
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = (transform.right * h +
                        transform.forward * v) * swimSpeed;

        if (Input.GetKey(KeyCode.Space))
            move.y = verticalSpeed;
        else if (Input.GetKey(KeyCode.LeftShift))
            move.y = -verticalSpeed;

        rb.linearVelocity = move;
    }
}