using Engine;
using UnityEngine;
namespace Assets.Game.Scripts.Tasks.Enemy
{
    public class EnemyRangeTask : RangeTask
    {
        [SerializeField] protected BindGameConfig m_BulletVelocityConfig = new("[{0}]Skill_BulletVelocity", "10");
        [SerializeField] protected BindGameConfig m_BulletDamageConfig = new("[{0}]Skill_BulletVelocity", "10");
        [SerializeField] protected BindGameConfig m_BulletNumberConfig = new ("[{0}]Skill_BulletVelocity", "10");
        [SerializeField] protected BindGameConfig m_AngleZoneConfig = new("[{0}]Skill_BulletVelocity", "10");
        [SerializeField] protected BindGameConfig m_Piercing = new("[{0}]Skill_Piercing", "10");
        [SerializeField] protected BindGameConfig m_PiercingDamageReduce = new("[{0}]Skill_PiercingDamageReduce", "10");

        public override void Begin()
        {
            m_BulletVelocityConfig.SetId(Caster.gameObject.name);
            m_BulletDamageConfig.SetId(Caster.gameObject.name);
            m_BulletNumberConfig.SetId(Caster.gameObject.name);
            m_AngleZoneConfig.SetId(Caster.gameObject.name);
            m_Piercing.SetId(Caster.gameObject.name);
            m_PiercingDamageReduce.SetId(Caster.gameObject.name);

            base.Begin();
            DefaultSpeed = new Stat(m_BulletVelocityConfig.FloatValue);
        }

        protected override void Attack()
        {
            var lowerRotate = m_FirePoint.eulerAngles.z - m_AngleZoneConfig.FloatValue / 2f;
            var angleOffset = m_AngleZoneConfig.FloatValue / m_BulletNumberConfig.IntValue;
            for (int i = 0; i < m_BulletNumberConfig.IntValue; i++)
            {
                CreateBullet(Quaternion.Euler(0, 0, lowerRotate));
                lowerRotate += angleOffset;
            }
        }

        protected override void SetupBullet(Bullet2D bullet)
        {
            base.SetupBullet(bullet);
            bullet.TargetNumber = m_Piercing.IntValue;
            bullet.PiercingReduce = m_PiercingDamageReduce.FloatValue;
            bullet.SetSpeed(DefaultSpeed);
        }
    }
}
