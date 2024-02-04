using Assets.Game.Scripts.BaseFramework.Architecture;
using BansheeGz.BGDatabase;
using Cysharp.Threading.Tasks;
using System;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuCheat : UICheat
{
    public UIButtonCheat buttonCheat;
    public Transform holder;
    public GameObject uiCheatTryHero;
    public GameObject uiCheatResource;
    public GameObject uiSetLanguage;
    public GameObject uiUnlockHero;
    public GameObject uiCheatUpdateCSV;
    protected override void Init()
    {
        base.Init();
        AddButton("Upadte CSV", OnUpdateCSV);
        AddButton("Set Try Hero", OnSetTryHero);
        AddButton("Cheat Resource", OnCheatResource);
        AddButton("Set Language", OnSetLanguage);
        AddButton("Cheat Exp(200)", OnCheatExp);
        AddButton("Unlock Hero", OnUnlockHero);
        AddButton("Reset Try Hero", OnResetTryHero);
        AddButton("Clear Data", OnClearData);
        AddButton("Unlock Dungeon", OnUnlockDungeon);
    }

    private void OnUnlockDungeon()
    {
        DataManager.Save.Dungeon.ClearNextDungeon();
        UIMainMenuPanel.Instance.TestNewDungeon();
    }

    private async void OnClearData()
    {
        var ui = await PanelManager.ShowNotice("Clear Data");
        ui.SetConfirmCallback(() =>
        {
            DataManager.Save.ClearData();
            SceneManager.LoadSceneAsync("InitScene");
        });
    }

    private void OnResetTryHero()
    {
        DataManager.Save.TryHero.IsTried = false;
        DataManager.Save.TryHero.Save();
        PanelManager.ShowNotice("Clear Try Hero").Forget();
    }

    private void OnUnlockHero()
    {
        var ui = UI.PanelManager.Create<UI.Panel>(uiUnlockHero);
        ui.Show();
    }

    private void OnCheatExp()
    {
        DataManager.Save.User.AddExp(200);
        Architecture.Get<BattlePassService>().AddExp(200);
    }

    private void OnSetLanguage()
    {
        var ui = UI.PanelManager.Create<UI.Panel>(uiSetLanguage);
        ui.Show();
    }

    private void OnCheatResource()
    {
        var ui = UI.PanelManager.Create<UI.Panel>(uiCheatResource);
        ui.Show();
    }

    private void OnSetTryHero()
    {
        var ui = UI.PanelManager.Create<UI.Panel>(uiCheatTryHero);
        ui.Show();
    }

    private void OnUpdateCSV()
    {
        var ui = UI.PanelManager.Create<UI.Panel>(uiCheatUpdateCSV);
        ui.Show();
    }

    private void LoadCompleted()
    {
        DataManager.Instance.ReloadDatabase();
        BansheeGz.BGDatabase.BGRepo.I.Addons.Get<BGAddonLiveUpdate>().OnLoadComplete -= LoadCompleted;
    }

    protected void AddButton(string title, System.Action callback)
    {
        var btn = PoolManager.Instance.Spawn(buttonCheat, holder);
        btn.SetCheat(title, callback);
    }
}