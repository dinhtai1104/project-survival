using System;
using System.Collections.Generic;

[System.Serializable]
public class AchievementQuestSave
{
    public EAchievement Type;
    public int Progress;
    public int Received;

    public AchievementQuestSave(EAchievement type)
    {
        Type = type;
        Progress = 0;
        Received = 0;
    }
    public void SetProgress(int p)
    {
        if (p > Progress)
        {
            Progress = p;
        }
    }
}