    using UnityEngine;
    public class JumpScript : MonoBehaviour
    {
    public float jumpForce = 5f; // Kekuatan lompatan
    private Rigidbody rb; // Referensi ke Rigidbody bola
    void Start()
    {
    // Mengambil komponen Rigidbody
    rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        // Melompat saat tombol spasi ditekan
    if (Input.GetKeyDown(KeyCode.G))
    {
    // Menambahkan gaya ke atas (lompatan)
    rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    }
    }   