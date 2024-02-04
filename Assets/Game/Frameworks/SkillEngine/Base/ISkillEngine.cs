using Game.GameActor;

namespace Game.Skill
{
    public interface ISkillEngine
    {
        ActorBase Owner { get; }
        /// <summary>
        /// Check has any skill can cast (finish cooldown => can Cast)
        /// </summary>
        bool IsSkillAvailable { get; }
        /// <summary>
        /// Check has any skill is Executing, not finish cooldown
        /// </summary>
        bool IsBusy { get; }
        bool CanCastSkill { get; }
        void Initialize(ActorBase owner);
        ISkill GetSkill(int id);
        bool CastSkill(int id);
        bool CancelSkill(int id);
        void CancelAllSkill();
        void Ticks();
        bool CastSkillRandom();
        void InteruptCurrentSkill();
        void AddModifierCooldownSkill(int id, StatModifier modifier);
        void AddModifierCooldownAllSkill(StatModifier modifier);
        void RemoveModifierCooldown(int id, StatModifier modifier);
    }
}