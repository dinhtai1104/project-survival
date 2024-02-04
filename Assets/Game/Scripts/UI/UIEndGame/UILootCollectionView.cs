using com.mec;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class UILootCollectionView : MonoBehaviour
{
    [SerializeField] private RectTransform lootParentHolder;
    private List<UIInventorySlot> lootItems = new List<UIInventorySlot>();
    public AnimationCurve scaleCurve;
    private CancellationTokenSource cancellationToken;

    public void Clear(bool despawn = false)
    {
        if (despawn)
        {
            foreach (var item in lootItems)
            {
                if (item != null)
                {
                    item.transform.localScale = Vector3.one;
                    item.Clear();
                    PoolManager.Instance.Despawn(item.gameObject);
                }
            }
        }
        lootItems.Clear();
    }

    public List<UIInventorySlot> GetLootItems() => lootItems;
    public async UniTask Show(LootCollectionData data, System.Action<UIInventorySlot> onSpawn = null)
    {
        Clear();
        cancellationToken = new CancellationTokenSource();
        var listTask = new List<UniTask<UIInventorySlot>>();

        for (int i = 0; i < data.lootData.Count; i++)
        {
            var lootData = data.lootData[i];
            var typeLoot = lootData.Type;

            var path = string.Format(AddressableName.UILootItemPath, typeLoot);
            if (typeLoot == ELootType.Equipment)
            {
                path = AddressableName.UIGeneralEquipmentItem;
            }
            var item = await UIHelper.GetUILootItem(path, lootData.Data, lootParentHolder);
            onSpawn?.Invoke(item);
            lootItems.Add(item);
            item.transform.localScale = Vector3.one;
        }

        SetToZeroPos();
        Timing.RunCoroutine(_DOAnim(), gameObject);
    }

    public async UniTask Show(LootCollectionData data, UIInventorySlot prefab, System.Action<UIInventorySlot> onSpawn = null)
    {
        Clear();
        cancellationToken = new CancellationTokenSource();
        var listTask = new List<UniTask<UIInventorySlot>>();

        for (int i = 0; i < data.lootData.Count; i++)
        {
            var lootData = data.lootData[i];
            var typeLoot = lootData.Type;

            var path = string.Format(AddressableName.UILootItemPath, typeLoot);
            if (typeLoot == ELootType.Equipment)
            {
                path = AddressableName.UIGeneralEquipmentItem;
            }
            var item = PoolManager.Instance.Spawn(prefab, lootParentHolder);
            var dataIcon = await UIHelper.GetUILootIcon(path, lootData.Data, item.transform);
            item.SetItem(dataIcon);
            if (typeLoot == ELootType.Equipment)
            {
                var stack = (int)lootData.Data.ValueLoot;
                item.SetStack(stack);
            }
            lootItems.Add(item);
            item.transform.localScale = Vector3.one;
            onSpawn?.Invoke(item);
        }
    }

    public void SetToZeroPos()
    {
        GetComponentInChildren<ScrollRect>().normalizedPosition = Vector3.zero;
    }

    private void OnDisable()
    {
        cancellationToken?.Cancel();
        cancellationToken?.Dispose();
        Clear();
        Timing.KillCoroutines(gameObject);
    }
    private IEnumerator<float> _DOAnim()
    {
        foreach (var item in lootItems)
        {
            item.gameObject.SetActive(true);
            item.transform.DOScale(Vector3.one, 0.15f).From(Vector3.one * 0.7f).SetEase(Ease.OutBack).SetId(item.gameObject);
            yield return Timing.WaitForSeconds(0.05f);
        }
    }
#if DEVELOPMENT 
    [Header("Test")]
    public List<string> LootParamsTesting = new List<string>();
    [Button]
    public void TestLoot()
    {
        var lootCollection = new LootCollectionData();
        foreach (var data in LootParamsTesting)
        {
            var loot = new LootParams(data);
            lootCollection.lootData.Add(loot);
        } 
        Show(lootCollection);
    }
#endif

}
public class LootCollectionData
{
    public List<LootParams> lootData;
    public LootCollectionData()
    {
        lootData = new List<LootParams>();
    }
    public LootCollectionData(List<LootParams> lootData)
    {
        this.lootData = lootData;
    }
}