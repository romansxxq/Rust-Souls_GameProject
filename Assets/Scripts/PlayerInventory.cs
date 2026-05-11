using UnityEngine;
using UnityEngine.UI; // Для роботи з базовим UI

public class PlayerInventory : MonoBehaviour
{
    [Header("Ресурси")]
    [SerializeField] private int rustCount = 0;
    [SerializeField] private int soulsCount = 0;

    [Header("UI Елементи")]
    public TMPro.TextMeshProUGUI rustText;
    public TMPro.TextMeshProUGUI soulsText;

    // Метод для додавання ресурсів
    public void AddResource(string type, int amount)
    {
        if (type == "Rust")
        {
            rustCount += amount;
            if (rustText != null) rustText.text = "Rust: " + rustCount;
        }
        else if (type == "Soul")
        {
            soulsCount += amount;
            if (soulsText != null) soulsText.text = "Souls: " + soulsCount;
        }
    }
}