using Cysharp.Threading.Tasks;
using TMPro;
using UI;
using UnityEngine;

public class UIButtonNewRollTryHeroByCoin : UIBaseButton
{
    public ValueConfigSearch coinRequire;
    public TextMeshProUGUI coin;
    public UITryHeroPanel TryHeroPanel;

    private ResourceData currencyData;

    protected override void OnEnable()
    {
        base.OnEnable();
        coin.text = coinRequire.IntValue.TruncateValue();
        currencyData = new ResourceData { Resource = EResource.Gold, Value = coinRequire.IntValue };
    }
    public override void Action()
    {
        var coinSave = DataManager.Save.Resources;
        if (coinSave.HasResource(currencyData))
        {
            coinSave.DecreaseResource(currencyData);
            Debug.Log("Use Coin To Reroll");
            TryHeroPanel.Reroll();
        }
        else
        {
            PanelManager.ShowNotice(string.Format(I2Localize.I2_NoticeNotEnough, currencyData.Resource.GetLocalize())).Forget();
        }
    }
}
