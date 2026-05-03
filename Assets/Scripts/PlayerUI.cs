using UnityEngine;
using TMPro; // Обов'язково для TextMeshPro

public class PlayerUI : MonoBehaviour
{
    [Header("Зв'язок з даними")]
    public Health playerHealth; // Посилання на здоров'я гравця
    public TextMeshProUGUI hpText; // Посилання на текст на екрані

    void Update()
    {
        // Якщо у нас є текст і скрипт здоров'я - оновлюємо цифри
        if (playerHealth != null && hpText != null)
        {
            hpText.text = "HP: " + playerHealth.currentHealth;
        }
    }
}