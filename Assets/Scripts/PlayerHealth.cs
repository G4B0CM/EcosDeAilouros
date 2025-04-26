using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private TMP_Text texto;
    [SerializeField] private TMP_Text vidas;

    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        vidas.text = currentHealth.ToString();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        vidas.text = currentHealth.ToString();

        if (currentHealth <= 0)
        {
            texto.text = "¡Has muerto!";
            Invoke("Die", 2f);
        }
    }

    private void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
