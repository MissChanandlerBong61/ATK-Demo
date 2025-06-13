using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Resume()
    {
        GameManager.Instance.ResumeGame();
    }

    public void MainMenu()
    {
        GameManager.Instance.ForceUnpause();
        SceneController.Instance.LoadScene(SceneController.SceneName.TitleScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
