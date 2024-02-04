using System.Collections.Generic;
using UnityEngine;

public class UIBuffCollectionView : MonoBehaviour
{
    [SerializeField] private UIBuffItemBase itemPrefab;
    [SerializeField] private RectTransform contentParent;

    private List<UIBuffItemBase> listItems = new List<UIBuffItemBase>();
    public void Show(BuffCollectionData data)
    {
        foreach (var item in listItems)
        {
            PoolManager.Instance.Despawn(item.gameObject);
        }
        var buffTable = DataManager.Base.Buff.Dictionary;
        foreach (var item in data.BuffEquiped)
        {
            if (item.Key > EBuff.None && item.Key < EBuff.NormalHeroPassive)
            {
                var itemIns = PoolManager.Instance.Spawn(itemPrefab, contentParent);
                itemIns.SetEntity(buffTable[item.Key]);
                itemIns.SetInfor();
                itemIns.SetStarActive(item.Value.Level);
            }
        }
    }
}