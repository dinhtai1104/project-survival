using Assets.Game.Scripts.Core.Data.Database.Equipment.Weapon;
using Engine;
using Engine.Weapon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Tasks.Weapon
{
    public class WeaponRangeTask : RangeTask
    {
        public new WeaponActor Caster
        {
            set { m_Actor = value; }
            get { return m_Actor as WeaponActor; }
        }


        private int m_ProjectileCount = 1;
        private int m_PiercingCount = 0;
        private float m_PiercingReduceDmg = 0;
        private float m_AngleZone = 0;
        public override void Begin()
        {
            base.Begin();

            m_ProjectileCount = (int)Caster.Stats.GetValue(StatKey.Projectiles);
            m_PiercingCount = (int)Caster.Stats.GetValue(StatKey.Pierce);
            m_PiercingReduceDmg = Caster.Stats.GetValue(StatKey.PierceReduce);
            m_AngleZone = Caster.Stats.GetValue(StatKey.AngleZone);
        }

        protected override void Attack()
        {
            var lowerRotate = m_FirePoint.eulerAngles.z - m_AngleZone / 2f;
            var angleOffset = m_AngleZone / m_ProjectileCount;
            for (int i = 0; i < m_ProjectileCount; i++)
            {
                CreateBullet(Quaternion.Euler(0, 0, lowerRotate));
                lowerRotate += angleOffset;
            }
        }

        protected override void SetupBullet(Bullet2D bullet)
        {
            base.SetupBullet(bullet);
            bullet.TargetNumber = m_PiercingCount;
            bullet.PiercingReduce = m_PiercingReduceDmg;
        }
    }
}
