using Assets.Game.Scripts.Utilities;
using Cysharp.Threading.Tasks;
using UI;
using UnityEngine;

public class UIShopOfferGoldItem : UIOfferBaseItem
{
    private OfferGoldEntity goldEntity => base.entity as OfferGoldEntity;
    [SerializeField] private UIIconText m_costGem;
    protected override void Setup()
    {
        base.Setup();
        base.m_ValueTxt.text = goldEntity.Value.TruncateValue();
        base.m_ValueFirstTimeTxt.text =  "+" + goldEntity.ValueFirstTime.TruncateValue();
        m_costGem.Set(goldEntity.Cost);
        m_Icon.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.Offer, $"coin_{goldEntity.Id + 1}");
        m_Icon.SetNativeSize();
    }

    protected override void OnClickOffer()
    {
        if (!resources.HasResource(goldEntity.Cost))
        {
            PanelManager.ShowNotice(string.Format(I2Localize.I2_NoticeNotEnough, goldEntity.Cost.Resource.GetLocalize()))
                .ContinueWith(t=>
                {
                    t.onClosed += () =>
                    {
                        MenuGameScene.Instance.EnQueue(EFlashSale.Gem);
                    };
                })
                .Forget();
            return;
        }
        save.Bought();
        resources.DecreaseResource(goldEntity.Cost);
        ShowRewards();
        Setup();
        CloudSave.Controller.Instance.ValidateAndSave().Forget();

        // Track
        FirebaseAnalysticController.Tracker.NewEvent("spend_resource")
            .AddStringParam("item_category", goldEntity.Cost.GetAllData()[0].Type.ToString())
            .AddStringParam("item_id", goldEntity.Cost.Resource.ToString())
            .AddStringParam("source", "shop")
            .AddStringParam("source_id", $"gold_{entity.GetValue()}")
            .AddDoubleParam("value", goldEntity.Cost.Value)
            .AddDoubleParam("remaining_value", DataManager.Save.Resources.GetResource(goldEntity.Cost.Resource))
            .AddDoubleParam("total_earned_value", FirebaseAnalysticController.Instance.GetTrackingResourceEarn(goldEntity.Cost.Resource))
            .Track();
    }
}