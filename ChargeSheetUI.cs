using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;

public class ChargeSheetUI : MonoBehaviour
{
    [SerializeField] private VisualTreeAsset cardTemplate;

    private VisualElement chargePanel;
    private List<VisualElement> predefinedCards = new List<VisualElement>();

    public void Initialize(VisualElement root)
    {
        chargePanel = root.Q<VisualElement>("ChargeSheetPanel");
    
        chargePanel.style.display = DisplayStyle.None;

        for (int i = 0; i < 3; i++)
        {
            var card = chargePanel.Q<VisualElement>($"ChargeCardRoot{i}");
            if (card != null)
            {
                card.style.display = DisplayStyle.None;
                predefinedCards.Add(card);
            }
            else
            {
                Debug.LogWarning($"ChargeCardRoot{i} not found in ChargeSheetPanel.");
            }
        }
    }

    public void ShowCharges(List<ChargeSheetEntry> entries)
    {
        GameManager.Instance?.PauseGame(PauseSource.UI);
        chargePanel.style.display = DisplayStyle.Flex;

        for (int i = 0; i < predefinedCards.Count; i++)
        {
            if (i < entries.Count)
            {
                var card = predefinedCards[i];
                card.style.display = DisplayStyle.Flex;
                SetupCard(card, entries[i]);
            }
            else
            {
                predefinedCards[i].style.display = DisplayStyle.None;
            }
        }
    }

    private void SetupCard(VisualElement card, ChargeSheetEntry entry)
    {
        var sheet = entry.sheet;

        var nameLabel = card.Q<Label>("AccusedName");
        var chargeListLabel = card.Q<Label>("ChargeList");
        var chargeListExp = card.Q<Label>("ChargeExp");
        var feeAmount = card.Q<Label>("FeeSignLabel");
        var profilePic = card.Q<VisualElement>("ProfilePic");
        var symbolImageContainer = card.Q<VisualElement>("DollarImage");
        var pickBtn = card.Q<Button>("PickingButton");

        nameLabel.text = sheet.accusedName;

        // Format charges
        chargeListLabel.text = "";
        foreach (string charge in sheet.charges)
            chargeListLabel.text += "• " + charge + "\n\n";

        chargeListExp.text = sheet.explanation;

        // Fee formatting 
        feeAmount.text = $"Fee: " + "\n" + sheet.attorneyFee + "$";

        // Profile sprite
        profilePic.Clear();
        if (sheet.profilePicture != null)
        {
            var img = new Image();
            img.image = sheet.profilePicture.texture;
            profilePic.Add(img);
        }

        symbolImageContainer.Clear();
        for (int i = 0; i < sheet.clientDifficulty; i++)
        {
            if (sheet.difficultySymbol != null)
            {
                var icon = new Image();
                icon.image = sheet.difficultySymbol.texture;
                icon.style.width = 50;
                icon.style.height = 50;
                icon.style.marginRight = 2;
                symbolImageContainer.Add(icon);
            }
            else
            {
                var fallback = new Label { text = "$" }; // fallback in case symbol is missing
                fallback.style.marginRight = 2;
                symbolImageContainer.Add(fallback);
            }
        }

        if (pickBtn != null)
        {
            pickBtn.Clear();
            pickBtn.clicked += () =>
            {                
                HideCharges();
                UIController.Instance.OnChargePicked(entry.jumpToID);

                if (NPController.activeNPC != null)
                    PickClient();
            };
        }
    }

    public void HideCharges()
    {
        chargePanel.style.display = DisplayStyle.None;
        foreach (var card in predefinedCards)
        {
            card.style.display = DisplayStyle.None;             
        }

        GameManager.Instance?.ResumeGame();
    }

    public void PickClient()
    {
        PlayerPrefs.SetString("ClientName", chargePanel.Q<Label>("AccusedName").text);
       
    }
    public bool IsOpen()
    {
        return chargePanel != null && chargePanel.style.display == DisplayStyle.Flex;
    }
}