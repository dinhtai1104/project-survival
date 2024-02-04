using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class UITryHeroPanel : UI.Panel
{
    public delegate void OnPickHero(EHero hero);
    public OnPickHero onPickHero;

    [SerializeField] private TextMeshProUGUI nameHeroRecommendTxt,passiveText;
    [SerializeField] private UITryHeroElement currentHeroUser;
    [SerializeField] private UITryHeroElement tryHeroRecommend;
    private TryHeroSaves dataSave;
    private UserSave userSave;
    EHero currentHero, recommendedHero;
    public override void PostInit()
    {
       
    }
    public void SetUp(EHero recommendedHero)
    {
        userSave = DataManager.Save.User;


        currentHero = userSave.Hero;
        this.recommendedHero = recommendedHero;
        // Set Skin
        currentHeroUser.Init(currentHero, true);
        tryHeroRecommend.Init(recommendedHero);
        nameHeroRecommendTxt.SetText(I2Localize.GetLocalize($"Hero_Name/{recommendedHero}"));

        currentHeroUser.onClickedTryHero = OnTryHeroSelect;
        tryHeroRecommend.onClickedTryHero = OnTryHeroSelect;
        //currentHeroUser.onClickedTryHero += OnTryHeroSelect;
        //tryHeroRecommend.onClickedTryHero += OnTryHeroSelect;

        passiveText.SetText(I2Localize.GetLocalize($"Buff_Description/{recommendedHero}Passive"));
        Show();
    }

    private async UniTask OnTryHeroSelect(EHero eHero)
    {
        currentHeroUser.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack);
        tryHeroRecommend.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack);
        await UniTask.Delay(100);
        ApplyNewDataUser(eHero);
        onPickHero?.Invoke(eHero);
    }

    private async UniTask PickHeroPlay(EHero eHero)
    {
        currentHeroUser.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack);
        tryHeroRecommend.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack);
        await UniTask.Delay(250);
        onPickHero?.Invoke(eHero);
    }
    public void SelectDefault()
    {
        PickHeroPlay(currentHero);
    }
    public void SelectRecommended()
    {
        PickHeroPlay(recommendedHero);

    }
    private void ApplyNewDataUser(EHero eHero)
    {
        if (eHero == DataManager.Save.User.Hero) return;
        var playerData = GameSceneManager.Instance.PlayerData;
        var userSave = DataManager.Save.User;
        userSave.SetTryHero(eHero);
     //   playerData.Stats.RemoveModifiersFromSource("Hero");

        var currentHero = userSave.HeroCurrent;
        playerData.HeroCurrent = currentHero;
        playerData.Stats.ReplaceAllStatBySource(playerData.HeroDatas[currentHero].heroStat, EStatSource.sourceHero);
    }

    public void Reroll()
    {
        var heroRecommend = dataSave.HeroRecommend; 
        tryHeroRecommend.Init(heroRecommend);
        nameHeroRecommendTxt.SetText(I2Localize.GetLocalize($"Hero_Name/{heroRecommend}"));
        DOAnimation();
    }

    private void DOAnimation()
    {
        tryHeroRecommend.DOAnimation(true);
        currentHeroUser.DOAnimation(true);
    }

    public async void ListHeroOnClick()
    {
        var ui = await UI.PanelManager.CreateAsync<UITryHeroCollectionsPanel>(AddressableName.UITryHeroCollectionsPanel);
        ui.onPickHero = (OnPickHero)onPickHero.Clone();
        ui.Show();
    }

    public void NoThanksOnClicked()
    {
        var heroCurrent = userSave.Hero;
        dataSave.SetTried(heroCurrent);
        PickHeroPlay(heroCurrent).Forget();
    }
}
