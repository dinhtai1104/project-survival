using BansheeGz.BGDatabase;
using System.Collections.Generic;

[System.Serializable]
public class SkillTreeEntity
{
    public int Level;
    public List<SkillTreeStageEntity> Skills = new List<SkillTreeStageEntity>();

    public SkillTreeEntity(BGEntity e)
    {
        Level = e.Get<int>("Level");
    }

    public SkillTreeStageEntity GetLast() => Skills[Skills.Count - 1];
    public void AddSkill(SkillTreeStageEntity e)
    {
        Skills.Add(e);
    }
}