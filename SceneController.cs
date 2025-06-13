using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour
{
    public enum SceneName
    {
        TitleScene,
        HubScene,
        CourtHouseScene,
        DAOffice,
        AttorneyLounge,
        PublicRecords,
        DemoFinal
    }

    public static SceneController Instance;

    [SerializeField] private GameObject fadeCanvasPrefab; 
    [SerializeField] private GameObject loadingCanvasPrefab;
    
    
    private FadeCanvas fadeCanvas;    
    private LoadingUI loadingUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (fadeCanvasPrefab != null)
            {
                GameObject fadeCanvasObject = Instantiate(fadeCanvasPrefab);
                DontDestroyOnLoad(fadeCanvasObject);
                fadeCanvas = fadeCanvasObject.GetComponent<FadeCanvas>();
            }

            if (loadingCanvasPrefab != null)
            {
                GameObject loadingCanvasObject = Instantiate(loadingCanvasPrefab);
                DontDestroyOnLoad(loadingCanvasObject);
                loadingUI = loadingCanvasObject.GetComponent<LoadingUI>();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadScene(SceneName sceneName)
    {
        string sceneString = ConvertToSceneString(sceneName);
        StartCoroutine(TransitionSceneAsync(sceneString));
    }


    private IEnumerator TransitionSceneAsync(string sceneName)
    {
        if (fadeCanvas != null)
        {
            yield return fadeCanvas.FadeIn();
        }
    
        if (loadingUI != null)
        {
            loadingUI.Show();
        }

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (operation.progress < 0.9f)
        {
            loadingUI?.SetProgress(operation.progress);
            yield return null;
        }

        loadingUI?.SetProgress(1f);
        yield return new WaitForSeconds(0.5f);
        operation.allowSceneActivation = true;


        yield return null;

        if (fadeCanvas != null)
        {
            yield return fadeCanvas.FadeOut();
        }

        loadingUI?.Hide();
    }


    public void ReloadCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        StartCoroutine(TransitionSceneAsync(currentSceneName));
    }

    private string ConvertToSceneString(SceneName scene)
    {
        return scene switch
        {
            SceneName.TitleScene => "0_TitleScene",
            SceneName.HubScene => "1_HubScene",
            SceneName.CourtHouseScene => "2_CourtHouse",
            SceneName.AttorneyLounge => "3_AttorneyLounge",
            SceneName.DAOffice => "4_DAOffice",
            SceneName.PublicRecords => "5_PublicRecords",
            SceneName.DemoFinal => "6_DemoFinal",
            _ => "0_TitleScene"
        };
    }

    public void LoadNextScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(nextIndex);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            if (!string.IsNullOrEmpty(sceneName))
            {
                StartCoroutine(TransitionSceneAsync(sceneName));
            }
            else
            {
                Debug.LogError($"Scene name at index {nextIndex} is invalid or empty.");
            }
        }
        else
        {
            Debug.LogWarning("No more scenes to load. Reloading current scene.");
            ReloadCurrentScene();
        }
    }

}
