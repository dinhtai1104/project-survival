using UnityEngine;

namespace UI.Tooltip
{
    public class UIBuffTooltip : UIBaseTooltip
    {
        public override void ShowTooltip()
        {
            var item = GetComponent<UIBuffItemBase>();
            var buffEntity = item.GetEntity();
            if (buffEntity != null)
            {
                //Debug.Log($"Show tooltip buff {buffEntity.Type} - {"Descriptions"}");

                //TooltipSystem.Instance.ShowTooltip(rectTransform, new TooltipData
                //{
                //    icon = ResourcesLoader.Instance.GetSprite("Buff", $"{buffEntity.Icon}"),
                //    header = $"{buffEntity.Type}",
                //    description = buffEntity.GetDescription()
                //});
            }
        }
    }
}