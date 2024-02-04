using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageStartPanel : UI.Panel
{
    [SerializeField]
    private TMPro.TextMeshProUGUI stageText;
    public override void PostInit()
    {
    }

   public void SetUp(int stage)
    {
        stageText.text = I2Localize.GetLocalize("Common/Title_Stage", stage);
        Show();
    }
}
