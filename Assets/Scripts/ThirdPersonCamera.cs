using UnityEngine;
public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 6f;
    public float height = 2f;
    public float sensitivityX = 3f;
    public float sensitivityY = 2f;
    public float minY = -60f;
    public float maxY = 75f;

    [Header("Rotation Settings")]
    public bool holdToRotate = true;
    public int mouseButton = 1; // 1 = Right Click, 0 = Left Click, 2 = Middle Click

    [HideInInspector] public float yaw;
    [HideInInspector] public float pitch;

    void Start()
    {
        if (!holdToRotate)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void LateUpdate()
    {
        if (!target) return;

        bool isRotating = !holdToRotate || Input.GetMouseButton(mouseButton);

        if (isRotating)
        {
            // Handle rotation
            yaw += Input.GetAxis("Mouse X") * sensitivityX;
            pitch -= Input.GetAxis("Mouse Y") * sensitivityY;
            pitch = Mathf.Clamp(pitch, minY, maxY);

            // Optional: Hide/Lock cursor while rotating
            if (holdToRotate)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        else if (holdToRotate)
        {
            // Unlock cursor when not rotating
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rotation * new Vector3(0, height, -distance);
        transform.position = target.position + offset;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}