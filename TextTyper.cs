using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public static class TextTyper
{
    public static IEnumerator TypeText(TextMeshProUGUI textTarget, string line, float delay)
    {
        textTarget.text = "";
        foreach (char c in line)
        {
            textTarget.text += c;
            yield return new WaitForSecondsRealtime(delay);
        }
        textTarget.text = line;
    }

    public static IEnumerator TypeText(Label labelTarget, string line, float delay)
    {
        labelTarget.text = "";
        foreach (char c in line)
        {
            labelTarget.text += c;
            yield return new WaitForSecondsRealtime(delay);
        }
        labelTarget.text = line;
    }
}
