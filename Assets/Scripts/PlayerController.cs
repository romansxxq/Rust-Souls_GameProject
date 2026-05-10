using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Move and jump Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 15f;
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    private Rigidbody2D rb;
    private float moveInputX;

    private bool isGrounded;
    private bool isDashing = false;
    private bool canDash = true;
    private Health PlayerHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        PlayerHealth = GetComponent<Health>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing) return;

        moveInputX = Input.GetAxisRaw("Horizontal");
        // moveInputY = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (Input.GetKeyDown(KeyCode.Space) && canDash && moveInputX != 0)
        {
            StartCoroutine(Dash());
        }
        
    }
    void FixedUpdate()
    {
        // Фізику рухаємо у FixedUpdate
        if (isDashing) return;

        // moveInputX гарантує, що рух по діагоналі не буде швидшим
        rb.linearVelocity = new Vector2(moveInputX * moveSpeed, rb.linearVelocity.y);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Якщо доторкнулися до об'єкта з тегом Ground - ми на землі
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Якщо перестали торкатися Ground - ми в повітрі
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        if (PlayerHealth != null)
        {
            PlayerHealth.isInvincible = true; // Робимо гравця невразливим під час ривка
        }
        // Запам'ятовуємо поточну гравітацію і вимикаємо її, щоб гравець летів прямо, а не падав
        float originalGravity = rb.gravityScale; 
        rb.gravityScale = 0f; 
        
        float dashDirection = moveInputX != 0 ? moveInputX : Mathf.Sign(transform.localScale.x);
        rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0f);

        yield return new WaitForSeconds(dashDuration);
        
        // Повертаємо гравітацію після ривка
        rb.gravityScale = originalGravity; 
        isDashing = false;
        
        if (PlayerHealth != null)
        {
            PlayerHealth.isInvincible = false; // Вимикаємо невразливість після ривка
        }
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}
