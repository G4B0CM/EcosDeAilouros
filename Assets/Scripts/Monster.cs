using StarterAssets;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    [SerializeField] float detectionRange = 5f;
    [SerializeField] float attackRange = 2f;
    [SerializeField] float wanderRadius = 3f;
    [SerializeField] float wanderDelay = 2f;
    [SerializeField] float walkSpeed = 0.5f;
    [SerializeField] float chaseSpeed = 2.1f;
    [SerializeField] float attackCooldown = 1f;

    Animator animator;
    FirstPersonController player;
    PlayerHealth playerHealth;
    NavMeshAgent agent;

    Vector3 originPosition;
    float nextWanderTime;
    float nextAttackTime;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = FindFirstObjectByType<FirstPersonController>();
        playerHealth = player.GetComponent<PlayerHealth>();
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        originPosition = transform.position;
        nextWanderTime = Time.time + wanderDelay;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isAttacking = stateInfo.IsName("Ataque") && stateInfo.normalizedTime < 1f;

        if (!isAttacking)
        {
            if (distanceToPlayer <= detectionRange)
            {
                animator.SetBool("jugadorDetectado", true);
                agent.isStopped = false;
                agent.speed = chaseSpeed;
                agent.SetDestination(player.transform.position);
            }
            else
            {
                animator.SetBool("jugadorDetectado", false);
                agent.isStopped = false;
                agent.speed = walkSpeed;
                Wander();
            }
        }
        else
        {
            agent.isStopped = true;
        }

        if (distanceToPlayer <= attackRange && !isAttacking)
        {
            animator.SetBool("jugadorEnRango", true);

            if (Time.time >= nextAttackTime)
            {
                playerHealth.TakeDamage(1);
                nextAttackTime = Time.time + attackCooldown;
            }

            agent.isStopped = true;
        }
        else
        {
            animator.SetBool("jugadorEnRango", false);
        }

        animator.SetFloat("velocidad", agent.velocity.magnitude);
    }

    void Wander()
    {
        if (Time.time >= nextWanderTime && agent.remainingDistance < 0.5f && !agent.pathPending)
        {
            Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
            randomDirection += originPosition;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, wanderRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(navHit.position);
                nextWanderTime = Time.time + wanderDelay;
            }
        }
    }
}
