using Mosframe;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using com.mec;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;
using UI.Tooltip;
using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using Assets.Game.Scripts.Utilities;

public class UISkillTreeItem : UIBehaviour, IDynamicScrollViewItem
{
    [ReadOnly] public int Index = -1;
    public UISkillTreeContainer container;

    [SerializeField] private Image m_SkillBgImg;
    [SerializeField] private Image m_SkillIconImg;
    [SerializeField] private Image m_SkillIconGrayScaleImg;

    [SerializeField] private GameObject m_MilestoneLevelGO;
    [SerializeField] private TextMeshProUGUI m_MilestoneLevel;
    [SerializeField] private Image m_MilestoneLevelBgImg;

    [SerializeField] private GameObject m_BuyButton;

    private SkillTreeStageEntity entity;
    private SkillTreeSave save;
    private SkillTreeService service;
    private PlayerData playerData;

    protected override void Awake()
    {
        playerData = GameSceneManager.Instance.PlayerData;
    }

    public int getIndex()
    {
        return Index;
    }

    public void onUpdateItem(int index)
    {
        Index = index;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (container == null) { return; }
        entity = container.Convert(this.Index);
        if (entity == null) return;
        service = Architecture.Get<SkillTreeService>();
        save = service.GetSkill(entity.Level, entity.Index);

        m_MilestoneLevelGO.SetActive(entity.MilestoneEnd);
        m_MilestoneLevel.text = entity.Level.ToString();

        m_SkillIconImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.SkillTree, entity.Modifier.StatKey.ToString());
        m_SkillIconGrayScaleImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.SkillTree, entity.Modifier.StatKey.ToString());
        m_SkillIconImg.enabled = false;
        m_SkillIconGrayScaleImg.enabled = false;

        if (service.CanUnlockSkill(entity.Level, entity.Index))
        {
            m_BuyButton.gameObject.SetActive(true);
        }
        else
        {
            m_BuyButton.gameObject.SetActive(false);
        }

        if (entity.Level > playerData.ExpHandler.CurrentLevel)
        {
            m_SkillIconGrayScaleImg.enabled = true;
            Lock();
        }
        else
        {
            m_SkillIconImg.enabled = true;
            Unlock();
        }
        if (save.IsClaimed)
        {
            m_BuyButton.SetActive(false);
            m_SkillBgImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.SkillTree, "skill_actived");
        }
        else
        {
        }
    }

    private void Lock()
    {
        m_BuyButton.SetActive(false);
        m_MilestoneLevelBgImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.SkillTree, "level_lock");
        m_SkillBgImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.SkillTree, "skill_lock");
    }
    private void Unlock()
    {
        m_MilestoneLevelBgImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.SkillTree, "level_unlock");
        m_SkillBgImg.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.SkillTree, "skill_unlock");

        if (service.CanUnlockSkill(entity.Level, entity.Index))
        {
            m_BuyButton.gameObject.SetActive(true);
        }
        else
        {
            m_BuyButton.gameObject.SetActive(false);
        }
    }

    public async void ItemOnClicked()
    {
        if (container == null) { return; }
        var ui = await UI.PanelManager.CreateAsync<UISkillTreeInforSkillPanel>(AddressableName.UISkillTreeInforSkillPanel);
        ui.Show(entity);
    }

    public async void ClaimOnClicked()
    {
        if (container == null) { return; }

        var ui = await UI.PanelManager.CreateAsync<UISkillTreeInforSkillPanel>(AddressableName.UISkillTreeInforSkillPanel);
        ui.Show(entity);
    }
}
