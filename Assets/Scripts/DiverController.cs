using UnityEngine;

public class DiverController : MonoBehaviour
{
    public float swimSpeed = 5f;
    public float verticalSpeed = 3f;
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;

    private Rigidbody rb;
    private float yaw;
    private float pitch;

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
       
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -75f, 75f);

        transform.localRotation = Quaternion.Euler(0, yaw, 0);

        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(pitch, 0, 0);
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