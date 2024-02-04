using Game.GameActor;

namespace Game.Skill
{
    public interface ISkill
    {
        ActorBase Caster { get; }
        int Id { get; }
        bool CanCast { get; }
        bool IsExecuting { get; }
        bool IsCoolingDown { get; }
        float CoolDownTimer { get; }
        void SetCoolDown(float time);
        float GetCoolDown();
        Stat GetCooldownStat();
        void Initialize(ActorBase actor);
        void Ticks();
        void Cast();
        void Stop();
        void AddModifierCooldown(StatModifier modifier);
        void RemoveModifierCooldown(StatModifier modifier);
    }
}