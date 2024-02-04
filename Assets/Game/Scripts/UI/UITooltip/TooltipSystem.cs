using Cysharp.Threading.Tasks;
using UnityEngine;
namespace UI.Tooltip
{
    public class TooltipSystem : LiveSingleton<TooltipSystem>
    {
        private UITooltipView tooltipPrefab;
        private UITooltipView tooltip;
        private async UniTask PreparePrefab()
        {
            var obj = await ResourcesLoader.Instance.LoadAsync<GameObject>(AddressableName.UITooltipViewPrath);
            tooltipPrefab = obj.GetComponent<UITooltipView>();
        }

        public async void ShowTooltip(RectTransform focusObject, TooltipData data)
        {
            if (tooltipPrefab == null)
            {
                await PreparePrefab();
            }
            if (tooltip == null)
            {
                tooltip = PoolManager.Instance.Spawn(tooltipPrefab, focusObject.parent);
            }
            tooltip.gameObject.SetActive(true);
            tooltip.transform.SetParent(focusObject.parent);
            var posTarget = Camera.main.WorldToScreenPoint(focusObject.transform.position);
            var pivotX = posTarget.x / Screen.width;
            var pivotY = posTarget.y / Screen.width;

            if (focusObject.transform.position.y >= 0)
            {
                pivotY = 1.5f;
            }
            else
            {
                pivotY = -0.5f;
            }
            tooltip.SetPivot(new Vector2(pivotX, pivotY));
            tooltip.transform.position = focusObject.position;

            tooltip.SetData(data);
        }
        public void Clear()
        {
            tooltip.gameObject.SetActive(false);
        }
    }

    public class TooltipData
    {
        public Sprite icon;
        public string header;
        public string description;
    }
}