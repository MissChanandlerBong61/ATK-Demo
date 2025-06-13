using UnityEngine;
using UnityEngine.SceneManagement;


public class NextLevel : MonoBehaviour
{

    public GameObject collusionText;
    private bool isClosetoNextLevel = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collusionText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isClosetoNextLevel && Input.GetKeyDown(KeyCode.F))
        {            
            LoadNextLevel();
        }        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isClosetoNextLevel = true;
            collusionText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isClosetoNextLevel = false;
            collusionText.SetActive(false);
        }
    }


    public void LoadNextLevel()
    {
        if (SceneController.Instance == null)
        {
            Debug.LogWarning("SceneController not found!");
            return;
        }

        var currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "6_DemoFinal")
        {
            SceneController.Instance.LoadScene(SceneController.SceneName.TitleScene);
        }
        else if (currentScene == "3_AttorneyLounge" || currentScene == "4_DAOffice" || currentScene == "5_PublicRecords")
        {
            SceneController.Instance.LoadScene(SceneController.SceneName.CourtHouseScene);
        }
        else if (currentScene == "2_CourtHouse")
        {
            return;
        }
        else
        {
            SceneController.Instance.LoadNextScene();
        }
    }
}
