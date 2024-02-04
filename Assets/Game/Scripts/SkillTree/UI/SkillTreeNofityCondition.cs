using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;

public class SkillTreeNofityCondition : NotifyCondition
{
    public override bool Validate()
    {
        var service = Architecture.Get<SkillTreeService>();

        int level = GameSceneManager.Instance.PlayerData.ExpHandler.CurrentLevel;
        foreach (var entity in service.table.Dictionary.Values)
        {
            if (level >= entity.Level)
            {
                foreach (var index in entity.Skills)
                {
                    if (!service.IsUnlockSkill(entity.Level, index.Index) && service.CanUnlockSkill(entity.Level, index.Index) && DataManager.Save.Resources.HasResource(index.Cost))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
