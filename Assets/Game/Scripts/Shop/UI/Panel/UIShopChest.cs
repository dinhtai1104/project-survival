using System.Collections.Generic;
using UnityEngine;

public class UIShopChest : UIShopItem
{
    private ChestTable chestTable;
    [SerializeField] private List<UIChestBase> chests;
    public override void OnInit()
    {
        chestTable = DataManager.Base.Chest;
        base.OnInit();
        Setup();
    }

    private void Setup()
    {
        foreach (var c in chests)
        {
            c.Init();
        }
    }
}