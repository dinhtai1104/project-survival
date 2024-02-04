using System.Collections.Generic;
using UnityEngine;
public abstract class UIHeroUpgradeBase : MonoBehaviour
{
    [SerializeField] protected EUpgradeHero upgradeType;
    [SerializeField] protected RectTransform affixHolder;
    [SerializeField] protected GameObject affixPrefab;
    [SerializeField] protected UITweenRunner upgradeEffect;

    public bool IsActive => gameObject.activeSelf;
    protected EHero heroType;
    protected HeroEntity heroEntity;
    protected List<GameObject> allAffixes = new List<GameObject>();
    protected HeroSave heroSave;
    protected UIHeroUpgradePanel uiView;
    protected UserSave userSave;
    protected PlayerData playerData;
    protected HeroStatData heroStatData;
    protected ResourcesSave resourcesSave;
    public virtual void Init(UIHeroUpgradePanel view, EHero eHero)
    {
        playerData = GameSceneManager.Instance.PlayerData;
        heroStatData = playerData.HeroDatas[eHero];
        Clear();
        userSave = DataManager.Save.User;
        resourcesSave = DataManager.Save.Resources;
        uiView = view;
        this.heroType = eHero;
        heroEntity = DataManager.Base.Hero.GetHero(eHero);
        heroSave = userSave.HeroSaves[heroType];
    }
    public abstract void UpgradeOnClicked();
    public void Clear()
    {
        foreach (var item in allAffixes)
        {
            PoolManager.Instance.Despawn(item);
        }
        allAffixes.Clear();
    }
}