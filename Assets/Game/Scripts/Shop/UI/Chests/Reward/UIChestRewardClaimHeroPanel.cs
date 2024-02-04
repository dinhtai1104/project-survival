using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class UIChestRewardClaimHeroPanel : UI.Panel
{
    [SerializeField] private RectTransform heroParent;
    [SerializeField] private TextMeshProUGUI heroName;
    [SerializeField] private TextMeshProUGUI heroDescription;
    [SerializeField] private RectTransform posSpawnEffect;
    [SerializeField] private AnimationCurve curveAppear;

    [SerializeField] private GameObject tapToClose;

    private UIPlayerActor heroView;
    private bool canClose = false;
    public override void PostInit()
    {
    }

    public async void Show(EHero hero)
    {
        var ePath = string.Format(AddressableName.UIHero, hero);

        heroView = ResourcesLoader.Instance.Get<UIPlayerActor>(ePath, heroParent);

        tapToClose.SetActive(false);
        base.Show();
        //actor.SetSkin($"{hero}");
        heroName.SetText(I2Localize.GetLocalize($"Hero_Name/{hero}"));
        heroDescription.SetText(I2Localize.GetLocalize($"Buff_Description/{hero}Passive"));
        await UniTask.Delay(2000);
        canClose = true;
        tapToClose.SetActive(true);
    }
    public override void Close()
    {
        if (!canClose) return;

        if (heroView != null)
        {
            PoolManager.Instance.Despawn(heroView.gameObject);
        }
        base.Close();
    }

    public void PlayVFXAppear()
    {
        heroView?.PlayVFXAppear();
    }
}
