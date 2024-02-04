using Assets.Game.Scripts._Services;
using Assets.Game.Scripts.BaseFramework.Architecture;
using BansheeGz.BGDatabase;
using System;
using System.Collections.Generic;

[System.Serializable]
public class AchievementQuestEntity
{
    public EAchievement Type;
    public List<AchievementStageEntity> Achievements = new List<AchievementStageEntity>();

    public AchievementQuestEntity(BGEntity e)
    {
        Enum.TryParse(e.Get<string>("Type"), out Type);
    }

    public AchievementStageEntity GetStage(int Index)
    {
        return Achievements[Index];
    }

    public void AddAchievement(AchievementStageEntity achievement)
    {
        achievement.Type = Type;
        Achievements.Add(achievement);
    }

    public int GetIndexStage(int progress)
    {
        var service = Architecture.Get<AchievementService>();
        AchievementStageEntity last = Achievements[0];
        for (int i = 0; i < Achievements.Count; i++)
        {
            var current = Achievements[i];

            if (progress >= current.Target)
            {
                if (!service.IsReceiveAchievementIndex(Type, current.Index))
                {
                    return last.Index;
                }
                else
                {
                    last = current;
                }
            }
            else
            {
                last = current;
                break;
            }
        }
        return last.Index;
    }

    public float GetPercentStage(int progress)
    {
        int index = GetIndexStage(progress);
        var percent = progress * 1.0f / Achievements[index].Target;

        return percent >= 1 ? 1 : percent;
    }
}

[System.Serializable]
public class AchievementStageEntity
{
    public int Index;
    public EAchievement Type;
    public int Target;
    public LootParams Reward;

    public AchievementStageEntity(BGEntity e)
    {
        Index = e.Get<int>("Index");
        Target = e.Get<int>("Target");
        Reward = new LootParams(e.Get<string>("Reward"));
    }
}