using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement; // TAMBAHAN: Wajib untuk merestart level saat kalah

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMove : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float patrolRadius = 10f;     
    public float minimumWaitTime = 2f;   
    public float maximumWaitTime = 5f;   
    public float patrolSpeed = 3.5f;     

    [Header("Chase Settings")]
    public float chaseSpeed = 6f;        
    public float keepDistance = 1.2f;    // KUNCI: Jarak aman diperkecil sedikit agar animasi kejar terasa pas sebelum menangkap

    [Header("Catch System")]
    public float catchDistance = 1.5f;   // Jarak minimal musuh dianggap berhasil "menangkap" player

    private NavMeshAgent agent;
    private Animator anim;
    private float waitTimer;
    private float currentWaitTime;
    private bool isWaiting = false;

    private bool isPlayerInBoxCollider = false;
    private Transform playerTransform;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        agent.speed = patrolSpeed;

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }
        
        MoveToRandomTarget();
    }

    void Update()
    {
        if (agent == null || !agent.isOnNavMesh || !agent.isActiveAndEnabled) return;

        if (anim != null && (anim.GetBool("Kill") == true || anim.GetCurrentAnimatorStateInfo(0).IsName("Stealth_Death") || anim.GetCurrentAnimatorStateInfo(0).IsName("Die")))
        {
            agent.isStopped = true;
            return;
        }

        // LOGIKA MENGEJAR & MENANGKAP PLAYER
        if (isPlayerInBoxCollider && playerTransform != null)
        {
            agent.speed = chaseSpeed;              
            agent.SetDestination(playerTransform.position); 
            
            agent.stoppingDistance = keepDistance; 
            agent.isStopped = false;

            // --- TAMBAHAN KUNCI: HITUNG JARAK REALTIME UNTUK MENANGKAP PLAYER ---
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer <= catchDistance)
            {
                Debug.Log("Player tertangkap oleh musuh! Mengulang level...");
                
                // Ambil nama scene aktif dan reload dari awal (Sistem Lose)
                string currentSceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(currentSceneName);
                return;
            }
            // --------------------------------------------------------------------

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                if (anim != null) anim.SetBool("isWalking", false); 
            }
            else
            {
                if (anim != null) anim.SetBool("isWalking", true);  
            }
            
            return; 
        }

        // KEMBALI PATROLI
        agent.speed = patrolSpeed;
        agent.stoppingDistance = 0.2f; 

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                waitTimer = 0f;
                currentWaitTime = Random.Range(minimumWaitTime, maximumWaitTime);
            }
        }

        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (anim != null) anim.SetBool("isWalking", false); 

            if (waitTimer >= currentWaitTime)
            {
                isWaiting = false;
                MoveToRandomTarget();
            }
        }
        else
        {
            if (anim != null) anim.SetBool("isWalking", true);
        }
    }

    void MoveToRandomTarget()
    {
        if (!agent.isOnNavMesh || isPlayerInBoxCollider) return;

        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;

        NavMeshHit navHit;
        if (NavMesh.SamplePosition(randomDirection, out navHit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(navHit.position);
            agent.isStopped = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInBoxCollider = true;
            playerTransform = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInBoxCollider = false;
            MoveToRandomTarget(); 
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
        
        // Menampilkan garis jangkauan tangkap di editor biar mudah dipantau
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, catchDistance);
    }
}