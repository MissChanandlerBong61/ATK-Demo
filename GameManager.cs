using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PauseSource
{
    Player,
    NPC,
    UI,
    Other
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}
    public TitleSceneUI TitleSceneUI;    
    
     
    private Stack<PauseSource> pauseStack = new Stack<PauseSource>();
    public PauseSource CurrentPauseSource { get; private set; }
    public bool isGamePaused => pauseStack.Count > 0;
    
    
    private int difficulty;
    private string startName = "Muhittin";
    public string charName;

    private void Awake()
    {
        if (Instance !=null && Instance != this)
        {           
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        //az daha cianý býcaklýyodm
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        difficulty = PlayerPrefs.GetInt("difficulty", 1);

        if (string.IsNullOrEmpty(PlayerPrefs.GetString("charName")))
            {
            charName = "You"; 
            PlayerPrefs.SetString("charName", startName);
            PlayerPrefs.Save();
        }
        if (SceneManager.GetActiveScene().name == "0_TitleScene")
        {
            
            TitleSceneUI = Object.FindFirstObjectByType<TitleSceneUI>();
        }

    }

// Update is called once per frame
    void Update()
    {
        if (SceneManager.GetActiveScene().name == "0_TitleScene") return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UIController.Instance != null && (UIController.Instance.IsDialogueOpen() || UIController.Instance.IsChargesheetOpen()))
            {
                UIController.Instance.CloseDialogue();
                ResumeGame();
                return;
            }

            if (isGamePaused)
            {
                ResumeGame();
            }

            else
            {
                PauseGame(PauseSource.Player);
            }
        }
    }

    public void PauseGame(PauseSource source)
    {
        bool wasAlreadyPaused = isGamePaused;
        pauseStack.Push(source);
        CurrentPauseSource = source;

        if (!wasAlreadyPaused)
        {
            Time.timeScale = 0f;
            AudioListener.pause = true;

            PlayerMovement pm = FindAnyObjectByType<PlayerMovement>();
            if (pm != null) pm.enabled = false;
        }
      
    }

    public void ResumeGame()
    {
        if (pauseStack.Count > 0)
        {
            pauseStack.Pop();

            if (pauseStack.Count > 0)
            {
                CurrentPauseSource = pauseStack.Peek();
            }
            else
            {
                Time.timeScale = 1f;
                AudioListener.pause = false;

                PlayerMovement pm = FindAnyObjectByType<PlayerMovement>();
                if (pm != null) pm.enabled = true;
            }

        }
        
    }
    public void ForceUnpause()
    {
        pauseStack.Clear(); 
        
        Time.timeScale = 1f;
        AudioListener.pause = false;

        PlayerMovement pm = FindAnyObjectByType<PlayerMovement>();
        if (pm != null) pm.enabled = true;
    }

    public void SetDifficulty(int difficultylevel)
    {
        difficulty = difficultylevel;
        PlayerPrefs.SetInt("difficulty", difficultylevel);
        PlayerPrefs.Save();        
    }

    public int GetDifficulty()
    {
        return difficulty;
    }
}
