using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityPlayerPrefs;
using DG.Tweening;
using GoogleMobileAds.Api;
using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIDailyQuestItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleQuestTxt;
    [SerializeField] private TextMeshProUGUI targetQuestTxt;
    [SerializeField] private UIIconText rewardQuest;
    [SerializeField] private RectTransform _rectFilled;
    [SerializeField] private float _rectXSizeMax;

    [SerializeField] private RectTransform claimedIcon;
    [SerializeField] private GameObject goButton;
    [SerializeField] private GameObject claimedButton;
    [SerializeField] private GameObject claimButton;
    [SerializeField] private GameObject claimButtonAds;

    [SerializeField] private AnimationCurve markedCurve;
    private UIQuestPanel uiPanel;
    private UIDailyQuestPage uiPage;

    private DailyQuestEntity questEntity;
    private DailyQuestSave questSave;

    private void OnEnable()
    {
        goButton.GetComponent<Button>().onClick.AddListener(GoLocatorOnClicked);
        claimButton.GetComponent<Button>().onClick.AddListener(ClaimQuestOnClicked);
        claimButtonAds.GetComponent<Button>().onClick.AddListener(ClaimQuestAdsOnClicked);
    }
    private void OnDisable()
    {
        goButton.GetComponent<Button>().onClick.RemoveListener(GoLocatorOnClicked);
        claimButton.GetComponent<Button>().onClick.RemoveListener(ClaimQuestOnClicked);
        claimButtonAds.GetComponent<Button>().onClick.RemoveListener(ClaimQuestAdsOnClicked);
    }

    public void Setup(UIQuestPanel panel, DailyQuestEntity questEntity)
    {
        this.uiPanel = panel;
        this.questEntity = questEntity;
        uiPage = GetComponentInParent<UIDailyQuestPage>();
        SetInformation();
    }

    private void SetInformation()
    {
        questSave = Architecture.Get<QuestService>().FindMission(questEntity.Type);
        titleQuestTxt.text = I2Localize.GetLocalize($"Quest/Title_{questEntity.Type}");
        targetQuestTxt.text = $"{questSave.Progress.TruncateValue()}/{questEntity.Target.TruncateValue()}";

        goButton.SetActive(false);
        claimedButton.SetActive(false);
        claimButton.SetActive(false);
        claimButtonAds.SetActive(false);
        rewardQuest.gameObject.SetActive(true);
        rewardQuest.Set(new ResourceData { Resource = EResource.DailyQuestMedal, Value = questEntity.Score });
        // Set Filled
        var targetX = _rectXSizeMax * questSave.Progress * 1.0f / questEntity.Target;
        targetX = Mathf.Clamp(targetX, 0, _rectXSizeMax);
        _rectFilled.sizeDelta = new Vector2(targetX, _rectFilled.sizeDelta.y);
        claimedIcon.gameObject.SetActive(false);

        if (questSave.Received)
        {
            rewardQuest.gameObject.SetActive(false);
            targetQuestTxt.text = "";

            claimedIcon.gameObject.SetActive(true);
            claimedButton.SetActive(true);
            return;
        }

        if (questEntity.Target <= questSave.Progress)
        {
            if (questEntity.Claim == EMissionClaim.None)
            {
                claimButton.SetActive(true);
            }
            else
            {
                claimButtonAds.SetActive(true);
            }
            return;
        }
        goButton.SetActive(true);
    }

    public void ClaimQuestOnClicked()
    {
        ClaimQuest();
        uiPage.UpdateUI();
    }
    public void ClaimQuestAdsOnClicked()
    {
        Architecture.Get<AdService>().ShowRewardedAd("quest_ads", (result) =>
        {
            if (result)
            {
                ClaimQuest();
                uiPage.UpdateUI();
            }
            else
            {
                PanelManager.ShowNotice(I2Localize.GetLocalize("Notice/Notice.NotAd"));
            }
        },placement:AD.AdPlacementKey.QUEST_AD);
    }

    public void GoLocatorOnClicked()
    {
        DailyQuestLocator.MissionLocator(questEntity.Type, uiPanel);
    }

    private void ClaimQuest()
    {
        //PanelManager.ShowRewards(questEntity.Reward, false);
        Architecture.Get<QuestService>().ReceiveMission(questEntity.Id);
        UpdateUI();
        uiPage.DOAnimationMedalToTarget(Vector3.zero);
        Messenger.Broadcast(EventKey.ClaimQuest);

        rewardQuest.gameObject.SetActive(false);
        claimedIcon.gameObject.SetActive(true);
        //claimedIcon.DOScale(Vector3.one, 0.25f).From(Vector3.one * 2).SetEase(markedCurve);
        //claimedIcon.GetComponent<Image>().DOFade(1, 0.25f).From(0);

        // Track
        FirebaseAnalysticController.Tracker.NewEvent("earn_resource")
            .AddStringParam("item_category", "Resource")
            .AddStringParam("item_id", EResource.DailyQuestMedal.ToString())
            .AddStringParam("source", "daily_quest")
            .AddStringParam("source_id", questEntity.Id.ToString())
            .AddFloatParam("value", questEntity.Score)
            .AddDoubleParam("remaining_value", Architecture.Get<QuestService>().Medal)
            .AddDoubleParam("total_earned_value", FirebaseAnalysticController.Instance.GetTrackingResourceEarn(EResource.DailyQuestMedal))
            .Track();
    }

    public void UpdateUI()
    {
        SetInformation();
    }
}