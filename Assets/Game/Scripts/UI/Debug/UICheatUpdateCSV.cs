using BansheeGz.BGDatabase;
using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine.UI;
using static Game.SDK.SDKIdConfig;

public class UICheatUpdateCSV : UI.Panel
{
    public TMP_InputField inputField;
    public Button updateDBDefault;
    public Button updateDBCustom;
    public override void PostInit()
    {
    }
    private void OnEnable()
    {
        updateDBCustom.onClick.AddListener(CustomUpdate);
        updateDBDefault.onClick.AddListener(DefaultUpdate);
    }
    private void OnDisable()
    {
        updateDBCustom.onClick.RemoveListener(CustomUpdate);
        updateDBDefault.onClick.RemoveListener(DefaultUpdate);
    }

    private async void CustomUpdate()
    {
        var id = inputField.text;

        await Game.Controller.Instance.LoadMenuScene();
        await UniTask.Delay(500);
        BansheeGz.BGDatabase.BGRepo.I.Addons.Get<BGAddonLiveUpdate>().OnLoadComplete += LoadCompleted;
        BansheeGz.BGDatabase.BGRepo.I.Addons.Get<BGAddonLiveUpdate>().SpreadsheetId = id;
        BansheeGz.BGDatabase.BGRepo.I.Addons.Get<BGAddonLiveUpdate>().Load();
    }

    private void LoadCompleted()
    {
        DataManager.Instance.ReloadDatabase();
        BansheeGz.BGDatabase.BGRepo.I.Addons.Get<BGAddonLiveUpdate>().OnLoadComplete -= LoadCompleted;
    }

    private async void DefaultUpdate()
    {
        await Game.Controller.Instance.LoadMenuScene();
        await UniTask.Delay(500);
        BansheeGz.BGDatabase.BGRepo.I.Addons.Get<BGAddonLiveUpdate>().OnLoadComplete += LoadCompleted;
        BansheeGz.BGDatabase.BGRepo.I.Addons.Get<BGAddonLiveUpdate>().SpreadsheetId = "1lWK5-XfFEg7tlGl8-0dPCe6kHReEdnkbUDAZ8fBPet4";
        BansheeGz.BGDatabase.BGRepo.I.Addons.Get<BGAddonLiveUpdate>().Load();
    }
}
