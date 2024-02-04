using Cysharp.Threading.Tasks;
using DG.Tweening;
using UI;
using UnityEngine;

public static class UIHelper
{
    public static async UniTask<UIInventorySlot> GetUILootItem(string addressPathIcon, ILootData data, Transform holder, float size = 1)
    {
        var inventory = (await ResourcesLoader.Instance.GetAsync<UIInventorySlot>(AddressableName.UIInventorySlot, holder));
        if (inventory != null)
        {
#if DEVELOPMENT
#endif
            inventory.gameObject.SetActive(false);
            var fragmentLootIns = (await ResourcesLoader.Instance.GetAsync<UIGeneralBaseIcon>(addressPathIcon, inventory.transform));
#if DEVELOPMENT
#endif
            if (fragmentLootIns != null)
            {
                fragmentLootIns.SetData(data);
                fragmentLootIns.SetSizeImage(size);
                inventory.SetItem(fragmentLootIns);
            }
        }
        return inventory;
    }

    public static async UniTask<UIGeneralBaseIcon> GetUILootIcon(string addressPathIcon, ILootData data, Transform holder = null, float size = 1)
    {
        var fragmentLootIns = (await ResourcesLoader.Instance.GetAsync<UIGeneralBaseIcon>(addressPathIcon, holder));
        if (fragmentLootIns != null)
        {
            fragmentLootIns.SetData(data);
            fragmentLootIns.SetSizeImage(size);
        }
        return fragmentLootIns;
    }

    public static async UniTask<UIFloatIcon> FloatIcon(Sprite icon, Vector3 from, RectTransform target, float time = 0.6f, Transform parent = null)
    {
        var uifloat = ResourcesLoader.Instance.Get<UIFloatIcon>(AddressableName.UIFloatIcon, parent == null ? PanelManager.Instance.transform : parent);
        uifloat.Set(icon, from, target, time, (t) =>
        {
            ResourcesLoader.Instance.GetAsync<ParticleSystem>("VFX_UIFloat_Icon", t.transform).ContinueWith(effect =>
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
        await uifloat.Run();
        return uifloat;
    }
}   