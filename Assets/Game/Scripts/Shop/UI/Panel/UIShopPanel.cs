using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class UIShopPanel : UI.Panel
{
    [SerializeField] private List<UIShopItem> shopItems;
    public override void PostInit()
    {
    }
    public override void Show()
    {
        base.Show();
        shopItems.ForEach(item => item.OnInit());
    }
}