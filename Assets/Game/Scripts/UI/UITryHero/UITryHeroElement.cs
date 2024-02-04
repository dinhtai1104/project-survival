using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UITryHeroElement : UIBaseButton
{
    [SerializeField] private Transform actorHolder;
    [SerializeField] protected UIActor uiActor;
    [SerializeField] protected TextMeshProUGUI nameHeroTxt;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private GameObject tryButton;
    [SerializeField] private GameObject adsButton;
    protected EHero heroType;

    public delegate UniTask OnClickedTryHero(EHero eHero);
    public OnClickedTryHero onClickedTryHero;

    private bool IsAdRequire;
    private bool IsOwner;
    protected override void OnEnable()
    {
        base.OnEnable();
    }
    public virtual void Init(EHero eHero, bool owner = false)
    {
        IsOwner = owner;
        heroType = eHero;
        nameHeroTxt.text = I2Localize.GetLocalize($"Hero_Name/{eHero}");

        Game.Pool.GameObjectSpawner.Instance.Get(string.Format(AddressableName.UIHero, eHero), obj => 
        {
            uiActor = obj.GetComponent<UIActor>();
            uiActor.transform.SetParent(actorHolder);
            uiActor.transform.localPosition = Vector3.zero;
            uiActor.transform.localScale = Vector3.one;
        });

        if (!owner)
        {
            if (eHero == EHero.AngelHero && DataManager.Save.TryHero.IsFreeHeroForFirstTime == false)
            {
                if (adsButton != null)
                {
                    tryButton.SetActive(true);
                    adsButton.SetActive(false);
                }
                IsAdRequire = false;
            }
            else
            {
                IsAdRequire = true;
                if (adsButton != null)
                {
                    tryButton.SetActive(false);
                    adsButton.SetActive(true);
                }
                else
                {
                    tryButton.SetActive(true);
                }
            }
        }
    }

    public override void Action()
    {
        if (!IsAdRequire)
        {
            if (!IsOwner)
            {
                DataManager.Save.TryHero.FreeFirstTime();
            }
            onClickedTryHero?.Invoke(heroType);
        }
        else
        {
            TryHeroByAdsOnClicked();
        }
    }

    public void DOAnimation(bool on)
    {
        transform.DOKill();
        canvasGroup.DOKill();

        if (on)
        {
            canvasGroup.DOFade(1, 0.25f).From(0);
            transform.DOScale(Vector3.one, 0.25f).From(Vector3.one * 1.5f);
        }
        else
        {
            canvasGroup.DOFade(0, 0.25f).From(1);
            transform.DOScale(Vector3.one * 1.5f, 0.25f);
        }
    }

    public void TryHeroByAdsOnClicked()
    {
        Architecture.Get<AdService>().ShowRewardedAd(AD.AdPlacementKey.TRIAL_HERO, (result) =>
        {
            if (result)
            {
                onClickedTryHero?.Invoke(heroType);
            }
            else
            {
                PanelManager.ShowNotice(I2Localize.GetLocalize("Notice/Notice.NotAd")).Forget();
            }
        }, placement: AD.AdPlacementKey.TRIAL_HERO);
    }

    private void OnDestroy()
    {
        uiActor.gameObject.SetActive(false);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        transform.DOKill();
        canvasGroup.DOKill();
        uiActor.gameObject.SetActive(false);
        if (adsButton)
        {
        }
    }
}