using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UITryHeroCollectionsPanel : UI.Panel
{
    public UITryHeroPanel.OnPickHero onPickHero;

    [SerializeField] private RectTransform holder;
    private List<GameObject> listGO = new List<GameObject>();
    private TryHeroSaves dataSave;
    public override async void PostInit()
    {
        dataSave = DataManager.Save.TryHero;
        var prefab = (await ResourcesLoader.Instance.LoadAsync<GameObject>(AddressableName.UITryHeroCollectionItem)).GetComponent<UITryHeroElement>();
        foreach (var hero in dataSave.Saves)
        {
            var heroIns = PoolManager.Instance.Spawn(prefab, holder);
            heroIns.Init(hero.Value.Hero);

            heroIns.onClickedTryHero += OnTryHeroSelect;

            listGO.Add(heroIns.gameObject);
        }
    }

    private async UniTask OnTryHeroSelect(EHero eHero)
    {
        await UniTask.Delay(100);
        DataManager.Save.TryHero.SetTried(eHero);
        ApplyNewDataUser(eHero);
        onPickHero?.Invoke(eHero);
    }
    private void ApplyNewDataUser(EHero eHero)
    {
        var playerData = GameSceneManager.Instance.PlayerData;
        var userSave = DataManager.Save.User;
        userSave.SetTryHero(eHero);

        var currentHero = userSave.HeroCurrent;
        playerData.HeroCurrent = currentHero;
        playerData.Stats.ReplaceAllStatBySource(playerData.HeroDatas[currentHero].heroStat, EStatSource.sourceHero);
    }
    public override void Close()
    {
        foreach (GameObject go in listGO)
        {
            PoolManager.Instance.Despawn(go);
        }
        listGO.Clear();
        base.Close();
    }
}
