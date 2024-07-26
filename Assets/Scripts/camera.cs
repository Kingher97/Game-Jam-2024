using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float distance = 10f;
    public float height = 5f;
    public float rotationSpeed = 5f;
    public float verticalAngleLimit = 80f;
    private float xRotation = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalAngleLimit, verticalAngleLimit);

        player.Rotate(Vector3.up * mouseX);
        player.rotation = Quaternion.Euler(0, player.eulerAngles.y, 0);

        Vector3 direction = new Vector3(0, height, -distance);
        Quaternion rotation = Quaternion.Euler(xRotation, player.eulerAngles.y, 0);
        transform.position = player.position + rotation * direction;

        transform.LookAt(player.position + Vector3.up * height);
    }
}
