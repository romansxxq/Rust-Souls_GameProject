using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Header("Налаштування здоров'я")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int currentHealth;
    [SerializeField] private bool disablePlayerOnDeath = false;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

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
            if (disablePlayerOnDeath)
            {
                gameObject.SetActive(false);
            }
        }
        else 
        {
            if (dropPrefab != null)
            {
                // Створюємо префаб на місці ворога
                SimpleObjectPool.Instance.Spawn(dropPrefab, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}