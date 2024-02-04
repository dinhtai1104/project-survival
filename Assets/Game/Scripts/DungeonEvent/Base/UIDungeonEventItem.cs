using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using Assets.Game.Scripts.Utilities;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UIDungeonEventItem : MonoBehaviour
{
    public delegate void JoinDungeonEvent(EDungeonEvent dungeon);
    public JoinDungeonEvent onJoinDungeonEvent;

    private EDungeonEvent Type;
    [SerializeField] private TextMeshProUGUI titleEventTxt;
    [SerializeField] private TextMeshProUGUI dungeonEventTxt;
    [SerializeField] private Image dungeonEventImg;

    [SerializeField] private Button buttonPlayByEnergy;
    [SerializeField] private Button buttonPlayByAds;
    [SerializeField] private GameObject buttonOut;
    [SerializeField] private TextMeshProUGUI adLeftTxt;

    private DungeonEventSave save;
    private DungeonEventConfigEntity config;
    private ResourcesSave resource;

    private ResourceData energyData;

    private void OnEnable()
    {
        buttonPlayByEnergy.onClick.AddListener(PlayByEnergyOnClicked);
        buttonPlayByAds.onClick.AddListener(PlayByAdsOnClicked);
    }
    private void OnDisable()
    {
        buttonPlayByEnergy.onClick.RemoveListener(PlayByEnergyOnClicked);
        buttonPlayByAds.onClick.RemoveListener(PlayByAdsOnClicked);
    }

    public void Setup(EDungeonEvent Event)
    {
        Type = Event;
        save = DataManager.Save.DungeonEvent.Saves[Event];
        config = DataManager.Base.DungeonEventConfig.Get(Event)[save.CurrentDungeon];
        resource = DataManager.Save.Resources;

        SetInformation();
    }

    private void SetInformation()
    {
        titleEventTxt.text = I2Localize.GetLocalize($"Common/DungeonEvent_Title_{Type}");
        dungeonEventTxt.text = I2Localize.GetLocalize($"Common/Title_DungeonEvent_Level", save.CurrentDungeon + 1);
        dungeonEventImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.DungeonEvent, $"{save.Type}");

        energyData = new ResourceData { Resource = EResource.Energy, Value = config.Energy };

        buttonPlayByEnergy.gameObject.SetActive(false);
        buttonPlayByAds.gameObject.SetActive(false);
        buttonOut.SetActive(false);

        var leftFreeplay = save.FreeLeftDay;
        var leftFreeAd = save.AdLeftDay;

        if (leftFreeplay > 0)
        {
            adLeftTxt.text = leftFreeplay.ToString() + "/" + DataManager.Base.DungeonEventConfig.Get(Type)[save.CurrentDungeon].FreePlay;
            buttonPlayByEnergy.gameObject.SetActive(true);
            return;
        }

        if (leftFreeAd > 0)
        {
            adLeftTxt.text = leftFreeAd.ToString() + "/" + DataManager.Base.DungeonEventConfig.Get(Type)[save.CurrentDungeon].MaxAdInDay;
            buttonPlayByAds.gameObject.SetActive(true);
            return;
        }

        buttonOut.SetActive(true);
        adLeftTxt.text = "";
        buttonPlayByAds.gameObject.SetActive(false);
        buttonPlayByEnergy.gameObject.SetActive(false);
    }

    private void PlayByEnergyOnClicked()
    {
        if (resource.HasResource(energyData))
        {
            save.FreeLeftDay--;
            save.Save(); 
            resource.DecreaseResource(energyData);
            onJoinDungeonEvent?.Invoke(Type);
        }
        else
        {
            PanelManager.ShowNotice(string.Format(I2Localize.I2_NoticeNotEnough, energyData.Resource.GetLocalize())).Forget();
        }
    }

    private void PlayByAdsOnClicked()
    {
        Architecture.Get<AdService>().ShowRewardedAd(GetPlacement(Type), (result) =>
        {
            if (result)
            {
                save.AdLeftDay--;
                save.Save();
                onJoinDungeonEvent?.Invoke(Type);
            }
            else
            {
                PanelManager.ShowNotice(I2Localize.GetLocalize("Notice/Notice.NotAd"));
            }
        }, placement: GetPlacement(Type));

        string GetPlacement(EDungeonEvent type)
        {
            switch (type)
            {
                case EDungeonEvent.Gold:
                    return AD.AdPlacementKey.GOLD_EVENT;
                case EDungeonEvent.Scroll:
                    return AD.AdPlacementKey.SCROLL_EVENT;
                case EDungeonEvent.Stone:
                    return AD.AdPlacementKey.STONE_EVENT;
                default:
                    return null;
            }
        }
    }

    [Button]
    private void PlayTest()
    {
        onJoinDungeonEvent?.Invoke(Type);
    }
}