using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public interface ISkillCaster
    {
        bool IsLocked { set; get; }
        ActorBase Owner { get; }
        ISkill CurrentSkill { get; }
        IEnumerable<ISkill> ActiveSkills { get; }
        IEnumerable<ISkill> AllSkills { get; }
        bool HasWaitingSkills { get; }
        bool HasAvailableSkill { get; }
        bool IsBusy { get; }
        bool IsExecuting { get; }
        void Init(ActorBase actor);
        void OnUpdate();
        ISkill GetSkillById(int id);
        bool AddSkill(ISkill skill);
        bool RemoveSkill(ISkill skill);
        void UpdateAvailableSkills();
        bool CastSkillById(int id);
        void ForceCastSkillById(int id);
        void SetLockSkill(int id, bool isLocked);
        bool HasSkillId(int id);
        void InterruptCurrentSkill();
        void InterruptAllSkills();
        void ResetAllSkills();
        bool CastRandomAvailableSkill();
    }
}