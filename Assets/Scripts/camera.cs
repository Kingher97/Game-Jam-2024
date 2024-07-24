using UnityEngine;

public class VengeCameraController : MonoBehaviour
{
    public Transform player; // Riferimento al personaggio
    public float distance = 10f; // Distanza tra la telecamera e il personaggio
    public float height = 5f; // Altezza della telecamera rispetto al personaggio
    public float rotationSpeed = 5f; // Velocità di rotazione della telecamera
    public float verticalAngleLimit = 80f; // Limite dell'angolo verticale
    private float xRotation = 0f;

    private void Start()
    {
        // Nascondi il cursore e bloccalo al centro dello schermo
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Input del mouse per la rotazione della telecamera
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

        // Calcolare la rotazione verticale e applicare il clamp
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalAngleLimit, verticalAngleLimit);

        // Ruota il personaggio insieme alla telecamera attorno all'asse Y
        player.Rotate(Vector3.up * mouseX);

        // Calcola la nuova posizione della telecamera
        Vector3 direction = new Vector3(0, height, -distance);
        Quaternion rotation = Quaternion.Euler(xRotation, player.eulerAngles.y, 0);
        transform.position = player.position + rotation * direction;

        // La telecamera guarda sempre il personaggio
        transform.LookAt(player.position + Vector3.up * height);
    }
}
