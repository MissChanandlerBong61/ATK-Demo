using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class NPController : MonoBehaviour
{
    public static NPController activeNPC = null;
        
    public TextMeshPro collusionText;     

    [Header("Dialogue Settings")]
    public List<DialogueLine> dialogueLines;
    public Sprite NPCprofpic;
    public Sprite playerSprite;        

    
    private bool playerIsClose = false;

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGamePaused && activeNPC != this)
            return;

        if (playerIsClose && Input.GetKeyDown(KeyCode.E))
        {
            if (activeNPC != null && activeNPC != this)
            {
                activeNPC = null;
            }
            activeNPC = this;

            if (gameObject.CompareTag("Criminal"))
            {
                PickCriminal();
            }
            else if (gameObject.CompareTag("NPC"))
            {
                HandleDialogueToggle();
            }
        }
    }    

    void HandleDialogueToggle()
    {
        if (UIController.Instance.IsDialogueOpen())
        {
            UIController.Instance.CloseDialogue();
            if (collusionText != null) collusionText.gameObject.SetActive(false);
            activeNPC = null;
        }
        else
        {
            if (dialogueLines == null || dialogueLines.Count == 0)
            {
                Debug.LogWarning($"NPC {gameObject.name} has no dialogue lines assigned. Dialogue will not start.");
                activeNPC = null;
                return;
            } 

            UIController.Instance.ShowDialogue(dialogueLines);

            if (UIController.Instance.IsDialogueOpen())
            {
                if (collusionText != null) collusionText.gameObject.SetActive(false);
            }

            else
            {
                activeNPC = null;
            }                       
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsClose = true;
            if (collusionText != null) collusionText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsClose = false;
            if (collusionText != null) collusionText.gameObject.SetActive(false);
            activeNPC = null;
        }
    }
    
    public void PickCriminal()
    {
        Debug.Log($"Picking criminal: {gameObject.name}");  
    }
}
