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

    [HideInInspector] public float yaw;
    [HideInInspector] public float pitch;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void LateUpdate()
    {
        if (!target) return;

        yaw += Input.GetAxis("Mouse X") * sensitivityX;
        pitch -= Input.GetAxis("Mouse Y") * sensitivityY;
        pitch = Mathf.Clamp(pitch, minY, maxY);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rotation * new Vector3(0, height, -distance);
        transform.position = target.position + offset;
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}