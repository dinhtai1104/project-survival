using Cysharp.Threading.Tasks;
using TMPro;
using UI;
using UnityEngine;

public class UIShopResourcePanel : UI.Panel
{
    [SerializeField] private UIInventorySlot slot;
    [SerializeField] private UIIconText cost;
    [SerializeField] private TextMeshProUGUI realCost;
    [SerializeField] private TextMeshProUGUI title;
    private ShopResourceEntity entity;
    public override void PostInit()
    {
    }
    public void Show(EResource resource)
    {
        title.text = I2Localize.GetLocalize("Notice/Notice.NeedMoreResource").AddParams(resource.GetLocalize());
        entity = DataManager.Base.ShopResource.Get(resource);
        if (entity == null)
        {
            Close();
            return;
        }
        base.Show();
        UIHelper.GetUILootIcon(AddressableName.UILootItemPath.AddParams(ELootType.Resource), entity.Buy, slot.transform).ContinueWith(t =>
        {
            slot.SetItem(t);
        }).Forget();

        cost.Set(entity.Cost);
        realCost.text = $"{(entity.Cost.Value * 10).TruncateValue()}";
    }

    public void BuyOnClicked()
    {
        var res = DataManager.Save.Resources;
        if (res.HasResource(entity.Cost))
        {
            res.DecreaseResource(entity.Cost);
            PanelManager.ShowRewards(entity.Buy.GetAllData()).Forget();
            Close();
        }
        else
        {
            PanelManager.ShowNotice(I2Localize.I2_NoticeNotEnough.AddParams(entity.Cost.Resource)).Forget();
        }
    }
}
