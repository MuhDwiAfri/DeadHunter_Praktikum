using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;        // Drag objek Player ke sini
    public float mouseSensitivity = 3f;
    public float distance = 5f;     // Jarak kamera dari player
    
    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        // Biar mouse nggak lari-lari keluar layar
        Cursor.lockState = CursorLockMode.Locked;
        
        // Ambil rotasi awal biar gak nge-snap pas mulai
        Vector3 angles = transform.eulerAngles;
        rotationY = angles.y;
        rotationX = angles.x;
    }

    void LateUpdate()
    {
        if (!target) return;

        // Input Mouse / Touchpad
        rotationY += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotationX -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Batasi rotasi vertikal biar gak kebalik (kayak GTA)
        rotationX = Mathf.Clamp(rotationX, -10f, 70f);

        // Kalkulasi Rotasi dan Posisi
        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0);
        
        // Rumus: Posisi Target + (Arah Mundur Kamera * Jarak)
        Vector3 position = target.position - (rotation * Vector3.forward * distance);

        // Terapkan ke Kamera
        transform.rotation = rotation;
        transform.position = position;
    }
}