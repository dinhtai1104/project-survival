using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIMessagePanel : UI.Panel
{
    [SerializeField] private TextMeshProUGUI description;
    public override void PostInit()
    {
    }

    public void SetUp(string text)
    {
        description.SetText(text);
        base.Show();
    }
}