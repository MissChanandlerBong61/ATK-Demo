using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LoadingUI : MonoBehaviour
{
    public GameObject loadingPanel;
    public Slider progressBar;
    public TextMeshProUGUI loadingText;

    public void Show()
    {
        loadingPanel.SetActive(true);
    }

    public void Hide()
    {
        loadingPanel.SetActive(false);
    }

    public void SetProgress(float progress)
    {
        if (progressBar != null)
        {
            progressBar.value = progress;
        }
        if (loadingText != null)
        {
            loadingText.text = "Loading... " + Mathf.RoundToInt(progress * 100f) + "%";
        }
    }
    public void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGamePaused) return;
    }

}
