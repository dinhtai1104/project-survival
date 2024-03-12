using Engine;
using Pool;
using UnityEngine;

namespace Engine.State.Weapon
{
    public class WeaponAttackState : BaseWeaponState<WeaponIdleState>
    {
        public override void Enter()
        {
            base.Enter();
        }

        public override void Execute()
        {
            base.Execute();
            if (!Weapon.SkillCaster.HasAvailableSkill)
            {
                NextState();
            }
            else
            {
                Weapon.SkillCaster.CastRandomAvailableSkill();
            }
        }


        //[SerializeField] private GameObject m_Bullet2D;
        //[SerializeField] private DamageDealer m_DamageDealer;
        //public override void Enter()
        //{
        //    base.Enter();
        //    m_DamageDealer = new DamageDealer();
        //    m_DamageDealer.Init(Weapon.Stat);

        //    var target = Weapon.TargetFinder.CurrentTarget;
        //    if (target != null)
        //    {
        //        // Attack
        //        var bullet = PoolManager.Instance.Spawn(m_Bullet2D).GetComponent<Bullet2D>();
        //        bullet.transform.position = Weapon.TriggerPoint.position;
        //        bullet.Owner = Weapon;
        //        bullet.TargetLayer = Weapon.EnemyLayerMask;

        //        bullet.Target = target.Trans;
        //        bullet.TargetPosition = target.CenterPosition;

        //        var dir = target.CenterPosition - Weapon.CenterPosition;
        //        bullet.transform.right = dir;

        //        if (m_DamageDealer != null)
        //        {
        //            bullet.DamageDealer?.CopyData(m_DamageDealer);
        //        }
        //        bullet.StartBullet();

        //    }
        //    Weapon.Fsm.ChangeState<WeaponIdleState>();
        //}
        //public override void Execute()
        //{
        //    base.Execute();

        //}
    }
}
