using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneUI : MonoBehaviour
{
    public GameObject pressAnyKeyText;
    public GameObject startButtonPanel;
    public GameObject difficultyPanel;
    public GameObject fadeTextAttorney;
    public GameObject fadeTextKneesTorned;

    public Button startButton;
    public Button knessButton;
    public Button attorneyButton;
    public Button settingsButton;
    public Button exitButtonStart;
    public Button exitButtonDifficulty;
    public Button lawlessnessButton;
    public Button[] difficultyButtons;

    private bool hasPressedKey = false;

    
    
    void Start()
    {
        Debug.Log("[Start] TitleSceneUI.cs yüklendi.");

        
        
        if (!PlayerPrefs.HasKey("difficulty"))
        {
            PlayerPrefs.SetInt("difficulty", 2);
            PlayerPrefs.Save();
        }
                

        if (difficultyButtons == null || difficultyButtons.Length == 0)
        {
            difficultyButtons = new Button[]
            {
            GameObject.Find("AttorneyButton")?.GetComponent<Button>(),
            GameObject.Find("KnessTornedButton")?.GetComponent<Button>(),
            GameObject.Find("LawlessnessButton")?.GetComponent<Button>()
            };
            
        }

        foreach (var btn in difficultyButtons)
        {
            if (btn != null)
            {
                btn.interactable = false;
            }
        }

        int unlockedLevel = PlayerPrefs.GetInt("difficulty");       

        for (int i = 0; i < unlockedLevel && i<difficultyButtons.Length; i++)
        {
            if (difficultyButtons[i] != null)
            {
                difficultyButtons[i].interactable = true;                
            }
        }

        // Sadece açýlabilir seviyedeki butonlarý aktif et
        for (int i = 0; i < unlockedLevel; i++)
        {
            if (i < difficultyButtons.Length && difficultyButtons[i] != null)
            {
                difficultyButtons[i].interactable = true;
            }
        }

        startButtonPanel.SetActive(false);
        difficultyPanel.SetActive(false);
        fadeTextAttorney.gameObject.SetActive(false);
        fadeTextKneesTorned.gameObject.SetActive(false);

       


        // Butonlara event ekleme
        startButton.onClick.AddListener(NewGame);
        attorneyButton.onClick.AddListener(() => SetDifficulty(1));
        knessButton.onClick.AddListener(() => SetDifficulty(2));
        lawlessnessButton.onClick.AddListener(() => SetDifficulty(3));
        settingsButton.onClick.AddListener(Settings);
        exitButtonStart.onClick.AddListener(ExitGame);
        exitButtonDifficulty.onClick.AddListener(Back);
               
    }

    void Update()
    {
        if (!hasPressedKey && Input.anyKeyDown)
        {
            hasPressedKey = true; // Tekrar çalýþmasýný önlemek için
            pressAnyKeyText.SetActive(false);
            startButtonPanel.SetActive(true);
        }
    }

    public void NewGame()
    {
        startButtonPanel.SetActive(false);
        difficultyPanel.SetActive(true);
    }

    public void SetDifficulty(int difficultylevel)
    {       
        GameManager.Instance.SetDifficulty(difficultylevel);
        difficultyPanel.SetActive(false);
        SceneController.Instance.LoadScene(SceneController.SceneName.HubScene);
    }

    public void Settings()
    {
        startButtonPanel.SetActive(false);
        
    }

    public void Back()
    {
        difficultyPanel.SetActive(false);
        startButtonPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}



