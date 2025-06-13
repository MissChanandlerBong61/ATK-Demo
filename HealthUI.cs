using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[System.Serializable]
public class HealthUI
{
    public VisualElement healthBar;

    public void Initialize(VisualElement root)
    {
        healthBar = root.Q<VisualElement>("HealthBarBackGround");
        if (healthBar == null)
        {
            Debug.LogError("HealthBar not found in the UI Document.");
            return;
        }
        if (SceneManager.GetActiveScene().name == "0_TitleScene")
        {
            healthBar.style.display = DisplayStyle.None;
        }
        else
        {
            healthBar.style.display = DisplayStyle.Flex;
        }
    }
    public void SetHealthValue(float percantage)
    {
        if (healthBar == null)
        {
            Debug.LogError("HealthBar is not initialized.");
            return;
        }

        healthBar.style.width = new StyleLength(new Length(percantage, LengthUnit.Percent));
    }
}
