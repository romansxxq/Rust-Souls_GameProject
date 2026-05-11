using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuContoller : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private string startSceneName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartButton()
    {
        if (!string.IsNullOrWhiteSpace(startSceneName))
        {
            SceneManager.LoadScene(startSceneName);
        }
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
