using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryHeroesView : MonoBehaviour
{
    [SerializeField] private RectTransform holder;
    private List<UIInventoryHeroItem> listHeroes = new List<UIInventoryHeroItem>();
    private HeroTable heroTable => DataManager.Base.Hero;

    public async void Show(UIInventoryHeroItem.HeroClicked onClick = null)
    {
        var prefab = (await ResourcesLoader.Instance.LoadAsync<GameObject>(AddressableName.UIInventoryHeroItem)).GetComponent<UIInventoryHeroItem>();
        var userSave = DataManager.Save.User;
        foreach (var heroEntity in heroTable.Dictionary.Values)
        {
            var ins = PoolManager.Instance.Spawn(prefab, holder);
            ins.Set(heroEntity, heroEntity.TypeHero == DataManager.Save.User.Hero);
            if (onClick != null)
            {
                ins.heroClicked += onClick;
            }
            listHeroes.Add(ins);
        }

        Messenger.Broadcast(EventKey.PickHero, userSave.Hero);
    }

    private void OnEnable()
    {
        //Messenger.AddListener<EHero>(EventKey.PickHero, Pick);
    }
    private void OnDisable()
    {
        //Messenger.RemoveListener<EHero>(EventKey.PickHero, Pick);
    }

    public void Pick(EHero hero)
    {
        foreach (var item in listHeroes)
        {
            if (item.HeroType == hero)
            {
                item.Pick();
            }
            else
            {
                item.UnPick();
            }
        }
    }

    public void Close()
    {
        foreach (var item in listHeroes)
        {
            PoolManager.Instance.Despawn(item.gameObject);
        }
        listHeroes.Clear();
    }
}