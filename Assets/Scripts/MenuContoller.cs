using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuContoller : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private string startSceneName = "Level1";

    // Кнопка "Почати гру" — завантажує перший рівень
    public void StartButton()
    {
        if (!string.IsNullOrWhiteSpace(startSceneName))
            SceneManager.LoadScene(startSceneName);
    }

    // Кнопка "Продовжити" — перезавантажує останній збережений рівень
    public void ContinueButton()
    {
        string lastScene = PlayerPrefs.GetString("LastScene", startSceneName);
        SceneManager.LoadScene(lastScene);
    }

    // Кнопка "Вибір рівня" — завантажує конкретну сцену за назвою
    public void LoadLevelButton(string sceneName)
    {
        if (!string.IsNullOrWhiteSpace(sceneName))
            SceneManager.LoadScene(sceneName);
    }

    // Кнопка "Вихід"
    public void ExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

