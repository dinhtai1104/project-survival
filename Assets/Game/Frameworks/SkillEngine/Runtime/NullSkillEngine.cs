using Game.GameActor;

namespace Game.Skill
{
    public class NullSkillEngine : ISkillEngine
    {
        public ActorBase Owner => null;

        public bool IsSkillAvailable => false;

        public bool IsBusy => true;

        public bool CanCastSkill => false;

        public void AddModifierCooldownAllSkill(StatModifier modifier)
        {
        }

        public void AddModifierCooldownSkill(int id, StatModifier modifier)
        {
        }

        public void CancelAllSkill()
        {
        }

        public bool CancelSkill(int id)
        {
            return false;
        }

        public bool CastSkill(int id)
        {
            return false;
        }

        public bool CastSkillRandom()
        {
            return false;
        }

        public ISkill GetSkill(int id)
        {
            return null;
        }

        public void Initialize(ActorBase owner)
        {
        }

        public void InteruptCurrentSkill()
        {
        }

        public void RemoveModifierCooldown(int id, StatModifier modifier)
        {
        }

        public void Ticks()
        {
        }
    }
}