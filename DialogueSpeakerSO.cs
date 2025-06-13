using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Speaker")]
public class DialogueSpeakerSO : ScriptableObject
{
    public string speakerName;
    public Sprite portrait;
    public string speakerRole; // "Player" or "NPC"
}

