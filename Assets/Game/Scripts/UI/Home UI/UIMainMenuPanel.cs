using Cysharp.Threading.Tasks;
using Game;
using GameUtility;
using Sirenix.OdinInspector;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class UIMainMenuPanel : UI.Panel
{
    private static UIMainMenuPanel instance;
    public static UIMainMenuPanel Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIMainMenuPanel>();
            }
            return instance;
        }
    }

    public UIMiniMapPanel MainMenuGate { get => mainMenuGate; }

    [SerializeField] private UIPlayerInfoView playerInforView;
    [SerializeField] private UIMiniMapPanel mainMenuGate;


    [SerializeField] private List<UIButtonFeature> buttonFeatures;
    [SerializeField] private List<UIButtonFlashSale> buttonFlashSales;
    [SerializeField] private RectTransform areaBehind;
    [SerializeField] private RectTransform areaMid;
    [SerializeField] private RectTransform areaFront;
    public CanvasGroup fadeBlackScreenMap;

    [SerializeField] private GameObject buttonPlay, buttonLockDungeon;

    public override void PostInit()
    {
        Sound.Controller.Instance.soundData.PlayMenuTheme();
    }

    private void OnEnable()
    {
        Messenger.AddListener<int>(EventKey.SelectMap, OnSelectMap);
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<int>(EventKey.SelectMap, OnSelectMap);
    }

    private void OnSelectMap(int map)
    {
        buttonLockDungeon.SetActive(false);

        if (map < 0)
        {
            // Map commin soon;
            buttonPlay.SetActive(false);
            buttonLockDungeon.SetActive(true);
            buttonLockDungeon.GetComponentInChildren<TextMeshProUGUI>().text = I2Localize.GetLocalize("Common/Description_DungeonComminSoon");

            return;
        }

        if (DataManager.Save.Dungeon.CanPlayDungeon(map))
        {
            buttonPlay.SetActive(true);
        }
        else
        {
            buttonPlay.SetActive(false);
            buttonLockDungeon.SetActive(true);
            buttonLockDungeon.GetComponentInChildren<TextMeshProUGUI>().text = I2Localize.GetLocalize("Common/Title_NeedClearDungeon", map);
        }
    }

    public override void Show()
    {
        MainMenuGate.Setup();
        base.Show();
        playerInforView.Setup();
    }

    public override void ShowByTransitions()
    {
        base.ShowByTransitions();
        foreach (var button in buttonFeatures)
        {
            button.InitFirst();
        }
    }

    public GameObject GetFeatureButton(EFeature feature)
    {
        return buttonFeatures.Find(t => t.Feature == feature).gameObject;
    }

    public async UniTask CheckButtonFeature()
    {
        foreach (var button in buttonFeatures)
        {
            button.InitFirst();
        }

        foreach (var button in buttonFeatures)
        {
            await button.Init();
        }

        foreach (var button in buttonFlashSales)
        {
            button.SetActive();
        }
    }
    public void UpdateMap()
    {
        mainMenuGate.UpdateMap();
    }

    public void CommingSoon()
    {
        buttonPlay.SetActive(false);
        buttonLockDungeon.SetActive(true);
        buttonLockDungeon.GetComponentInChildren<TextMeshProUGUI>().text = I2Localize.GetLocalize("Common/Description_DungeonComminSoon");
    }

    [Button]
    public async void TestNewDungeon()
    {
        var save = DataManager.Save.Dungeon;
        MainMenuGate.ScrollToDungeon(save.CurrentDungeon);

        var u = await PanelManager.CreateAsync<UINewDungeonPanel>(AddressableName.UINewDungeonPanel);
        u.Show(save.CurrentDungeon);
    }

    public GameObject FindFSButton(EFlashSale saleType)
    {
        return buttonFlashSales.Find(t => t.FlashSale == saleType).transform.parent.gameObject;
    }

    public void SetArea(GameObject target, EArea area)
    {
        switch (area)
        {
            case EArea.Behind:
                target.transform.SetParent(areaBehind);
                break;
            case EArea.Mid:
                target.transform.SetParent(areaMid);
                break;
            case EArea.Front:
                target.transform.SetParent(areaFront);
                break;
        }
    }
}

public enum EArea
{
    Behind,
    Mid,
    Front,
}