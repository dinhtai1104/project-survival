
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityPlayerPrefs;

public class DungeonWorldNotifyCondition : NotifyCondition
{
    public override bool Validate()
    {
        var dungeonWorldSave = DataManager.Save.DungeonWorld;
        var dungeonWorldTable = DataManager.Base.DungeonWorld;
        var dungeonSave = DataManager.Save.Dungeon;

        for (int i = 0; i <= dungeonSave.CurrentDungeon; i++)
        {
            for (int j = 0; j < dungeonWorldTable.Get(i).Stages.Count; j++)
            {
                var stage = dungeonWorldTable.Get(i).Stages[j];
                if (dungeonSave.IsDungeonCleared(stage.Dungeon))
                {
                    if (!dungeonWorldSave.IsClaimedReward(stage.Dungeon, stage.Stage))
                    {
                        return true;
                    }
                }
                else
                {
                    if (dungeonSave.CurrentDungeon >= stage.Dungeon)
                    {
                        if (dungeonSave.BestStage >= stage.Stage)
                        {
                            if (!dungeonWorldSave.IsClaimedReward(stage.Dungeon, stage.Stage))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }
}
