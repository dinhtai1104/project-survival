using Lean.Pool;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class UIHotSaleHeroPanel : UI.Panel
{
    [SerializeField] private RectTransform m_Content;
    [SerializeField] private ScrollRect m_ScrollRect;
    [SerializeField] private UIHotSaleHeroItem m_SalePrefab;
    [SerializeField] private GameObject m_Empty;

    private HotSaleHeroTable table;
    private HotSaleHeroSaves saves;
    private List<UIHotSaleHeroItem> m_Items = new List<UIHotSaleHeroItem>();

    public override void PostInit()
    {
    }
    [Button]
    public override void Show()
    {
        base.Show();
        table = DataManager.Base.HotSaleHero;
        saves = DataManager.Save.HotSaleHero;

        Setup();
    }
    public override void Clear()
    {
        base.Clear();
        foreach (var item in m_Items)
        {
            PoolManager.Instance.Despawn(item.gameObject);
        }
        m_Items.Clear();
    }

    private void Setup()
    {
        Clear();
        var all = saves.Saves.Values.ToList().FindAll(t=>t.IsActived).OrderBy(t => t.TimeEnd).ToList();
        m_Empty.SetActive(all.Count == 0);
        foreach (var model in all)
        {
            var save = model;
            if (save.IsActived == false) continue;

            var item = PoolManager.Instance.Spawn(m_SalePrefab, m_Content);
            item.Init(save.Hero, this);
            m_Items.Add(item);
        }
    }

    [Button]
    public override void Close()
    {
        m_Items.Clear();
        onClosed += ()=> LeanPool.CleanUpByPrefab(m_SalePrefab.gameObject);
        base.Close();
    }

    public void UpdateUI()
    {
        Setup();
    }
}
