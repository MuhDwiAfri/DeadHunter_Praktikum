using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    private Animator enemyAnimator;
    private bool isDead = false;

    void Start()
    {
        enemyAnimator = GetComponent<Animator>();
    }

    public void TakeStealthDeath()
{
    if (isDead) return;
    isDead = true;

    if (enemyAnimator != null)
    {
        enemyAnimator.SetTrigger("Die");
    }

    // MATIKAN COLLIDER
    Collider enemyCollider = GetComponent<Collider>();
    if (enemyCollider != null) enemyCollider.enabled = false;

    // JIKA MUSUH PAKAI CHARACTER CONTROLLER, WAJIB MATIKAN JUGA:
    CharacterController enemyCc = GetComponent<CharacterController>();
    if (enemyCc != null) enemyCc.enabled = false;

    // JIKA PAKAI NAVMESH AGENT:
    UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    if (agent != null) agent.enabled = false; 
}
}