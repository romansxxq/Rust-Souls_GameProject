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
    [Header("Animation Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private bool useBoolForWalk = true;
    [SerializeField] private string walkFloatParam = "Speed";
    [SerializeField] private string walkBoolParam = "IsWalking";
    [SerializeField] private float walkThreshold = 0.01f;
    [SerializeField] private bool useBoolForRun = false;
    [SerializeField] private string runBoolParam = "Running";
    [SerializeField] private float dashAnimSpeed = 1.5f;
    [Header("Attack Settings")]
    [SerializeField] private bool useMouse0ForAttack = true;
    [SerializeField] private bool useKeyForAttack = true;
    [SerializeField] private KeyCode attackKey = KeyCode.F;
    [SerializeField] private string attackBoolParam = "isAttack";
    [SerializeField] private float attackDuration = 0.4f;
    [SerializeField] private float attackCooldown = 0.1f;
    [Header("Hurt/Death Settings")]
    [SerializeField] private string hurtBoolParam = "isHurt";
    [SerializeField] private float hurtDuration = 0.2f;
    [SerializeField] private string deathBoolParam = "isDead";

    private Rigidbody2D rb;
    private float moveInputX;

    private bool isGrounded;
    private bool isDashing = false;
    private bool canDash = true;
    private bool isAttacking = false;
    private bool isDead = false;
    private Coroutine hurtCoroutine;
    private Health PlayerHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        PlayerHealth = GetComponent<Health>();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
        if (PlayerHealth != null)
        {
            PlayerHealth.onTakeDamage.AddListener(HandleTakeDamage);
            PlayerHealth.onDeath.AddListener(HandleDeath);
        }
    }

    private void OnDestroy()
    {
        if (PlayerHealth != null)
        {
            PlayerHealth.onTakeDamage.RemoveListener(HandleTakeDamage);
            PlayerHealth.onDeath.RemoveListener(HandleDeath);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;

        if (isDashing)
        {
            UpdateRunAnimation(true);
            if (useBoolForWalk)
            {
                UpdateWalkAnimation(0f);
            }
            else
            {
                UpdateWalkAnimation(dashAnimSpeed);
            }
            return;
        }

        moveInputX = Input.GetAxisRaw("Horizontal");
        // moveInputY = Input.GetAxisRaw("Vertical");
        UpdateRunAnimation(false);
        UpdateWalkAnimation(Mathf.Abs(moveInputX));

        if (IsAttackInput() && !isAttacking)
        {
            StartCoroutine(Attack());
        }

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
        if (isDead) return;
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
        UpdateRunAnimation(true);
        if (useBoolForWalk)
        {
            UpdateWalkAnimation(0f);
        }
        else
        {
            UpdateWalkAnimation(dashAnimSpeed);
        }

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
        UpdateRunAnimation(false);
        
        if (PlayerHealth != null)
        {
            PlayerHealth.isInvincible = false; // Вимикаємо невразливість після ривка
        }
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void UpdateWalkAnimation(float speed)
    {
        if (animator == null) return;

        if (useBoolForWalk)
        {
            if (!string.IsNullOrEmpty(walkBoolParam))
            {
                animator.SetBool(walkBoolParam, speed > walkThreshold);
            }
        }
        else
        {
            if (!string.IsNullOrEmpty(walkFloatParam))
            {
                animator.SetFloat(walkFloatParam, speed);
            }
        }
    }

    private void UpdateRunAnimation(bool isRunning)
    {
        if (animator == null) return;

        if (useBoolForRun && !string.IsNullOrEmpty(runBoolParam))
        {
            animator.SetBool(runBoolParam, isRunning);
        }
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        UpdateAttackAnimation(true);

        yield return new WaitForSeconds(attackDuration);

        UpdateAttackAnimation(false);
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    private void HandleTakeDamage()
    {
        if (isDead) return;

        if (hurtCoroutine != null)
        {
            StopCoroutine(hurtCoroutine);
        }
        hurtCoroutine = StartCoroutine(Hurt());
    }

    private void HandleDeath()
    {
        if (isDead) return;

        isDead = true;
        isDashing = false;
        isAttacking = false;
        UpdateWalkAnimation(0f);
        UpdateRunAnimation(false);
        UpdateAttackAnimation(false);
        UpdateHurtAnimation(false);
        UpdateDeathAnimation(true);
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private IEnumerator Hurt()
    {
        UpdateHurtAnimation(true);

        yield return new WaitForSeconds(hurtDuration);

        UpdateHurtAnimation(false);
        hurtCoroutine = null;
    }

    private bool IsAttackInput()
    {
        if (useMouse0ForAttack && Input.GetMouseButtonDown(0)) return true;
        if (useKeyForAttack && attackKey != KeyCode.None && Input.GetKeyDown(attackKey)) return true;
        return false;
    }

    private void UpdateAttackAnimation(bool isActive)
    {
        if (animator == null) return;

        if (!string.IsNullOrEmpty(attackBoolParam))
        {
            animator.SetBool(attackBoolParam, isActive);
        }
    }

    private void UpdateHurtAnimation(bool isActive)
    {
        if (animator == null) return;

        if (!string.IsNullOrEmpty(hurtBoolParam))
        {
            animator.SetBool(hurtBoolParam, isActive);
        }
    }

    private void UpdateDeathAnimation(bool isActive)
    {
        if (animator == null) return;

        if (!string.IsNullOrEmpty(deathBoolParam))
        {
            animator.SetBool(deathBoolParam, isActive);
        }
    }
}
