using Assets.Game.Scripts.Utilities;
using com.mec;
using System;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class UIHotSaleHeroItem : MonoBehaviour
{
    [SerializeField] private Image m_BgImage;
    [SerializeField] private TextMeshProUGUI m_TimeLeftTxt;
    [SerializeField] private GameObject m_NewLabel;
    private UIHotSaleHeroPanel uiRoot;
    private HotSaleHeroEntity entity;
    private HotSaleHeroSave save;

    public void Init(EHero hero, UIHotSaleHeroPanel uiRoot)
    {
        this.uiRoot = uiRoot;
        entity      = DataManager.Base.HotSaleHero.GetSaleHero(hero);
        save        = DataManager.Save.HotSaleHero.Get(hero);
        m_NewLabel.SetActive(save.IsNew);
        m_BgImage.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.HotSaleHero, $"Bg_{hero}");
        Timing.RunCoroutine(_Ticks(), Segment.RealtimeUpdate, gameObject);
    }

    private void OnDisable()
    {
        Timing.KillCoroutines(gameObject);
    }

    private IEnumerator<float> _Ticks()
    {
        var endTimeSale = save.TimeEnd;
        while(true)
        {
            var current = DateTime.UtcNow;
            var left = endTimeSale - current;
            if (!save.IsActived) break;
            if (left.TotalSeconds <= 0) break;

            m_TimeLeftTxt.text = I2Localize.GetLocalize("Common/Title_TimeEndIn") + " " + left.ConvertTimeToString();
            yield return Timing.WaitForSeconds(1f);
        }
        save.DeActiveHotSale();
        this.uiRoot.UpdateUI();
        gameObject.SetActive(false);
    }

    public async void SaleOnClicked()
    {
        var ui = await PanelManager.CreateAsync<UIHotSaleHeroDetailPanel>(AddressableName.UIHotSaleHeroDetailPanel);
        ui.Show(entity.Hero);
    }
}