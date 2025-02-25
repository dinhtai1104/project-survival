using Engine;
using UnityEngine;
namespace Assets.Game.Scripts.Tasks.Enemy
{
    public class EnemyRangeTask : RangeTask
    {
        [SerializeField] protected BindConfig m_BulletVelocityConfig = new("[{0}]Skill_BulletVelocity", "10");
        [SerializeField] protected BindConfig m_BulletDamageConfig = new("[{0}]Skill_BulletDamage", "1");
        [SerializeField] protected BindConfig m_BulletNumberConfig = new ("[{0}]Skill_BulletNumber", "1");
        [SerializeField] protected BindConfig m_AngleZoneConfig = new("[{0}]Skill_AngleZone", "0");
        [SerializeField] protected BindConfig m_Piercing = new("[{0}]Skill_Piercing", "0");
        [SerializeField] protected BindConfig m_PiercingDamageReduce = new("[{0}]Skill_PiercingDamageReduce", "0.25");

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
            var target = Caster.TargetFinder.CurrentTarget;
            var dir = target.CenterPosition - m_FirePoint.position;

            var quaternionTarget = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
            m_FirePoint.transform.rotation = quaternionTarget;
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
            bullet.TargetNumber = m_Piercing.IntValue + 1;
            bullet.PiercingReduce = m_PiercingDamageReduce.FloatValue;
            bullet.SetSpeed(DefaultSpeed);
        }
    }
}
