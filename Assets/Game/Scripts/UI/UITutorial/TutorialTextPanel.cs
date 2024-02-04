using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTextPanel : UI.Panel
{
    [SerializeField]
    private TMPro.TextMeshProUGUI text;
    public override void PostInit()
    {
    }
    public void SetUp(string text)
    {
        this.text.SetText(text);

        Show();
    }
}
