using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class UISkillTreePanel : UI.Panel
{
    public UISkillTreeContainer container;
    public NotifyCondition hasSkillTreeCanBuy;
    public GameObject buttonQuickBuy;
    public override void PostInit()
    {
    }
    public override void Show()
    {
        base.Show();
        container.Init();
        buttonQuickBuy.SetActive(hasSkillTreeCanBuy.Validate());
    }

    public void QuickBuyOnClicked()
    {
        var db = DataManager.Base.SkillTree;
        var service = Architecture.Get<SkillTreeService>();
        var resource = DataManager.Save.Resources;
        var levelUser = GameSceneManager.Instance.PlayerData.ExpHandler.CurrentLevel;

        var currentIndex = service.CurrentIndex;
        var currentLevel = service.CurrentLevel;

        var list = new List<SkillTreeStageEntity>();
        bool finish = false;
        foreach (var level in db.Dictionary.Values)
        {
            if (finish) break;
            foreach (var index in level.Skills)
            {
                if (!service.IsUnlockSkill(index.Level, index.Index) && index.Level <= levelUser)
                {
                    if (resource.HasResource(index.Cost))
                    {
                        list.Add(index);
                        resource.DecreaseResource(index.Cost);
                        service.UnlockSkill(index.Level, index.Index);
                    }
                    else
                    {
                       // MenuGameScene.Instance.EnQueue(EFlashSale.Gold);
                        Logger.Log("Enqueue flash sale skill tree");
                        finish = true;
                        break;
                    }
                }
            }
        }
        buttonQuickBuy.SetActive(false);

        container.UpdateUI();
        var result = list.Merge();
        PanelManager.ShowRewards(result, false);
    }
}
