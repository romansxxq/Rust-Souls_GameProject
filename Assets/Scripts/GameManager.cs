using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Конфігурація рівня")]
    [SerializeField] private GameConfig config;

    [Header("Панелі стану гри")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject victoryPanel;
    [SerializeField] private GameObject defeatPanel;

    private bool isPaused = false;
    private int currentLives;

    public int CurrentLives => currentLives;
    public GameConfig Config => config;
    public bool IsGameOver { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        currentLives = config != null ? config.startingLives : 3;
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Health health = player.GetComponent<Health>();
            if (health != null)
                health.onDeath.AddListener(OnPlayerDied);
        }

        SetPanels(false, false, false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !IsGameOver)
            TogglePause();
    }

    // ── Пауза ──────────────────────────────────────────────
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        if (pausePanel != null) pausePanel.SetActive(isPaused);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    // ── Перемога ────────────────────────────────────────────
    public void WinLevel()
    {
        if (IsGameOver) return;
        IsGameOver = true;
        Time.timeScale = 0f;
        SetPanels(false, true, false);
    }

    // ── Смерть гравця ───────────────────────────────────────
    private void OnPlayerDied()
    {
        if (IsGameOver) return;
        currentLives--;
        IsGameOver = true;
        Time.timeScale = 0f;
        SetPanels(false, false, true);
    }

    // ── Навігація ───────────────────────────────────────────
    public void RestartLevel()
    {
        ResetState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        ResetState();
        if (config != null && !string.IsNullOrWhiteSpace(config.nextSceneName))
            SceneManager.LoadScene(config.nextSceneName);
        else
            GoToMenu();
    }

    public void GoToMenu()
    {
        ResetState();
        string menu = config != null ? config.menuSceneName : "MainMenu";
        SceneManager.LoadScene(menu);
    }

    // ── Утиліти ─────────────────────────────────────────────
    private void SetPanels(bool pause, bool victory, bool defeat)
    {
        if (pausePanel != null)   pausePanel.SetActive(pause);
        if (victoryPanel != null) victoryPanel.SetActive(victory);
        if (defeatPanel != null)  defeatPanel.SetActive(defeat);
    }

    private void ResetState()
    {
        IsGameOver = false;
        isPaused = false;
        Time.timeScale = 1f;
    }
}
