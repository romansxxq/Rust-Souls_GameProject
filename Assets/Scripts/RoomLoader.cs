using UnityEngine;

/// <summary>
/// Завантажує prefab кімнати відповідно до типу вузла графу.
///
/// Налаштування в Inspector:
///   - Для кожного RoomType перетягни відповідний prefab у поле
///   - Attach до того ж GameObject що і RunGenerator
///
/// Як використовувати:
///   1. На початку сцени викликається GenerateRun() — будується граф
///   2. Гравець натискає кнопку переходу (UI) → викликається GoToRoom(node)
///   3. RoomLoader знищує поточну кімнату і спаунить нову
/// </summary>
[RequireComponent(typeof(RunGenerator))]
public class RoomLoader : MonoBehaviour
{
    [Header("Prefabi кімнат (перетягни з Assets/Prefabs/Rooms/)")]
    [SerializeField] private GameObject roomStart;
    [SerializeField] private GameObject roomCombat;
    [SerializeField] private GameObject roomElite;
    [SerializeField] private GameObject roomEvent;
    [SerializeField] private GameObject roomShop;
    [SerializeField] private GameObject roomRest;
    [SerializeField] private GameObject roomBoss;

    [Header("Точка спауну кімнати (лівий край сцени)")]
    [SerializeField] private Transform spawnPoint;

    private RunGenerator generator;
    private GameObject   currentRoomInstance;

    // ── Ініціалізація ──────────────────────────────────────────────────────────

    private void Start()
    {
        generator = GetComponent<RunGenerator>();

        // Генеруємо граф і завантажуємо стартову кімнату
        RoomNode root = generator.GenerateRun();
        LoadRoom(root);
    }

    // ── Публічне API ──────────────────────────────────────────────────────────

    /// <summary>
    /// Викликай при натисканні кнопки переходу до наступної кімнати.
    /// node — один з вузлів CurrentNode.next, обраний гравцем.
    /// </summary>
    public void GoToRoom(RoomNode node)
    {
        generator.EnterRoom(node);
        LoadRoom(node);
    }

    // ── Завантаження prefab ───────────────────────────────────────────────────

    private void LoadRoom(RoomNode node)
    {
        // Знищуємо стару кімнату
        if (currentRoomInstance != null)
            Destroy(currentRoomInstance);

        GameObject prefab = GetPrefabForType(node.type);

        if (prefab == null)
        {
            Debug.LogWarning($"[RoomLoader] Prefab для типу {node.type} не призначено!");
            return;
        }

        Vector3 pos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        currentRoomInstance = Instantiate(prefab, pos, Quaternion.identity);
    }

    private GameObject GetPrefabForType(RoomType type)
    {
        return type switch
        {
            RoomType.Start  => roomStart,
            RoomType.Combat => roomCombat,
            RoomType.Elite  => roomElite,
            RoomType.Event  => roomEvent,
            RoomType.Shop   => roomShop,
            RoomType.Rest   => roomRest,
            RoomType.Boss   => roomBoss,
            _               => roomCombat
        };
    }
}
