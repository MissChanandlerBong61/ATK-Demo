using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Charge Sheet")]
public class ChargeSheetSO : ScriptableObject
{
    public float attorneyFee;
    public string accusedName;
    public Sprite profilePicture;
    public int clientDifficulty;
    public Sprite difficultySymbol;
    [TextArea] public string[] charges;
    [TextArea] public string explanation;

}
