using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using DG.Tweening;
using System;
using System.Linq;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIAchievementQuestItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleTxt;
    [SerializeField] private TextMeshProUGUI _targetTxt;
    [SerializeField] private float _filledSizeX;
    [SerializeField] private RectTransform _filledRect;
    [SerializeField] private UIIconText _rewardPanel;

    [SerializeField] private GameObject buttonClaimed;
    [SerializeField] private GameObject buttonClaim;
    [SerializeField] private GameObject buttonGo;
    [SerializeField] private RectTransform markedGO;

    public AnimationCurve markCurve;
    private UIQuestPanel uiPanel;
    private AchievementQuestEntity entity;
    private AchievementQuestSave save;
    private AchievementService service;
    private int currentIndex = 0;
    private LootParams reward;
    public void Setup(AchievementQuestSave save, UIQuestPanel uiPanel)
    {
        this.uiPanel = uiPanel;
        this.entity = DataManager.Base.AchievementQuest.Get(save.Type);
        this.save = save;
        this.service = Architecture.Get<AchievementService>();

        UpdateUI();
    }

    public void UpdateUI()
    {
        buttonClaimed.SetActive(false);
        buttonClaim.SetActive(false);
        buttonGo.SetActive(false);
        currentIndex = entity.GetIndexStage(save.Progress);

        _titleTxt.text = I2Localize.GetLocalize($"Achievement/Title_{entity.Type}", entity.Achievements[currentIndex].Target);
        if (entity.Type >= EAchievement.OwnHeroPoison && entity.Type <= EAchievement.OwnHeroEvil)
        {
            _titleTxt.text = I2Localize.GetLocalize($"Achievement/Title_OwnHero", ((EHero)((int)entity.Type)).GetLocalize());
        }

        _targetTxt.text = $"{save.Progress}/{entity.GetStage(currentIndex).Target}";

        float percent = entity.GetPercentStage(save.Progress);
        float targetX = percent * _filledSizeX;
        _filledRect.sizeDelta = new Vector2(targetX, _filledRect.sizeDelta.y);
        reward = entity.GetStage(currentIndex).Reward;
        markedGO.gameObject.SetActive(false);
        _rewardPanel.gameObject.SetActive(true);

        _rewardPanel.Set(reward.Data);
        if (service.CanReceive(entity.Type))
        {
            buttonClaim.SetActive(true);
            return;
        }
        if (save.Received >= entity.Achievements.Count || save.Progress >= entity.Achievements.Last().Target)
        {
            _targetTxt.text = "";
            _rewardPanel.gameObject.SetActive(false);
            markedGO.gameObject.SetActive(true);

            buttonClaimed.SetActive(true);
            return;
        }
        buttonGo.SetActive(true);
    }

    public void ClaimOnClicked()
    {
        PanelManager.ShowRewards(reward);
        service.ReceiveAchievement(entity.Type);
        GetComponentInParent<UIAchievementQuestPage>().UpdateUI();

        if (save.Received >= entity.Achievements.Count || save.Progress >= entity.Achievements.Last().Target)
        {
            markedGO.DOScale(Vector3.one, 0.25f).From(Vector3.one * 2).SetEase(markCurve);
            markedGO.GetComponent<Image>().DOFade(1, 0.25f).From(0);
        }

        // Track
        FirebaseAnalysticController.Tracker.NewEvent("earn_resource")
            .AddStringParam("item_category", reward.Type.ToString())
            .AddStringParam("item_id", (reward.Data as ResourceData).Resource.ToString())
            .AddStringParam("source", "achievement")
            .AddStringParam("source_id", $"{entity.Type}_{entity.GetIndexStage(save.Progress)}")
            .AddFloatParam("value", (float)(reward.Data as ResourceData).Value)
            .AddDoubleParam("remaining_value", DataManager.Save.Resources.GetResource((reward.Data as ResourceData).Resource))
            .AddDoubleParam("total_earned_value", FirebaseAnalysticController.Instance.GetTrackingResourceEarn((reward.Data as ResourceData).Resource))
            .Track();
    }

    public void GoOnClicked()
    {
        AchievementQuestLocator.AchievementLocator(entity.Type, uiPanel);
    }
}