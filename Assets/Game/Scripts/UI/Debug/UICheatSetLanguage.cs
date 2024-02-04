using System.Collections.Generic;
using System;
using TMPro;
using UI;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class UICheatSetLanguage : UI.Panel
{
    public TMP_Dropdown dropdown;
    public Button cheat;
    public override void PostInit()
    {
        var list = new List<string>();
        int index = 0;
        foreach (var rs in I2.Loc.LocalizationManager.GetAllLanguages())
        {
            list.Add($"{index++}. {rs}");
        }
        dropdown.ClearOptions();
        dropdown.AddOptions(list);
        cheat.onClick.AddListener(AddResourceClick);
    }

    private void AddResourceClick()
    {
        try
        {
            var type = dropdown.value;
            var language = I2.Loc.LocalizationManager.GetAllLanguages()[type];
            var code = I2.Loc.LocalizationManager.GetLanguageCode(language);
            I2.Loc.LocalizationManager.SetLanguageAndCode(language, code);
            Debug.Log("Update language: " + language);
            GameSceneManager.Instance.UpdateTmpFontAsset(language);
            Close();
        }
        catch (Exception e)
        {
            PanelManager.ShowNotice(e.Message).Forget();
        }
    }
}