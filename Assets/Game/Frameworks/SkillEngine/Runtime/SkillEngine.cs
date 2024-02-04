using Game.GameActor;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Skill
{
    public class SkillEngine : MonoBehaviour, ISkillEngine
    {
        [SerializeField] private Transform skillHolder;
        private ActorBase _owner;
        private BaseSkill[] allSkills;
        [ShowInInspector]
        private Dictionary<int, ISkill> _allSkillsDict = new Dictionary<int, ISkill>();
        private List<BaseSkill> _skillAvailable = new List<BaseSkill>();
        [ShowInInspector] private ISkill _currentSkill;

        [SerializeField] private List<BaseSkill> _skillRunOverlifetime = new List<BaseSkill>();

        public ActorBase Owner => _owner;
        public bool IsSkillAvailable => _skillAvailable.Count > 0;
        public ISkill CurrentSkill => _currentSkill;
        public bool IsBusy
        {
            get
            {
                foreach (var skill in allSkills)
                {
                    if (skill.IsExecuting)
                        return true;
                }

                return false;
            }
        }
        public bool CanCastSkill
        {
            get
            {
                foreach (var skill in allSkills)
                {
                    if (skill.CanCast)
                        return true;
                }

                return false;
            }
        }

        public List<BaseSkill> SkillRunOverlifetime => _skillRunOverlifetime;

        public void Initialize(ActorBase owner)
        {
            this._owner = owner;
            allSkills = skillHolder.GetComponentsInChildren<BaseSkill>();
            _allSkillsDict.Clear();
            foreach (var skill in allSkills)
            {
                skill.Initialize(Owner);
                _allSkillsDict.Add(skill.Id, skill);
            }
        }
        public void CancelAllSkill()
        {
            foreach (var skill in _allSkillsDict)
            {
                skill.Value.Stop();
            }
        }

        public bool CancelSkill(int id)
        {
            if (!_allSkillsDict.ContainsKey(id))
            {
                return false;
            }
            _allSkillsDict[id].Stop();
            return true;
        }

        public bool CastSkill(int id)
        {
            if (Owner.IsDead()) return false;
            if (CurrentSkill != null)
            {
                InteruptCurrentSkill();
            }
            if (!_allSkillsDict.ContainsKey(id))
            {
                return false;
            }
            _currentSkill = _allSkillsDict[id];
            _allSkillsDict[id].Cast();
            return true;
        }

        public ISkill GetSkill(int id)
        {
            if (!_allSkillsDict.ContainsKey(id))
            {
                return null;
            }
            return _allSkillsDict[id];
        }

        public void Ticks()
        {
            for (int i = 0; i < allSkills.Length; ++i)
            {
                allSkills[i].Ticks();
            }
            // Skill Run overlife time
            for (int i = 0; i < _skillRunOverlifetime.Count; i++)
            {
                if (_skillRunOverlifetime[i] != null)
                {
                    _skillRunOverlifetime[i].Ticks();
                }
            }
            UpdateAvailableSkills();
        }

        public void InteruptCurrentSkill()
        {
            if (_currentSkill == null) return;
            _currentSkill.Stop();
            _currentSkill = null;
        }
        private void UpdateAvailableSkills()
        {
            if (allSkills != null)
            {
                // Find available skills
                for (int i = 0; i < allSkills.Length; ++i)
                {
                    BaseSkill skill = allSkills[i];

                    if (skill.CanCast && !_skillAvailable.Contains(skill))
                    {
                        _skillAvailable.Add(skill);
                    }
                }
            }
            if (_skillAvailable != null)
            {
                // Remove unavailable skills
                for (int i = _skillAvailable.Count - 1; i >= 0; --i)
                {
                    BaseSkill skill = _skillAvailable[i];

                    if (!skill.CanCast)
                        _skillAvailable.RemoveAt(i);
                }
            }
        }

        public bool CastSkillRandom()
        {
            if (IsBusy || _skillAvailable.Count == 0)
                return false; 
            if (_allSkillsDict.Count == 0) return false;
            var randomSkill = _skillAvailable[UnityEngine.Random.Range(0, _skillAvailable.Count)];
            return CastSkill(randomSkill.Id);
        }

        public void AddModifierCooldownSkill(int id, StatModifier modifier)
        {
            var skill = GetSkill(id);
            if (skill == null) return;
            skill.AddModifierCooldown(modifier);
        }

        public void AddModifierCooldownAllSkill(StatModifier modifier)
        {
            foreach (var skill in _allSkillsDict)
            {
                skill.Value.AddModifierCooldown(modifier);
            }
            foreach (var skill in _skillRunOverlifetime)
            {
                skill.AddModifierCooldown(modifier);
            }
        }

        public void RemoveModifierCooldown(int id, StatModifier modifier)
        {
            var skill = GetSkill(id);
            if (skill == null) return;
             skill.RemoveModifierCooldown(modifier);
        }
    }
}