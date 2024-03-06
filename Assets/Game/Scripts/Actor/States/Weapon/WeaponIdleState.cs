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

        public override void Enter()
        {
            base.Enter();
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
                var dir = target.CenterPosition - Weapon.CenterPosition;
                Weapon.transform.LookAt2D(dir);
                Weapon.Movement.SetDirection(dir);
            }
            else
            {
                // Follow Joystick
                var directionMove = Owner.Movement.CurrentDirection;
                Weapon.transform.LookAt2D(directionMove);
                Weapon.Movement.SetDirection(directionMove);
            }
        }
    }
}
