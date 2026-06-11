using UnityEngine;

public class MovementScript : MonoBehaviour
{
    public float speed = 5f;
    public Transform cam; // Slot untuk Main Camera

    void Update()
    {
        float h = Input.GetAxis("Horizontal"); // A atau D
        float v = Input.GetAxis("Vertical");   // W atau S

        Vector3 direction = new Vector3(h, 0, v).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Menghitung sudut arah kamera
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            
            // Mengubah arah gerak sesuai arah kamera
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            
            // Gerakkan karakter
            transform.Translate(moveDir.normalized * speed * Time.deltaTime, Space.World);
            
            // Karakter otomatis hadap ke arah jalan
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }
    }
}