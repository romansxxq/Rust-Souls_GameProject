using UnityEngine;

/// <summary>
/// Визначає умову перемоги на рівні.
/// ReachZone   — гравець доходить до тригерної зони (розмістіть на окремому об'єкті з Collider2D Is Trigger).
/// KillAllEnemies — всі вороги знищені.
/// CollectResources — гравець зібрав задану кількість Rust і Souls (з GameConfig).
/// </summary>
public class LevelGoal : MonoBehaviour
{
    public enum WinCondition { ReachZone, KillAllEnemies, CollectResources }

    [SerializeField] private WinCondition condition = WinCondition.KillAllEnemies;
    [SerializeField] private string enemyTag = "Enemy";

    private bool goalReached = false;
    private int initialEnemyCount = 0;

    private void Start()
    {
        initialEnemyCount = GameObject.FindGameObjectsWithTag(enemyTag).Length;
        if (initialEnemyCount == 0 && condition == WinCondition.KillAllEnemies)
            Debug.LogWarning($"[LevelGoal] Жодного об'єкта з тегом '{enemyTag}' не знайдено! Перевір теги ворогів.");
    }

    private void Update()
    {
        if (goalReached) return;

        if (condition == WinCondition.KillAllEnemies)
        {
            if (initialEnemyCount == 0) return; // чекаємо поки вороги з'являться
            GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
            if (enemies.Length == 0)
                TriggerWin();
        }
        else if (condition == WinCondition.CollectResources)
        {
            CheckResourceGoal();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (goalReached) return;
        if (condition == WinCondition.ReachZone && other.CompareTag("Player"))
            TriggerWin();
    }

    private void CheckResourceGoal()
    {
        GameConfig cfg = GameManager.Instance?.Config;
        if (cfg == null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        PlayerInventory inv = player.GetComponent<PlayerInventory>();
        if (inv == null) return;

        bool rustOk  = cfg.rustToWin  <= 0 || inv.RustCount  >= cfg.rustToWin;
        bool soulsOk = cfg.soulsToWin <= 0 || inv.SoulsCount >= cfg.soulsToWin;

        if (rustOk && soulsOk)
            TriggerWin();
    }

    private void TriggerWin()
    {
        goalReached = true;
        GameManager.Instance?.WinLevel();
    }
}
