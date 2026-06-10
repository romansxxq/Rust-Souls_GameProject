using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Генерує граф кімнат для roguelike-забігу (Rust &amp; Souls).
///
/// Структура:
///   Start → Combat/Event/Elite → ... → (Shop або Rest кожні 5) → ... → Boss
///
/// Attach до того ж GameObject що і RoomLoader.
/// Викликай GenerateRun() на початку кожного забігу.
/// </summary>
public class RunGenerator : MonoBehaviour
{
    [Header("Параметри (або берутся з GameConfig)")]
    [SerializeField] private int roomsPerRun = 15;
    [SerializeField] private int pathChoices = 2;
    [SerializeField] [Range(0f, 1f)] private float eliteChance = 0.15f;
    [SerializeField] [Range(0f, 1f)] private float eventChance = 0.25f;
    [SerializeField] [Range(0f, 1f)] private float shopChance  = 0.10f;

    public RoomNode RootNode    { get; private set; }
    public RoomNode CurrentNode { get; private set; }

    // ── Ініціалізація з GameConfig якщо є ─────────────────────────────────────

    private void Awake()
    {
        if (GameManager.Instance != null && GameManager.Instance.Config != null)
        {
            var cfg = GameManager.Instance.Config;
            roomsPerRun  = cfg.roomsPerRun;
            pathChoices  = cfg.pathChoices;
            eliteChance  = cfg.eliteChance;
            eventChance  = cfg.eventChance;
            shopChance   = cfg.shopChance;
        }
    }

    // ── Публічне API ──────────────────────────────────────────────────────────

    /// <summary>
    /// Будує новий граф забігу. Повертає стартовий вузол.
    /// </summary>
    public RoomNode GenerateRun()
    {
        RootNode    = new RoomNode { type = RoomType.Start, isCurrent = true };
        CurrentNode = RootNode;
        BuildGraph(RootNode, 0);
        return RootNode;
    }

    /// <summary>
    /// Переводить гравця до вузла. Викликається з RoomLoader при переході.
    /// </summary>
    public void EnterRoom(RoomNode node)
    {
        if (CurrentNode != null)
            CurrentNode.isCurrent = false;

        node.visited   = true;
        node.isCurrent = true;
        CurrentNode    = node;
    }

    // ── Побудова графу ─────────────────────────────────────────────────────────

    private void BuildGraph(RoomNode node, int depth)
    {
        // Кінець — Boss
        if (depth >= roomsPerRun - 1)
        {
            node.next.Add(new RoomNode { type = RoomType.Boss });
            return;
        }

        // Перша і передфінальна кімнати — одна гілка (без розгалуження)
        bool single = (depth == 0 || depth == roomsPerRun - 2);
        int branches = single ? 1 : Mathf.Clamp(pathChoices, 1, 3);

        // Кожні 5 кімнат — гарантовано відновлення
        bool forceRecovery = (depth % 5 == 4);

        var usedTypes = new HashSet<RoomType>();

        for (int i = 0; i < branches; i++)
        {
            RoomType type = forceRecovery
                ? (i == 0 ? RoomType.Shop : RoomType.Rest)
                : RollType(usedTypes);

            usedTypes.Add(type);

            var child = new RoomNode { type = type };
            node.next.Add(child);
            BuildGraph(child, depth + 1);
        }
    }

    private RoomType RollType(HashSet<RoomType> used)
    {
        for (int attempt = 0; attempt < 6; attempt++)
        {
            float r = Random.value;
            RoomType t;

            if      (r < eliteChance)                          t = RoomType.Elite;
            else if (r < eliteChance + eventChance)            t = RoomType.Event;
            else if (r < eliteChance + eventChance + shopChance) t = RoomType.Shop;
            else                                               t = RoomType.Combat;

            if (!used.Contains(t)) return t;
        }
        return RoomType.Combat;
    }
}
