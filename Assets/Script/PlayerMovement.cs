using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Camera TPS")]
    public Transform tpsCamera;

    [Header("Movement Speeds")]
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float crouchSpeed = 3f;

    [Header("Physics & Jump")]
    public float jumpPower = 2f;
    public float gravityForce = 25f;

    [Header("Mouse Look (Horizontal Only)")]
    public float lookSpeed = 2f;

    [Header("Height Settings")]
    public float defaultHeight = 2f;
    public float crouchHeight = 1f;

    [Header("Audio Clips SFX")]
    public AudioClip walkSound;
    public AudioClip runSound;  // Masukkan audio lari 9 detikmu di sini
    public AudioClip jumpSound;
    public AudioClip waterSplashSound; // KOLOM BARU: Efek suara masuk air

    [Header("Audio Source BGM")]
    public AudioSource bgmAudioSource; // Khusus Musik Latar (BGM)
    public AudioClip bgmSound;         // File musik latar game

    // Private internal variables
    private CharacterController characterController;
    private Animator animator;
    private Vector3 moveDirection = Vector3.zero;
    private bool isCrouching = false;

    // AudioSource internal yang otomatis dibuat lewat kode (tidak perlu drag di Inspector)
    private AudioSource internalSfxSource; 

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Membuat AudioSource khusus SFX secara otomatis via code saat game mulai
        internalSfxSource = gameObject.AddComponent<AudioSource>();
        internalSfxSource.playOnAwake = false;

        // Mengunci cursor mouse ke tengah layar
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Otomatis menyalakan BGM saat game dimulai
        if (bgmAudioSource != null && bgmSound != null)
        {
            bgmAudioSource.clip = bgmSound;
            bgmAudioSource.loop = true; // Musik otomatis mengulang
            bgmAudioSource.Play();
        }
    }

    void Update()
    {
        // 1. CEK GROUNDED & AMBIL INPUT PERGERAKAN
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Deteksi apakah tombol WASD / Arrow sedang ditekan
        bool isMoving = (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0);
        // Berlari hanya aktif jika sedang bergerak maju/samping DAN menekan Shift
        bool isRunning = isMoving && Input.GetKey(KeyCode.LeftShift) && !isCrouching;
        
        // Atur kecepatan berdasarkan kondisi (Crouch vs Run vs Walk)
        float curSpeedX = (isCrouching ? crouchSpeed : (isRunning ? runSpeed : walkSpeed)) * Input.GetAxis("Vertical");
        float curSpeedY = (isCrouching ? crouchSpeed : (isRunning ? runSpeed : walkSpeed)) * Input.GetAxis("Horizontal");
        
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // PENGAMAN GROUNDED: Mengunci nilai gravitasi agar tidak amblas menembus tanah
        if (characterController.isGrounded)
        {
            if (moveDirection.y < 0)
            {
                moveDirection.y = -2f; 
            }
        }

        // 2. LOGIKA LOMPAT (JUMP)
        if (Input.GetButton("Jump") && characterController.isGrounded && !isCrouching)
        {
            moveDirection.y = Mathf.Sqrt(jumpPower * 2f * gravityForce);
            
            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }

            // Memutar suara lompat secara instan di posisi koordinat player
            if (jumpSound != null)
            {
                AudioSource.PlayClipAtPoint(jumpSound, transform.position);
            }
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        // Aplikasikan Gravitasi normal jika sedang melayang di udara
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravityForce * Time.deltaTime;
        }

        // 3. LOGIKA JONGKOK (CROUCH)
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;
            characterController.height = isCrouching ? crouchHeight : defaultHeight;
        }

        // Eksekusi pergerakan fisik karakter
        characterController.Move(moveDirection * Time.deltaTime);

        // 4. LOGIKA ROTASI KAMERA TPS (HANYA KANAN-KIRI)
        if (tpsCamera != null)
        {
            float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
            transform.Rotate(0, mouseX, 0);
        }

        // 5. UPDATE PARAMETER ANIMATOR
        if (animator != null)
        {
            animator.SetBool("isGrounded", characterController.isGrounded);
            animator.SetBool("isWalking", isMoving && !isRunning && characterController.isGrounded);
            animator.SetBool("isRunning", isRunning && characterController.isGrounded);
        }

        // 6. LOGIKA AUDIO SFX JALAN/LARI (KONTROL PENUH VIA CODE)
        if (internalSfxSource != null)
        {
            if (isMoving && characterController.isGrounded)
            {
                // Tentukan klip audio berdasarkan kondisi input
                AudioClip targetClip = isRunning ? runSound : walkSound;

                if (targetClip != null)
                {
                    // Jika klip berubah (misal dari jalan ke lari) ATAU audio sedang mati, mainkan klip baru
                    if (internalSfxSource.clip != targetClip || !internalSfxSource.isPlaying)
                    {
                        internalSfxSource.clip = targetClip;
                        internalSfxSource.loop = true; // looping aman untuk durasi panjang
                        internalSfxSource.Play();
                    }
                }
            }
            else
            {
                if (internalSfxSource.isPlaying)
                {
                    internalSfxSource.Stop();
                }
            }
        }
    }

    // LOGIKA BARU: Memicu suara saat menyentuh air (Trigger)
    private void OnTriggerEnter(Collider other)
    {
        // Cek apakah objek yang ditabrak adalah air (punya tag "Water" atau nama mengandung "Water")
        if (other.CompareTag("Water") || other.name.Contains("Water"))
        {
            if (waterSplashSound != null)
            {
                // Mainkan suara splash sekali di posisi player
                AudioSource.PlayClipAtPoint(waterSplashSound, transform.position);
            }
        }
    }
}