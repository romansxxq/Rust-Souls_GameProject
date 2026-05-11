using UnityEngine;
using TMPro; // Обов'язково для TextMeshPro
using System.Reflection;

public class PlayerUI : MonoBehaviour
{
    [Header("Зв'язок з даними")]
    public Health playerHealth; // Посилання на здоров'я гравця
    public TextMeshProUGUI hpText; // Посилання на текст на екрані
    private FieldInfo currentHealthField;

    void Update()
    {
        // Якщо у нас є текст і скрипт здоров'я - оновлюємо цифри
        if (playerHealth != null && hpText != null)
        {
            if (currentHealthField == null)
            {
                currentHealthField = playerHealth.GetType().GetField("currentHealth", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            }

            var value = currentHealthField != null ? currentHealthField.GetValue(playerHealth) : null;
            hpText.text = "HP: " + (value != null ? value.ToString() : "0");
        }
    }
}