using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    // Тип ресурсу: "Rust" або "Soul"
    public string resourceType = "Rust"; 
    public int amount = 1;

    // Спрацьовує, коли щось торкається Тригера
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Перевіряємо, чи це гравець
        if (collision.CompareTag("Player"))
        {
            // Шукаємо інвентар на гравці
            PlayerInventory inventory = collision.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                // Додаємо ресурс
                inventory.AddResource(resourceType, amount);
                
                // Знищуємо предмет зі сцени
                Destroy(gameObject);
            }
        }
    }
}