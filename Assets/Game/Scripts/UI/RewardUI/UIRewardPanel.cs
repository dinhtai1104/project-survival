using Cysharp.Threading.Tasks;
using Lean.Pool;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIRewardPanel : UI.Panel
{
    [SerializeField] private TextMeshProUGUI m_Title;
    [SerializeField] private UILootCollectionView lootCollectionView;
    [SerializeField] private GameObject tapToContinue;
    private bool _canClosePanel;

    public List<UIInventorySlot> InventorySlot => lootCollectionView.GetLootItems();
    public override void PostInit()
    {
        tapToContinue.SetActive(false);
        SetTitle(I2Localize.GetLocalize("Common/Title_Common_Reward"));
    }

    public async void Show(List<LootParams> lootData)
    {
        _canClosePanel = true;
        base.Show();
        lootCollectionView.Clear();
        await lootCollectionView.Show(new LootCollectionData { lootData = lootData }, OnSpawn);
        //_canClosePanel = true;
        tapToContinue.SetActive(true);
    }

    private async void OnSpawn(UIInventorySlot slot)
    {
        var pathEff = "Eff/VFX_Reward_Appear.prefab";
        var ef = await ResourcesLoader.Instance.GetGOAsync(pathEff, slot.transform);
        ef.transform.localPosition = Vector3.zero;
        var part = ef.transform.GetChild(0).GetComponent<ParticleSystem>();
        var colorS = part.main;
        colorS.startColor = new ParticleSystem.MinMaxGradient
        {
            color = RarityExtension.GetColor(slot.Rarity),
            colorMin = RarityExtension.GetColor(slot.Rarity),
            colorMax = RarityExtension.GetColor(slot.Rarity),
        };

        ef.GetComponent<ParticleSystem>().Play();
        LeanPool.Detach(ef.gameObject, true);
        Destroy(ef.gameObject, 1f);
    }

    public override void Close()
    {
        if (_canClosePanel)
        {
            base.Close();
        }
    }

    public void SetTitle(string title)
    {
        m_Title.text = title;
    }
}