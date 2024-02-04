using Game.Skill;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

[System.Serializable]
public class SkillTreeSaves : BaseDatasave
{
    [ShowInInspector]
    public Dictionary<int, SkillTreeLevelSave> Saves;

    [NonSerialized]
    [ShowInInspector]
    private LinkedList<SkillTreeSave> _skillTreeNodes;

    public int CurrentIndex
    {
        get
        {
            var db = DataManager.Base.SkillTree;
            var currentIndex = 0;
            foreach (var level in db.Dictionary.Values)
            {
                foreach (var index in level.Skills)
                {
                    if (IsUnlockSkill(index.Level, index.Index))
                    {
                        currentIndex = index.Index;
                    }
                }
            }
            return currentIndex;
        }
    }

    public int CurrentLevel
    {
        get
        {
            return 0;
        }
    }

    public class SkillTreeNode
    {
        public SkillTreeSave skill;
        public bool IsClaim;
        public SkillTreeSave nextSkill;
    }

    public SkillTreeSaves(string key) : base(key)
    {
        Saves = new Dictionary<int, SkillTreeLevelSave>();

        var data = DataManager.Base.SkillTree;

        foreach (var skill in data.Dictionary)
        {
            var level = skill.Key;
            var skills = skill.Value;

            if (!Saves.ContainsKey(level))
            {
                Saves.Add(level, new SkillTreeLevelSave(level, skills));
            }
        }
    }
    [Button]
    public void UnlockSkill(int Level, int Index)
    {
        if (!Saves.ContainsKey(Level)) return;
        Saves[Level].Unlock(Index);
        Save();

        var playerData = GameSceneManager.Instance.PlayerData;
        var entity = DataManager.Base.SkillTree.Get(Level).Skills[Index];
        playerData.AddModifierSkillTree(entity.Modifier);
    }

    public bool IsUnlockSkill(int Level, int Index)
    {
        if (!Saves.ContainsKey(Level)) return false;
        return Saves[Level].IsUnlockSkill(Index);
    }

    public override void Fix()
    {
        var data = DataManager.Base.SkillTree;
        if (_skillTreeNodes == null)
        {
            _skillTreeNodes = new LinkedList<SkillTreeSave>();
        }
        foreach (var skill in data.Dictionary)
        {
            var level = skill.Key;
            var skills = skill.Value;

            if (!Saves.ContainsKey(level))
            {
                Saves.Add(level, new SkillTreeLevelSave(level, skills));
            }
        }
        foreach (var model in Saves)
        {
            foreach (var skill in model.Value.Saves)
            {
                if (!_skillTreeNodes.Contains(skill))
                {
                    _skillTreeNodes.AddLast(skill);
                }
            }
        }
    }

        


    public bool CanUnlockSkill(int Level, int Index)
    {
        //if (_skillTreeNodes==null||_skillTreeNodes.First == null) return false;
        var lastSkill = Saves[1].Saves[0];
        foreach (var skill in Saves)
        {
            foreach (var skillStage in skill.Value.Saves) 
            {
                if (skillStage == lastSkill) continue;
                if (skillStage.Index == Index && skillStage.Level == Level)
                {
                    if (lastSkill.IsClaimed)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                lastSkill = skillStage;
            }
        }
        return true;
    }

    public SkillTreeSave GetSkill(int level, int index)
    {
        return Saves[level].Saves[index];
    }
}
