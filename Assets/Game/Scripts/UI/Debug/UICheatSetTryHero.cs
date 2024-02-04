using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICheatSetTryHero : UI.Panel
{
    public TMP_Dropdown dropDown;
    public Button buttonCheat;
    protected void Init()
    {
        var allHero = DataManager.Base.Hero.Dictionary;
        var data = new List<string>();
        int index = 0;
        foreach ( var hero in allHero )
        {
            data.Add($"{index++}. {hero.Key}");
        }
        dropDown.ClearOptions();
        dropDown.AddOptions(data);
        buttonCheat.onClick.AddListener(ClickTryHero);
    }

    public void ClickTryHero()
    {
        var index = dropDown.value;
        var allHero = DataManager.Base.Hero.Dictionary;
        var hero = (EHero)index;
        var trySave = DataManager.Save.TryHero;
        trySave.SetTried(hero);
        ApplyNewDataUser(hero);
    }
    private void ApplyNewDataUser(EHero eHero)
    {
        var playerData = GameSceneManager.Instance.PlayerData;
        var userSave = DataManager.Save.User;
        userSave.SetTryHero(eHero);
        //   playerData.Stats.RemoveModifiersFromSource("Hero");

        var currentHero = userSave.HeroCurrent;
        playerData.HeroCurrent = currentHero;
        playerData.Stats.ReplaceAllStatBySource(playerData.HeroDatas[currentHero].heroStat, EStatSource.sourceHero);
    }
    public override void PostInit()
    {
        Init();
    }
}