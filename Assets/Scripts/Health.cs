using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Налаштування здоров'я")]
    public int maxHealth = 100;
    public int currentHealth;

    // ДОДАНО: Прапорець невразливості
    public bool isInvincible = false; 

    [Header("Події")]
    public UnityEvent onTakeDamage;
    public UnityEvent onDeath;
    [Header("Лут при смерті (тільки для ворогів)")]
    public GameObject dropPrefab;
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        // ДОДАНО: Якщо ми у стані ухилення - ігноруємо шкоду!
        if (isInvincible) 
        {
            Debug.Log("Ухилення спрацювало! Шкоди немає.");
            return;
        }

        currentHealth -= damage;
        onTakeDamage.Invoke(); 

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        onDeath.Invoke(); 

        if (gameObject.CompareTag("Player"))
        {
            Debug.Log("Гравець помер!");
            gameObject.SetActive(false);
        }
        else 
        {
            if (dropPrefab != null)
            {
                // Створюємо префаб на місці ворога
                Instantiate(dropPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}