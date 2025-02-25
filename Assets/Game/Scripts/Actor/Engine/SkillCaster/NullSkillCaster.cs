using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public class NullSkillCaster : ISkillCaster
    {
        public bool IsLocked { get; set; }
        public ActorBase Owner { get; private set; }
        public ISkill CurrentSkill { get; }
        public IEnumerable<ISkill> ActiveSkills { get; }
        public IEnumerable<ISkill> AllSkills { get; }
        public bool HasWaitingSkills { get; }
        public bool HasAvailableSkill { get; }
        public bool IsBusy { get; }
        public bool IsExecuting { get; }

        public void Init(ActorBase actor)
        {
            Owner = actor;
        }

        public void OnUpdate()
        {
        }

        public ISkill GetSkillById(int id)
        {
            return null;
        }

        public bool AddSkill(ISkill skill)
        {
            return false;
        }

        public bool RemoveSkill(ISkill skill)
        {
            return false;
        }

        public void UpdateAvailableSkills()
        {
        }

        public bool CastSkillById(int id)
        {
            return false;
        }

        public void ForceCastSkillById(int id)
        {
        }

        public void SetLockSkill(int id, bool isLocked)
        {
        }

        public bool HasSkillId(int id)
        {
            return false;
        }

        public void InterruptCurrentSkill()
        {
        }

        public void InterruptAllSkills()
        {
        }

        public void ResetAllSkills()
        {
        }

        public bool CastRandomAvailableSkill()
        {
            return false;
        }
    }
}