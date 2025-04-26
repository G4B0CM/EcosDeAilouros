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

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        // Desactivar comportamientos
        agent.isStopped = true;
        monsterScript.enabled = false;

        // Animaci�n de muerte
        animator.SetTrigger("Die");

        // Destruir al terminar animaci�n (opcional)
        Destroy(gameObject, 3f); // Aseg�rate que la animaci�n dure < 3 segundos o ajusta el tiempo
    }
}
