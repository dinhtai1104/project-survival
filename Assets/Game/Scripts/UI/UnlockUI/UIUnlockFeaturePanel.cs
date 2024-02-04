using Assets.Game.Scripts.Utilities;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Pool;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIUnlockFeaturePanel : UI.Panel
{
    public Image icon;
    public TextMeshProUGUI descriptionTxt;
    private EFeature feature;
    public override void PostInit()
    {
    }
    public void Show(EFeature feature)
    {
        base.Show();
        descriptionTxt.text = I2Localize.GetLocalize("Notice/Notice.Unlock_" + feature.ToString());
        this.icon.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.FeatureButton, feature.ToString()); ;
        this.icon.SetNativeSize();
        this.feature = feature;
    }
    public override void Close()
    {
        string featureStr = "";
        switch (feature)
        {
            case EFeature.SkillTree:
                featureStr = "skill-tree";
                break;
            case EFeature.Subscription:
                featureStr = "subscription";
                break;
            case EFeature.DungeonEvent:
                featureStr = "events";
                break;
            case EFeature.Quest:
                featureStr = "quest";
                break;
            case EFeature.SaleHero:
                featureStr = "sale-hero";
                break;
            case EFeature.Loadout:
                featureStr = "hero";
                break;
            case EFeature.PiggyBank:
                featureStr = "piggybank";
                break;
            case EFeature.PlayGift:
                featureStr = "play-gift";
                break;
            case EFeature.BattlePass:
                featureStr = "battle-pass";
                break;
        }

        try
        {
            FirebaseAnalysticController.Tracker.NewEvent("button_click")
                .AddStringParam("category", "notice")
                .AddStringParam("name", $"unlock-{featureStr}")
                .Track();
        }
        catch (System.Exception e)
        {

        }
        var uifloat = ResourcesLoader.Instance.Get<UIFloatIcon>(AddressableName.UIFloatIcon, PanelManager.Instance.transform);
        var menu = PanelManager.Instance.GetPanel<UIMainMenuPanel>();
        var target = menu.GetFeatureButton(feature);
        var featureButton = ResourcesLoader.Instance.GetSprite(AtlasName.FeatureButton, feature.ToString());
        uifloat.Set(featureButton, icon.rectTransform.position, target.GetComponent<RectTransform>(), 0.6f, (t)=>
        {
            ResourcesLoader.Instance.GetAsync<ParticleSystem>("VFX_UIFloat_Icon", t.transform).ContinueWith(effect=>
            {
                effect.transform.localPosition = Vector3.zero;
                t.GetComponent<CanvasGroup>().alpha = 1;
                effect.Play();
            }).Forget();

            target.transform.DOScale(Vector3.one * 0.7f, 0.1f).SetEase(Ease.InSine).OnComplete(() =>
            {
                target.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack).OnKill(() =>
                {
                    target.transform.localScale = Vector3.one;
                });
            }).OnKill(() =>
            {
                target.transform.localScale = Vector3.one;
            });
        });
        uifloat.Run().Forget();
        base.Close();
    }
}