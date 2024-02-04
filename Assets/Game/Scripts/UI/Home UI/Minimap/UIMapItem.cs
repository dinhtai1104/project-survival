using Assets.Game.Scripts.UI.Home_UI.Minimap;
using Assets.Game.Scripts.Utilities;
using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIMapItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameZoneTxt;
    [SerializeField] private TextMeshProUGUI bestCleanedTxt;
    [SerializeField] private Image gateMapImg;
    [SerializeField] private Image gateMapMaskImg;
    [SerializeField] private List<UIMapBossVisual> mapBossVisuals;
    [SerializeField] private Material grayscaleMaterial;
    [SerializeField] private Transform effectTrans;
    [SerializeField] private GameObject[] particles;
    private DungeonSave zoneSave;
    private int dungeonId = -1;

    public int DungeonId { get => dungeonId; set => dungeonId = value; }
    public List<UIMapBossVisual> MapBossVisuals { get => mapBossVisuals; set => mapBossVisuals = value; }

    private void OnEnable()
    {
        Messenger.AddListener(EventKey.ChangeLanguage, LocalizationManager_OnLocalizeEvent);
    }

    private void LocalizationManager_OnLocalizeEvent()
    {
        if (DungeonId != -1)
        {
            Setup(DungeonId);
        }
    }

    private void OnDisable()
    {
        Messenger.RemoveListener(EventKey.ChangeLanguage, LocalizationManager_OnLocalizeEvent);
    }

    public Tween DOFade(float target, float from, float duration)
    {
        Sequence sq = DOTween.Sequence();
        var cv = GetComponentsInChildren<CanvasGroup>();
        foreach (var e in cv)
        {
            var tween = e.DOFade(target, duration).From(from).SetId(gameObject);
            sq.Join(tween);
        }
        return sq;
    }

    public void Setup(int dungeonId)
    {
        this.DungeonId = dungeonId;

        if (DungeonId == -1)
        {
            // Comming soon
            var cmS = ResourcesLoader.Instance.GetSprite(AtlasName.DungeonIcon, $"Dungeon_{dungeonId}");

            gateMapImg.sprite = cmS;
            gateMapMaskImg.sprite = cmS;
            bestCleanedTxt.text = "";
            nameZoneTxt.text = I2Localize.GetLocalize("Common/Title_DungeonCommingSoon");
            return;
        }

        zoneSave = DataManager.Save.Dungeon;
        var et = DataManager.Base.Dungeon.Get(dungeonId);

        gateMapImg.material = zoneSave.CanPlayDungeon(dungeonId) ? null : grayscaleMaterial;
        

        if (zoneSave.CurrentDungeon == DungeonId)
        {
            bestCleanedTxt.text = I2Localize.GetLocalize("Common/BestCleaned") + ": " + (zoneSave.BestStage) + "/" + et.Stages.Count;
        }
        else
        {
            bestCleanedTxt.text = "";
        }

        nameZoneTxt.text = $"{dungeonId + 1}. " + I2Localize.GetLocalize($"Dungeon/Dungeon_{DungeonId + 1}");
        var gateMap = ResourcesLoader.Instance.GetSprite(AtlasName.DungeonIcon, $"Dungeon_{dungeonId}");   
        gateMapImg.sprite = gateMap;

        var saves = DataManager.Save.DungeonWorld.Saves[dungeonId];

        int index = 0;
        foreach (var boss in MapBossVisuals)
        {
            boss.SetData(saves.Stages[index++]);
            
            if (!zoneSave.CanPlayDungeon(dungeonId))
            {
                boss.gameObject.SetActive(false);
            }
            else
            {
                boss.gameObject.SetActive(true);
            }

            if (zoneSave.IsDungeonCleared(saves.Stages[0].Dungeon) && !zoneSave.IsShowAnimDungeon(saves.Stages[0].Dungeon))
            {
                boss.gameObject.SetActive(false);
            }
        }

        gateMapMaskImg.sprite = gateMapImg.sprite;
        gateMapMaskImg.material = gateMapImg.material;

        foreach (var go in particles)
        {
            go.SetActive(zoneSave.CanPlayDungeon(dungeonId));
        }
        //SpawnEffectMap();
    }

    private async void SpawnEffectMap()
    {
        var prefab = "Eff/VFX_UI_Map_{0}.prefab".AddParams(DungeonId + 1);
        ResourcesLoader.Instance.GetGOAsync(prefab, effectTrans);

    }

    public void UpdateMap()
    {
        try
        {
            var saves = DataManager.Save.DungeonWorld.Saves[dungeonId];
            int index = 0;
            foreach (var boss in MapBossVisuals)
            {
                boss.SetData(saves.Stages[index++]);

                if (!zoneSave.CanPlayDungeon(dungeonId))
                {
                    boss.gameObject.SetActive(false);
                }
                else
                {
                    boss.gameObject.SetActive(true);
                }

                if (zoneSave.IsDungeonCleared(saves.Stages[0].Dungeon) && !zoneSave.IsShowAnimDungeon(saves.Stages[0].Dungeon))
                {
                    boss.gameObject.SetActive(false);
                }
            }
        }
        catch (Exception ex)
        {

        }
    }
}