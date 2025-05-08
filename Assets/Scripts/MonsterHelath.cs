using UnityEngine;
using UnityEngine.AI;

public class MonsterHealth : MonoBehaviour
{
    [SerializeField] public int health = 3;


    int currentHealth;
    Animator animator;
    NavMeshAgent agent;
    Monster monsterScript; 

    private bool isDead = false;

    private void Start()
    {
        currentHealth = health;
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        monsterScript = GetComponent<Monster>();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        animator.ResetTrigger("GetHit");
        animator.SetTrigger("GetHit");
        monsterScript.OnHit();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        agent.isStopped = true;
        monsterScript.enabled = false;

        animator.SetTrigger("Die");

        Destroy(gameObject, 3f); 
    }
}
