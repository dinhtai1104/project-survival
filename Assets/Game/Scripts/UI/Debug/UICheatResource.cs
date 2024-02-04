using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine.UI;

public class UICheatResource : UI.Panel
{
    public TMP_Dropdown dropdown;
    public Text currentResource;
    public TMP_InputField inputResource;
    public Button cheat;
    public List<EResource> resources = new List<EResource>();
    public override void PostInit()
    {
        var list = new List<string>();
        int index = 0;
        foreach (var rs in (EResource[])Enum.GetValues(typeof(EResource)))
        {
            list.Add($"{index++}. {rs}");
            resources.Add(rs);
        }
        dropdown.ClearOptions();
        dropdown.AddOptions(list);
        dropdown.onValueChanged.AddListener(OnChangeValue);
        dropdown.value = 0;
        OnChangeValue(0);
        cheat.onClick.AddListener(AddResourceClick);
    }

    private void OnChangeValue(int value)
    {
        var type = resources[value];
        var data = DataManager.Save.Resources;
        currentResource.text = data.GetResource(type).ToString();
    }

    private void AddResourceClick()
    {
        try
        {
            var type = resources[dropdown.value];
            var value = int.Parse(inputResource.text);
            var data = DataManager.Save.Resources;
            data.IncreaseResource(new ResourceData { Resource = type, Value = value });
            currentResource.text = data.GetResource(type).ToString();
        }
        catch (Exception e)
        {
            PanelManager.ShowNotice(e.Message).Forget();
        }
    }
}
