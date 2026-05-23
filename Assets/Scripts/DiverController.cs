using UnityEngine;

public class DiverController : MonoBehaviour
{
    public float swimSpeed = 5f;
    public float verticalSpeed = 3f;
    public ThirdPersonCamera cam;

    [Header("Boundaries")]
    public float minX = 270f;
    public float maxX = 660f;
    public float minY = -9f;
    public float maxY = 0f;
    public float minZ = 370f;
    public float maxZ = 580f;

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

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
        transform.position = pos;
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