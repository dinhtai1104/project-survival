using Assets.Game.Scripts.BaseFramework.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts._Services
{
    public class SkillTreeService : Service
    {
        public SkillTreeTable table;
        public SkillTreeSaves save;

        public int CurrentIndex => save.CurrentIndex;
        public int CurrentLevel => save.CurrentLevel;

        public override void OnInit()
        {
            base.OnInit();
            table = DataManager.Base.SkillTree;
            save = DataManager.Save.SkillTree;
        }
        public override void OnStart()
        {
            base.OnStart();
            ApplySkillTree(GameSceneManager.Instance.PlayerData);
        }
        public void UnlockSkill(int Level, int Index)
        {
            save.UnlockSkill(Level, Index);
        }

        public bool IsUnlockSkill(int Level, int Index)
        {
            return save.IsUnlockSkill(Level, Index);
        }

        public bool CanUnlockSkill(int Level, int Index)
        {
            return save.CanUnlockSkill(Level, Index);
        }

        public void ApplySkillTree(PlayerData player)
        {
            var data = DataManager.Base.SkillTree;
            var mods = new List<SkillTreeStageEntity>();
            foreach (var skill in data.Dictionary)
            {
                var level = skill.Key;
                var skills = skill.Value;

                foreach (var skillId in skills.Skills)
                {
                    if (IsUnlockSkill(level, skillId.Index))
                    {
                        mods.Add(skillId);
                    }
                }
            }
            var att = mods.MergeSkillTree();
            foreach (var mod in att)
            {
                player.AddModifierSkillTree(mod);
            }
        }

        public void ApplySkillTree(IStatGroup stat)
        {
            stat.RemoveModifiersFromSource(EStatSource.SkillTree);
            var data = DataManager.Base.SkillTree;
            var mods = new List<SkillTreeStageEntity>();
            foreach (var skill in data.Dictionary)
            {
                var level = skill.Key;
                var skills = skill.Value;

                foreach (var skillId in skills.Skills)
                {
                    if (IsUnlockSkill(level, skillId.Index))
                    {
                        mods.Add(skillId);
                    }
                }
            }
            var att = mods.MergeSkillTree();
            foreach (var mod in att)
            {
                stat.AddModifier(mod.StatKey, mod.Modifier, EStatSource.SkillTree);
            }
        }

        public SkillTreeSave GetSkill(int level, int index)
        {
            return save.GetSkill(level, index);
        }
    }
}
