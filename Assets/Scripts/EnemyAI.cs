using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] private int damage = 10;
    
    private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        // Ворог сам шукає гравця на сцені за тегом!
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        // Визначаємо напрямок до гравця (вліво чи вправо)
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        
        // Якщо гравець далі ніж на 0.6 юніта - йдемо до нього
        if (Mathf.Abs(player.position.x - transform.position.x) > 0.6f)
        {
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        }
        else
        {
            // Зупиняємось, щоб просто бити впритул
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    // Якщо ворог торкається гравця - наносить шкоду
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                // Тут викликається скрипт ХП. Якщо гравець у Dash (isInvincible), шкода не пройде!
                playerHealth.TakeDamage(damage);
            }
        }
    }
}