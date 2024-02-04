using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using Assets.Game.Scripts.Utilities;
using Cysharp.Threading.Tasks;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class UISkillTreeInforSkillPanel : UI.Panel
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameSkill;
    [SerializeField] private TextMeshProUGUI descriptionSkill;

    [SerializeField] private UIIconText costPanel;
    [SerializeField] private TextMeshProUGUI levelUnlockedTxt;
    [SerializeField] private GameObject buttonBuy, buttonLockLevel, buttonRequireUnlockLastSkill;

    private SkillTreeStageEntity entity;
    private ResourcesSave resource;
    private SkillTreeService service;
    private PlayerData playerData;
    public void Show(SkillTreeStageEntity entity)
    {
        base.Show();
        this.entity = entity;
        var name = I2Localize.GetLocalize($"Stat/{entity.Modifier.StatKey}");
        var description = I2Localize.GetLocalize($"SkillTree/{entity.Modifier.StatKey}");

        levelUnlockedTxt.text = I2Localize.GetLocalize("SkillTree/Title_Skill_Unlock_At", entity.Level);

        nameSkill.SetText(name);

        icon.sprite = ResourcesLoader.Instance.GetSprite(AtlasName.SkillTree, $"{entity.Modifier.StatKey}");

        buttonBuy.SetActive(false);
        buttonLockLevel.SetActive(false);
        buttonRequireUnlockLastSkill.SetActive(false);

        costPanel.Set(entity.Cost);
        if (service.IsUnlockSkill(entity.Level, entity.Index))
        {
            description = entity.Modifier.ToString();
        }
        descriptionSkill.SetText(description);

        if (playerData.ExpHandler.CurrentLevel < entity.Level)
        {
            buttonLockLevel.SetActive(true);
            return;
        }

        if (!service.CanUnlockSkill(entity.Level, entity.Index))
        {
            buttonRequireUnlockLastSkill.SetActive(true);
        }

        if (!service.IsUnlockSkill(entity.Level, entity.Index))
        {
            buttonBuy.SetActive(true);
        }
    }
    public override void PostInit()
    {
        resource = DataManager.Save.Resources;
        service = Architecture.Get<SkillTreeService>();
        playerData = GameSceneManager.Instance.PlayerData;
    }

    public async void BuySkillTreeOnClicked()
    {
        if (!resource.HasResource(entity.Cost))
        {
            PanelManager.ShowNotice(string.Format(I2Localize.I2_NoticeNotEnough, entity.Cost.Resource.GetLocalize()))
                .ContinueWith(t =>
                {
                    t.onClosed += () =>
                    {
                        switch (entity.Cost.Resource)
                        {
                            case EResource.Gold:
                                MenuGameScene.Instance.EnQueue(EFlashSale.Gold);
                                break;
                            case EResource.Gem:
                                MenuGameScene.Instance.EnQueue(EFlashSale.Gem);
                                break;
                        }
                    };
                }).Forget();
            return;
        }

        resource.DecreaseResource(entity.Cost);
        service.UnlockSkill(entity.Level, entity.Index);
        PanelManager.Instance.GetPanel<UISkillTreePanel>().container.UpdateUI();

        Close();
        var ui = await PanelManager.CreateAsync<UISkillTreeResultPanel>(AddressableName.UISkillTreeResultPanel);
        ui.Show(entity);

        // Track
        FirebaseAnalysticController.Tracker.NewEvent("spend_resource")
            .AddStringParam("item_category", entity.Cost.GetAllData()[0].Type.ToString())
            .AddStringParam("item_id", entity.Cost.Resource.ToString())
            .AddStringParam("source", "skill_tree")
            .AddStringParam("source_id", $"{entity.Level + 1}_{entity.Index + 1}")
            .AddDoubleParam("value", entity.Cost.Value)
            .AddDoubleParam("remaining_value", DataManager.Save.Resources.GetResource(entity.Cost.Resource))
            .AddDoubleParam("total_earned_value", FirebaseAnalysticController.Instance.GetTrackingResourceEarn(entity.Cost.Resource))
            .Track();
    }
}