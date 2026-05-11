using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Налаштування атаки")]
    [SerializeField] private Transform attackPoint; // Точка, звідки йде удар
    [SerializeField] private float attackRange = 0.5f; // Радіус ураження
    [SerializeField] private int attackDamage = 25; // Скільки ХП знімаємо
    [SerializeField] private float attackCooldown = 0.5f; // Затримка між ударами

    [SerializeField] private LayerMask enemyLayers; // Вказуємо шар ворогів

    private float nextAttackTime = 0f;

    void Update()
    {
        // Перевіряємо, чи пройшов кулдаун
        if (Time.time >= nextAttackTime)
        {
            // Б'ємо на ліву кнопку миші (0) або на клавішу F
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.F))
            {
                Attack();
                // Задаємо час, коли можна вдарити наступного разу
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void Attack()
    {
        // 1. Створюємо невидиме коло і знаходимо всіх ворогів, які в нього потрапили
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // 2. Проходимося по кожному знайденому ворогу і наносимо шкоду
        foreach (Collider2D enemy in hitEnemies)
        {
            Health enemyHealth = enemy.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage);
                Debug.Log("Гравець вдарив ворога!");
            }
        }
    }

    // Ця функція просто малює коло в редакторі Unity, щоб ти міг налаштувати радіус
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}