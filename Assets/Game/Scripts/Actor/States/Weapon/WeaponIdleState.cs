using com.mec;
using Engine;
using UnityEngine;

namespace Assets.Game.Scripts.Actor.States.Weapon
{
    public class WeaponIdleState : BaseWeaponState<WeaponAttackState>
    {
        [SerializeField] private Stat m_Cooldown;
        private float m_CooldownTimer = 0;
        private bool IsSkillAvailable => m_CooldownTimer >= m_Cooldown.Value;

        public override void InitializeStateMachine()
        {
            base.InitializeStateMachine();
        }

        public override void Enter()
        {
            base.Enter();
            m_Cooldown = new Stat(Actor.Stats.GetValue(StatKey.AttackSpeed));
            Actor.SkillCaster.GetSkillById(0).SetManuallyCooldown(m_Cooldown);
            m_CooldownTimer = 0;
            m_Cooldown.RecalculateValue();
        }

        public override void Execute()
        {
            base.Execute();
            m_CooldownTimer += Time.deltaTime;

            var target = Weapon.TargetFinder.CurrentTarget;
            if (target != null)
            {
                if (IsSkillAvailable && Weapon.SkillCaster.HasAvailableSkill)
                {
                    NextState();
                }
            }
        }
    }
}
