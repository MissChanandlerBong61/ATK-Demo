using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NUnit.Framework.Constraints;

public class WardrobeFunction : MonoBehaviour
{
    public GameObject customizationMenu;
    public GameObject wardrobeText;
    private bool isNearWardrobe;

    public Color tieColor;
    public Image tieType;
    public Image latestTieColor;
    public Sprite[] availableSprites;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadTieColor();
        LoadTieType();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (isNearWardrobe && Input.GetKeyDown(KeyCode.E))
        {
            ToggleCustomizationMenu();  
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isNearWardrobe = true;
            wardrobeText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isNearWardrobe = false;
            wardrobeText.SetActive(false);
        }
    }

    public void ToggleCustomizationMenu()
    {      

        bool isActive = customizationMenu.activeSelf;
        
        customizationMenu.SetActive(!isActive);

        if (!isActive)
        {
            GameManager.Instance.PauseGame(PauseSource.UI);
            wardrobeText.SetActive(false);
            

        }
        else
        {
            GameManager.Instance.ResumeGame();
            wardrobeText.SetActive(true);
        }


    }

    public void SetTieColorFromButton(Button tieColorButton)
    {
        if (tieColorButton == null) return;

        Color pickedColor = tieColorButton.GetComponent<Image>().color;
        tieColor = pickedColor;

        if (latestTieColor != null)
        {
            latestTieColor.color = tieColor;
        }

        PlayerPrefs.SetFloat("TieColor_R", pickedColor.r);
        PlayerPrefs.SetFloat("TieColor_G", pickedColor.g);
        PlayerPrefs.SetFloat("TieColor_B", pickedColor.b);
        PlayerPrefs.SetFloat("TieColor_A", pickedColor.a);

        PlayerPrefs.Save();
    }

    public void LoadTieColor()
    {
        float r = PlayerPrefs.GetFloat("TieColor_R", 1f); 
        float g = PlayerPrefs.GetFloat("TieColor_G", 1f);
        float b = PlayerPrefs.GetFloat("TieColor_B", 1f);
        float a = PlayerPrefs.GetFloat("TieColor_A", 1f);

        tieColor = new Color(r, g, b, a);

        if (latestTieColor != null) 
        { 
            latestTieColor.color = tieColor; 
        }
    }
     

    public void SetTieTypeFromButton(Sprite selectedSprite)
    {
        if (selectedSprite == null) return;
                
        tieType.sprite = selectedSprite;

        PlayerPrefs.SetString("SelectedSprite", selectedSprite.name);
        PlayerPrefs.Save();
    }

    public void LoadTieType()
    {
        string savedSpriteName = PlayerPrefs.GetString("SelectedSprite", "");

        if (!string.IsNullOrEmpty(savedSpriteName))
        {
            foreach (Sprite sprite in availableSprites)
            {
                if (sprite.name == savedSpriteName)
                {
                    tieType.sprite = sprite;
                }
            }
        }
    }

}


