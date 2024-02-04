using Assets.Game.Scripts.Utilities;
using com.mec;
using Cysharp.Threading.Tasks;
using Foundation.Game.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonHotSaleHero : UIBaseButton
{
    [SerializeField] private Image m_CurrentHeroImg;
    [SerializeField] private TextMeshProUGUI m_NameHeroTxt;
    [SerializeField] private TextMeshProUGUI m_TimeLeftTxt;
    [SerializeField] private Sprite m_ManyHeroSprite;

    protected override void OnEnable()
    {
        base.OnEnable();

        Setup();
      
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        Timing.KillCoroutines(gameObject);
    }

    private void Setup()
    {
        Timing.KillCoroutines(gameObject);
        var heroCurrent = DataManager.Save.HotSaleHero.GetHeroNewest();
        if (heroCurrent == null)
        {
            transform.parent.gameObject.SetActive(false);
            return;
        }
        m_NameHeroTxt.text = I2Localize.GetLocalize($"Hero_Name/{heroCurrent.Hero}");

        if (DataManager.Save.HotSaleHero.Saves.Count(t => t.Value.IsActived) > 1)
        {
            m_CurrentHeroImg.sprite = m_ManyHeroSprite;
        }
        else
        {
            m_CurrentHeroImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.Hero, $"{heroCurrent.Hero.ToString()}");
        }
        //m_CurrentHeroImg.SetNativeSize();
        Timing.RunCoroutine(_Ticks(heroCurrent));
    }

    private IEnumerator<float> _Ticks(HotSaleHeroSave heroCurrent)
    {
        var end = heroCurrent.TimeEnd;
        while(true)
        {
            if (!heroCurrent.IsActived || DataManager.Save.User.IsUnlockHero(heroCurrent.Hero)) break;
            var current = UnbiasedTime.UtcNow;
            var left = end - current;
            m_TimeLeftTxt.text = I2Localize.GetLocalize("Common/Title_TimeEndIn") + " " + left.ConvertTimeToString();
            if (left.TotalSeconds <= 0) break;

            yield return Timing.WaitForSeconds(1f);
        }
        heroCurrent.DeActiveHotSale();
        Setup();
    }


    public override async void Action()
    {
        var heroCurrent = DataManager.Save.HotSaleHero.Saves.Count(t => t.Value.IsActived);
        if (heroCurrent > 1)
        {
            var last = PanelManager.Instance.GetLast();
            last.HideByTransitions().Forget();

            PanelManager.CreateAsync(AddressableName.UIHotSaleHeroPanel).ContinueWith(panel =>
            {
                panel.Show();
                panel.onClosed += last.ShowByTransitions;
            }).Forget();
        }
        else
        {
            var last = PanelManager.Instance.GetLast();
            last.HideByTransitions().Forget();

            PanelManager.CreateAsync(AddressableName.UIHotSaleHeroDetailPanel).ContinueWith(panel =>
            {
                (panel as UIHotSaleHeroDetailPanel).Show(DataManager.Save.HotSaleHero.GetHeroNewest().Hero);
                panel.onClosed += last.ShowByTransitions;
            }).Forget();
        }
    }
}
