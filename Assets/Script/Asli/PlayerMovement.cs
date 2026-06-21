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
    public AudioClip waterSplashSound; // Efek suara masuk air

    [Header("Audio Source BGM")]
    public AudioSource bgmAudioSource; // Khusus Musik Latar (BGM)
    public AudioClip bgmSound;         // File musik latar game

    [Header("Stealth Takedown Settings")]
    public float detectionRadius = 2f;       // Jarak kedekatan dengan musuh untuk bisa takedown
    public LayerMask enemyLayer;            // Pastikan layer musuh diatur ke layer khusus (misal: "Enemy")
    public GameObject takedownUiPrompt;     // Drag Game Object UI teks "Tekan E untuk Takedown" ke sini
    public float takedownDashSpeed = 8f;     // Kecepatan melangkah maju merapat secara dinamis

    // Private internal variables
    private CharacterController characterController;
    private Animator animator;
    private Vector3 moveDirection = Vector3.zero;
    private bool isCrouching = false;
    private bool isExecutingTakedown = false; // Pengunci gerakan saat eksekusi

    // Variabel baru untuk transisi melangkah maju dinamis (Mencegah teleportasi kaku)
    private bool isMovingToTakedownTarget = false;
    private Vector3 takedownTargetPosition;
    private Quaternion takedownTargetRotation;

    // AudioSource internal yang otomatis dibuat lewat kode
    private AudioSource internalSfxSource; 

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        internalSfxSource = gameObject.AddComponent<AudioSource>();
        internalSfxSource.playOnAwake = false;

        // Mengunci cursor mouse ke tengah layar
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Otomatis menyalakan BGM saat game dimulai
        if (bgmAudioSource != null && bgmSound != null)
        {
            bgmAudioSource.clip = bgmSound;
            bgmAudioSource.loop = true;
            bgmAudioSource.Play();
        }

        // Sembunyikan UI prompt di awal game
        if (takedownUiPrompt != null)
        {
            takedownUiPrompt.SetActive(false);
        }
    }

    void Update()
    {
        // 1. LOGIKA DINAMIS: Menggeser player maju secara halus ke posisi pas di belakang musuh
        if (isMovingToTakedownTarget)
        {
            transform.position = Vector3.MoveTowards(transform.position, takedownTargetPosition, takedownDashSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, takedownTargetRotation, takedownDashSpeed * 2f * Time.deltaTime);

            // Jika posisi sudah pas nempel dengan target di belakang musuh
            if (Vector3.Distance(transform.position, takedownTargetPosition) < 0.05f)
            {
                isMovingToTakedownTarget = false;
                ExecuteTakedownSequence(); // Baru nyalakan animasinya di sini
            }
            return; // Kunci input WASD selama merapat
        }

        // 2. PERBAIKAN LOGIKA PENGAMAN: MATIKAN GRAVITASI SKRIP AGAR GERAKAN LOMPAT ANIMASI KELUAR!
        if (isExecutingTakedown)
        {
            moveDirection = Vector3.zero;

            if (internalSfxSource.isPlaying)
            {
                internalSfxSource.Stop();
            }
            return; // Keluar dari Update
        }

        // 3. CEK GROUNDED & AMBIL INPUT PERGERAKAN WASD
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isMoving = (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0);
        bool isRunning = isMoving && Input.GetKey(KeyCode.LeftShift) && !isCrouching;
        
        float curSpeedX = (isCrouching ? crouchSpeed : (isRunning ? runSpeed : walkSpeed)) * Input.GetAxis("Vertical");
        float curSpeedY = (isCrouching ? crouchSpeed : (isRunning ? runSpeed : walkSpeed)) * Input.GetAxis("Horizontal");
        
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (characterController.isGrounded)
        {
            if (moveDirection.y < 0)
            {
                moveDirection.y = -2f; 
            }
        }

        // 4. LOGIKA LOMPAT MANUAL (JUMP)
        if (Input.GetButton("Jump") && characterController.isGrounded && !isCrouching)
        {
            moveDirection.y = Mathf.Sqrt(jumpPower * 2f * gravityForce);
            
            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }

            if (jumpSound != null)
            {
                AudioSource.PlayClipAtPoint(jumpSound, transform.position);
            }
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravityForce * Time.deltaTime;
        }

        // 5. LOGIKA JONGKOK (CROUCH)
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;
            characterController.height = isCrouching ? crouchHeight : defaultHeight;
        }

        // Eksekusi pergerakan fisik CharacterController
        characterController.Move(moveDirection * Time.deltaTime);

        // 6. LOGIKA ROTASI KAMERA TPS (HANYA KANAN-KIRI)
        if (tpsCamera != null)
        {
            float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
            transform.Rotate(0, mouseX, 0);
        }

        // 7. UPDATE PARAMETER ANIMATOR JALAN/LARI
        if (animator != null)
        {
            animator.SetBool("isGrounded", characterController.isGrounded);
            animator.SetBool("isWalking", isMoving && !isRunning && characterController.isGrounded);
            animator.SetBool("isRunning", isRunning && characterController.isGrounded);
        }

        // 8. LOGIKA AUDIO SFX JALAN/LARI
        if (internalSfxSource != null)
        {
            if (isMoving && characterController.isGrounded)
            {
                AudioClip targetClip = isRunning ? runSound : walkSound;

                if (targetClip != null)
                {
                    if (internalSfxSource.clip != targetClip || !internalSfxSource.isPlaying)
                    {
                        internalSfxSource.clip = targetClip;
                        internalSfxSource.loop = true;
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

        // 9. SISTEM CEK MUSUH (UNTUK EVENT TOMBOL E TAKEDOWN)
        CheckForStealthTakedown();
    }

    private void CheckForStealthTakedown()
    {
        if (isMovingToTakedownTarget || isExecutingTakedown) return;

        Collider[] closeEnemies = Physics.OverlapSphere(transform.position, detectionRadius, enemyLayer);

        if (closeEnemies.Length > 0)
        {
            if (takedownUiPrompt != null) takedownUiPrompt.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E) && characterController.isGrounded)
            {
                Transform enemyTransform = closeEnemies[0].transform;
                EnemyHealth enemy = closeEnemies[0].GetComponent<EnemyHealth>();
                
                if (enemy != null)
                {
                    takedownTargetPosition = enemyTransform.position - (enemyTransform.forward * 0.35f);
                    takedownTargetPosition.y = transform.position.y; 

                    takedownTargetRotation = Quaternion.LookRotation(enemyTransform.forward);

                    enemy.TakeStealthDeath();

                    isMovingToTakedownTarget = true;

                    if (takedownUiPrompt != null) takedownUiPrompt.SetActive(false);
                }
            }
        }
        else
        {
            if (takedownUiPrompt != null) takedownUiPrompt.SetActive(false);
        }
    }

    private void ExecuteTakedownSequence()
    {
        isExecutingTakedown = true;

        if (takedownUiPrompt != null) takedownUiPrompt.SetActive(false);

        // Matikan fisik CharacterController sementara agar karakter bisa melompat bebas tanpa kaku
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetTrigger("ExecuteTakedown"); 
        }
    }

    // SATU FUNGSI UTAMA: Dipanggil dari Animation Event saat gerakan selesai
    public void OnTakedownAnimationComplete()
    {
        isExecutingTakedown = false;

        // Hidupkan kembali fisik CharacterController agar bisa dikontrol normal
        if (characterController != null)
        {
            characterController.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water") || other.name.Contains("Water"))
        {
            if (waterSplashSound != null)
            {
                AudioSource.PlayClipAtPoint(waterSplashSound, transform.position);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}