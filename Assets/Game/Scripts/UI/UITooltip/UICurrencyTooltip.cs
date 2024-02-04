using Cysharp.Threading.Tasks;
using UnityEngine;

namespace UI.Tooltip
{
    public class UICurrencyTooltip : UIBaseTooltip
    {
        public EResource Type;
        public override void ShowTooltip()
        {
            Debug.Log("You are show tooltip at: " + Type);
            PanelManager.CreateAsync<UITooltipPanel>(AddressableName.UITooltipPanel).ContinueWith(t =>
            {
                t.Show(new ResourceData { Resource = Type, Value = DataManager.Save.Resources.GetResource(Type) });
            }).Forget();
        }
    }
}