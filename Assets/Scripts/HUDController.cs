using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("Здоров'я")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Ресурси")]
    [SerializeField] private TextMeshProUGUI rustText;
    [SerializeField] private TextMeshProUGUI soulsText;

    [Header("Стан гравця")]
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI levelText;

    private Health playerHealth;
    private PlayerInventory playerInventory;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerHealth = player.GetComponent<Health>();
            playerInventory = player.GetComponent<PlayerInventory>();

            if (playerHealth != null)
            {
                playerHealth.onTakeDamage.AddListener(RefreshHealthUI);
                playerHealth.onDeath.AddListener(RefreshHealthUI);

                if (healthSlider != null)
                    healthSlider.maxValue = playerHealth.MaxHealth;
            }
        }

        if (levelText != null && GameManager.Instance?.Config != null)
            levelText.text = "Рівень: " + GameManager.Instance.Config.levelNumber;

        RefreshHealthUI();
    }

    private void Update()
    {
        RefreshResourceUI();
        RefreshLivesUI();
    }

    private void RefreshHealthUI()
    {
        if (playerHealth == null) return;

        int hp = playerHealth.CurrentHealth;
        int maxHp = playerHealth.MaxHealth;

        if (healthSlider != null)
            healthSlider.value = hp;

        if (healthText != null)
            healthText.text = $"HP: {hp} / {maxHp}";
    }

    private void RefreshResourceUI()
    {
        if (playerInventory == null) return;

        if (rustText != null)
            rustText.text = "Rust: " + playerInventory.RustCount;

        if (soulsText != null)
            soulsText.text = "Souls: " + playerInventory.SoulsCount;
    }

    private void RefreshLivesUI()
    {
        if (livesText == null || GameManager.Instance == null) return;
        livesText.text = "Життя: " + GameManager.Instance.CurrentLives;
    }
}
