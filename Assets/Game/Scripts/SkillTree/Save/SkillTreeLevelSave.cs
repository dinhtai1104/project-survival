using System;
using System.Collections.Generic;

[System.Serializable]
public class SkillTreeLevelSave
{
    public int Level;
    public List<SkillTreeSave> Saves;

    public SkillTreeLevelSave()
    {
        Saves = new List<SkillTreeSave>();
    }

    public SkillTreeLevelSave(int level, SkillTreeEntity skills) : this()
    {
        this.Level = level;

        foreach (var skill in skills.Skills)
        {
            Saves.Add(new SkillTreeSave(Level, skill.Index, false));
        }
    }

    public void Unlock(int index)
    {
        Saves[index].IsClaimed = true;
    }

    public bool IsUnlockSkill(int index)
    {
        return Saves[index].IsClaimed;
    }
}