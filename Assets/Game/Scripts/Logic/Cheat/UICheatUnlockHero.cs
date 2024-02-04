using System;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine.UI;

public class UICheatUnlockHero : UI.Panel
{
    public TMP_Dropdown dropDown;
    public Button cheat;
    public List<EHero> ID = new List<EHero>();
    public override void PostInit()
    {
        
    }

    public override void Show()
    {
        base.Show();
        dropDown.ClearOptions();
        var list = new List<string>();
        for (var hero = EHero.NormalHero; hero <= EHero.EvilHero; hero++)
        {
            list.Add(hero.ToString());
            ID.Add(hero);
        }
        dropDown.AddOptions(list);
    }
    private void OnEnable()
    {
        cheat.onClick.AddListener(Cheat);
    }
    private void OnDisable()
    {
        cheat.onClick.RemoveListener(Cheat);
    }

    private async void Cheat()
    {
        var value = dropDown.value;
        var heroCurrent = ID[value];
        DataManager.Save.User.UnlockHero(heroCurrent);
        DataManager.Save.User.SetPickHero(heroCurrent);

        Messenger.Broadcast(EventKey.UpdateHeroItemUI, heroCurrent);
        Messenger.Broadcast(EventKey.PickHero, heroCurrent);

        var ui = await PanelManager.CreateAsync<UIChestRewardClaimHeroPanel>(AddressableName.UIChestRewardHeroPanel);
        ui.Show(heroCurrent);
    }
}
