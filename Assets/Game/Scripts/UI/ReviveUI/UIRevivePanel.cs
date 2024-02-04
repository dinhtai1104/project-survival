using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using com.mec;
using System;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIRevivePanel : UI.Panel
{
    [SerializeField] private TextMeshProUGUI cooldownTxt;
    [SerializeField] private Image cooldownImg;

    [SerializeField] private GameObject gemButton;
    [SerializeField] private GameObject cardButton;
    [SerializeField] private GameObject adButton;

    [SerializeField] private UIIconText gemCostPanel;
    [SerializeField] private UIIconText cardCostPanel;

    private ResourceData gemCost;
    private ResourceData reviveCost;
    private ResourcesSave save;
    private System.Action<bool> reviveCb;
    public override void PostInit()
    {
        save = DataManager.Save.Resources;
    }

    private void OnEnable()
    {
        gemButton.GetComponent<Button>().onClick.AddListener(GemReviveOnClicked);    
        cardButton.GetComponent<Button>().onClick.AddListener(CardReviveOnClicked);    
        adButton.GetComponent<Button>().onClick.AddListener(AdsReviveOnClicked);    
    }

    public void Show(System.Action<bool> reviveCb)
    {
        base.Show();
        this.reviveCb = reviveCb;
        gemCost = new LootParams(new ValueConfigSearch("[Revive]GemCost").StringValue).Data as ResourceData;
        reviveCost = new LootParams(new ValueConfigSearch("[Revive]ReviveCardCost").StringValue).Data as ResourceData;

        gemCostPanel.Set(gemCost);
        cardCostPanel.Set(reviveCost);

        gemButton.SetActive(false);
        cardButton.SetActive(false);
        adButton.SetActive(true);

        if (save.HasResource(reviveCost))
        {
            cardButton.SetActive(true);
        }
        else
        {
            gemButton.SetActive(true);
        }

        Timing.RunCoroutine(_Cooldown(), gameObject);
    }

    private void OnDisable()
    {
        Timing.KillCoroutines(gameObject);
        gemButton.GetComponent<Button>().onClick.RemoveListener(GemReviveOnClicked);
        cardButton.GetComponent<Button>().onClick.RemoveListener(CardReviveOnClicked);
        adButton.GetComponent<Button>().onClick.RemoveListener(AdsReviveOnClicked);
    }

    private IEnumerator<float> _Cooldown()
    {
        float timeRevive = new ValueConfigSearch("[Revive]Time", "3").FloatValue;
        float time = timeRevive;
        while (time > 0)
        {
            cooldownTxt.text = ((int)time).ToString();
            cooldownImg.fillAmount = time / timeRevive;

            time -= Time.deltaTime;
            yield return Timing.DeltaTime;
        }
        GetComponent<CanvasGroup>().interactable = false;
        onClosed += () => reviveCb?.Invoke(false);
        Close();
    }

    public void GemReviveOnClicked()
    {
        if (save.HasResource(gemCost))
        {
            GetComponent<CanvasGroup>().interactable = false;
            Timing.KillCoroutines(gameObject);
            save.DecreaseResource(gemCost);
            onClosed += () => reviveCb?.Invoke(true);
            Close();
        }
    }
    public void CardReviveOnClicked()
    {
        if (save.HasResource(reviveCost))
        {
            GetComponent<CanvasGroup>().interactable = false;
            Timing.KillCoroutines(gameObject);
            cardButton.SetActive(true);
            save.DecreaseResource(reviveCost);
            onClosed += () => reviveCb?.Invoke(true);
            Close();
        }
    }
    public void AdsReviveOnClicked()
    {
        Timing.PauseCoroutines(gameObject);
        Architecture.Get<AdService>().ShowRewardedAd(AD.AdPlacementKey.REVIVE, async (result) =>
        {
            if (result)
            {
                GetComponent<CanvasGroup>().interactable = false;
                Timing.KillCoroutines(gameObject);
                onClosed += () => reviveCb?.Invoke(true);
                Close();
            }
            else
            {
                var ui = await PanelManager.ShowNotice(I2Localize.GetLocalize("Notice/Notice.NotAd"));
                ui.SetConfirmCallback(() =>
                {
                    Timing.ResumeCoroutines(gameObject);
                });
                ui.SetCancelCallback(() =>
                {
                    Timing.ResumeCoroutines(gameObject);
                });
            }
        },placement: AD.AdPlacementKey.REVIVE);
    }

    public void CloseReviveClicked()
    {
        GetComponent<CanvasGroup>().interactable = false;
        Timing.KillCoroutines(gameObject);
        onClosed += () => reviveCb?.Invoke(false);
        Close();
    }
}