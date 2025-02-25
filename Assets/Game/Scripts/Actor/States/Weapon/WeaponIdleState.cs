using com.mec;
using Engine;
using ExtensionKit;
using UnityEngine;

namespace WeaponState
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
                    return;
                }
            }

            Vector2 dir = Vector2.zero;
            if (target != null)
            {
                dir = target.CenterPosition - Weapon.Trans.position;

            }
            else
            {
                // Follow Joystick
                dir = Owner.Movement.CurrentDirection;
            }

            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Weapon.Trans.eulerAngles = new Vector3(0, 0, angle);
            Weapon.Movement.SetDirection(dir);

            if (angle > 90 || angle < -90)
            {
                Weapon.Trans.LocalScaleY(-1);
            }
            else
            {
                Weapon.Trans.LocalScaleY(1);
            }
        }
    }
}
