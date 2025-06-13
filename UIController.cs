using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public enum Speaker
{
    NPC,
    Player
}


public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    [Header("UIDocument")]
    public UIDocument uiDocument;

    [Header("Modules")]
    [SerializeField] private HealthUI healthUI = new HealthUI();
    [SerializeField] private ChargeSheetUI chargeSheetUI;

    private Dictionary<string, DialogueLine> dialogueMap;
    private DialogueLine currentLine;

    private VisualElement pauseMenuUI;

    //Dialogue Elements
    private VisualElement dialogWindow;
    private Label dialogText;
    private Label continueText;
    private Label npcName;
    private VisualElement npcProfPic;
    private VisualElement playerProfPic;
    private Label leftNameLabel;
    private Label rightNameLabel;
    private VisualElement responseContainer;    
    
    private Coroutine typingCoroutine;
    private bool skipRequested;
    private bool isTyping;
    

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        Instance = this;
    }

    void Start()
    {
        VisualElement root = uiDocument.rootVisualElement;
        chargeSheetUI.Initialize(root);

        pauseMenuUI = root.Q<VisualElement>("PauseMenu");
        if (pauseMenuUI == null)
        {
            Debug.LogError("PauseMenu not found in UI Document.");
        }
        dialogWindow = root.Q<VisualElement>("DialogWindow");
        playerProfPic = root.Q<VisualElement>("PlayerProfPic");
        leftNameLabel = root.Q<Label>("LeftNameLabel");
        rightNameLabel = root.Q<Label>("RightNameLabel"); responseContainer = root.Q<VisualElement>("ResponseContainer");
        
        if (responseContainer == null) Debug.LogWarning("ResponseContainer not found");
        if (playerProfPic == null) Debug.LogWarning("PlayerProfPic not found");
        if (leftNameLabel == null) Debug.LogWarning("LeftNameLabel not found");
        if (rightNameLabel == null) Debug.LogWarning("RightNameLabel not found");
        Debug.Log(dialogWindow == null ? "DialogWindow is NULL" : "DialogWindow is NOT null");
        dialogText = root.Q<Label>("DialogueText");
        npcName = root.Q<Label>("NPCName");
        npcProfPic = root.Q<VisualElement>("NPCProfPic");
        continueText = root.Q<Label>("ContinueText");

        dialogWindow.style.display = DisplayStyle.None;        

        var resumeButton = root.Q<Button>("ResumeButton");
        if (resumeButton != null)
        {
            resumeButton.clicked += () => {
                GameManager.Instance?.ResumeGame();
                TogglePauseMenu(false);
            };
        }

        var mainMenuButton = root.Q<Button>("ExitButton");
        if (mainMenuButton == null)
        {
            Debug.LogError("MainMenuButton not found in the UI Document.");
            return;
        }
        else
        {
            mainMenuButton.clicked += () =>
            {
                GameManager.Instance.ForceUnpause();
                SceneController.Instance.LoadScene(SceneController.SceneName.TitleScene);
            };
        }
        healthUI.Initialize(root);
    }
    public void ShowDialogue(List<DialogueLine> lines)
    {
        GameManager.Instance?.PauseGame(PauseSource.NPC);
        dialogWindow.style.display = DisplayStyle.Flex;

        dialogueMap = new Dictionary<string, DialogueLine>();

        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line.lineID))
            {
                Debug.LogWarning("Line ID is empty or null. Skipping.");
                continue;
            }
            else
            {
                dialogueMap[line.lineID] = line;
            }
        }

        currentLine = lines[0];  // start from first line
        ApplyLine(currentLine.lineID);
    }

    private void ApplySpeaker (DialogueLine line)
    {
        if (leftNameLabel != null) leftNameLabel.text = "";
        if (rightNameLabel != null) rightNameLabel.text = "";
        if (npcProfPic != null) npcProfPic.Clear();
        if (playerProfPic != null) playerProfPic.Clear();


        if (line.speaker == null)
        {
            Debug.LogWarning("No speaker assigned to line: " + line.lineID);
            return;
        }

        var speaker = line.speaker;

        // Name
        if (speaker.speakerRole == "Player")
            leftNameLabel.text = speaker.speakerName;
        else
            rightNameLabel.text = speaker.speakerName;

        // Portrait
        if (speaker.portrait != null)
        {
            Image img = new Image()
            {
                image = speaker.portrait.texture,
                scaleMode = ScaleMode.ScaleToFit
            };            
            
            img.style.flexGrow = 1;

            if (speaker.speakerRole == "Player")
                playerProfPic.Add(img);
            else
                npcProfPic.Add(img);
        }
        Debug.Log($"Set {(speaker.speakerRole == "Player" ? "Left" : "Right")}NameLabel to: {speaker.speakerName}");
    }
    private void ApplyLine(string lineID)
    {
        if (!dialogueMap.ContainsKey(lineID))
        {
            Debug.LogWarning($"Dialogue line ID not found: {lineID}");
            CloseDialogue();
            return;
        }

        currentLine = dialogueMap[lineID];
        ApplySpeaker(currentLine);
        StartTyping(currentLine);

        

        if (currentLine.showResponses && currentLine.responseOptions != null && currentLine.responseOptions.Count > 0)
        {
                ShowResponses(currentLine.responseOptions);
                continueText.SetEnabled(false);
        }
        else
        {
                responseContainer.style.display = DisplayStyle.None;
                continueText.SetEnabled(true);
        }
    }

    private void StartTyping(DialogueLine line)
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeLine(line.text));
    }


    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        skipRequested = false;
        dialogText.text = "";

        for (int i = 0; i < line.Length; i++)
        {
            if (skipRequested)
            {
                dialogText.text = line;
                break;
            }

            dialogText.text += line[i];
            yield return new WaitForSecondsRealtime(0.02f);
        }

        isTyping = false;
        if (currentLine.showChargesheets && currentLine.chargeSheets != null)
        {
            // Wait one frame just to make sure
            yield return null;

            dialogWindow.style.display = DisplayStyle.None;
            chargeSheetUI.ShowCharges(currentLine.chargeSheets);
            yield break;
        }
        continueText.style.display = DisplayStyle.Flex;
    }

    private void Continue()
    {
        if (isTyping)
        {
            skipRequested = true;
            return;
        }
        if (!string.IsNullOrEmpty(currentLine.nextLineID) && dialogueMap.TryGetValue(currentLine.nextLineID, out var next))
        {
            ApplyLine(next.lineID);
        }
        else
        {
            CloseDialogue();
        }
    }
    public void CloseDialogue()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        dialogWindow.style.display = DisplayStyle.None;
        responseContainer.style.display = DisplayStyle.None;
        GameManager.Instance?.ResumeGame();
    }

    public bool IsDialogueOpen() => dialogWindow.style.display == DisplayStyle.Flex;


    public void SetHealth (float saglýk)
    {
        healthUI.SetHealthValue(saglýk);
    }

    public void TogglePauseMenu(bool state)
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.style.display = state ? DisplayStyle.Flex : DisplayStyle.None;
            if (healthUI.healthBar != null)
            {
                healthUI.healthBar.style.display = state ? DisplayStyle.None : DisplayStyle.Flex;
            }
        }
        else
        {
            Debug.LogError("PauseMenuUI is not initialized.");
        }
    }
    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGamePaused)
        {
            TogglePauseMenu(GameManager.Instance.CurrentPauseSource == PauseSource.Player);
        }
        else
        {
            TogglePauseMenu(false);
        }

        if (IsDialogueOpen() && Input.GetKeyDown(KeyCode.Q))
        {
            if (isTyping)
                skipRequested = true;
            else
                Continue();
        }
    }
    public void ShowResponses(List<ResponseOption> options)
    {
        responseContainer.Clear();
        responseContainer.style.display = DisplayStyle.Flex;

        foreach (var option in options)
        {
             Debug.Log("Creating button for: " + option);
            Button btn = new Button(() => OnResponseSelected(option.text))
            {
                text = option.text
            };
            btn.style.flexGrow = 1;
            responseContainer.Add(btn);
        }
    }
    private void OnResponseSelected(string selectedOption)
    {
        Debug.Log("Player selected: " + selectedOption);
        responseContainer.style.display = DisplayStyle.None;
        continueText.style.display = DisplayStyle.Flex;
       
        foreach (var response in currentLine.responseOptions)
        {
            if (response.text == selectedOption)
            {
                ApplyLine(response.jumpToID);
                return;
            }
        }

        Debug.LogWarning($"No matching response found for: {selectedOption}");
    }

    public bool IsChargesheetOpen()
    {
        return chargeSheetUI.IsOpen();
    }
    public void OnChargePicked(string jumpToID)
    {
        Debug.Log($"Picked ChargeSheet, jumping to: {jumpToID}");

        // Diyalog UI tekrar açýlsýn
        dialogWindow.style.display = DisplayStyle.Flex;

        if (!string.IsNullOrEmpty(jumpToID) && dialogueMap.TryGetValue(jumpToID, out var next))
        {
            ApplyLine(next.lineID);
        }
        else
        {
            Debug.LogWarning($"JumpToID '{jumpToID}' not found in dialogueMap");
            CloseDialogue();
        }
    }
}


