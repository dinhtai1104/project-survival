using BansheeGz.BGDatabase;
using System;

[System.Serializable]
public class SkillTreeTable : DataTable<int, SkillTreeEntity>
{
    int lastLevel = -1;
    public override void GetDatabase()
    {
        lastLevel = -1;
        DB_SkillTree.ForEachEntity(e => Get(e));

        Dictionary[lastLevel].GetLast().MilestoneEnd = true;
    }

    private void Get(BGEntity e)
    {
        int Level = e.Get<int>("Level");
        if (!Dictionary.ContainsKey(Level))
        {
            Dictionary.Add(Level, new SkillTreeEntity(e));
        }
        if (Level != lastLevel && lastLevel != -1)
        {
            Dictionary[lastLevel].GetLast().MilestoneEnd = true;
        }

        var skillStage = new SkillTreeStageEntity(e);
        Dictionary[Level].AddSkill(skillStage);

        lastLevel = Level;
    }
}
