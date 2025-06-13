using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueLine
{
    public string lineID;
    public string nextLineID;

    [TextArea(2, 5)]
    public string text;

    public DialogueSpeakerSO speaker;

    public bool showResponses;
    public List<ResponseOption> responseOptions;

    public bool showChargesheets;
    public List<ChargeSheetEntry> chargeSheets;
}

[Serializable]
public class ResponseOption
{
    public string text;
    public string jumpToID;
}
[System.Serializable]
public class ChargeSheetEntry
{
    public ChargeSheetSO sheet;
    public string jumpToID;
}