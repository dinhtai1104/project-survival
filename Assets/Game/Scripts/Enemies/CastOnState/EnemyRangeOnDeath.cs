using com.mec;
using Engine;
using Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Game.Scripts.Enemies.CastOnState
{
    public class EnemyRangeOnDeath : MonoBehaviour, IActionEnterState
    {
        [SerializeField] protected Transform m_FirePoint;
        [SerializeField] protected GameObject m_BulletPrefab;
        public bool IsRandomDirection = false;

        private DamageDealer m_DamageDealer = new DamageDealer();
        [SerializeField] protected BindGameConfig m_BulletVelocityConfig = new("[{0}]Skill_BulletVelocity", "10");
        [SerializeField] protected BindGameConfig m_BulletDamageConfig = new("[{0}]Skill_BulletDamage", "1");
        [SerializeField] protected BindGameConfig m_BulletNumberConfig = new("[{0}]Skill_BulletNumber", "1");
        [SerializeField] protected BindGameConfig m_Delay = new("[{0}]Skill_BulletDelay", "0.1");


        public void OnEnterState(ActorBase Caster)
        {
            m_DamageDealer.Init(Caster.Stats);
            m_BulletVelocityConfig.SetId(Caster.gameObject.name);
            m_BulletDamageConfig.SetId(Caster.gameObject.name);
            m_BulletNumberConfig.SetId(Caster.gameObject.name);
            m_Delay.SetId(Caster.gameObject.name);

            Timing.RunCoroutine(_Shoot(Caster));
        }

        private IEnumerator<float> _Shoot(ActorBase Caster)
        {
            var target = Caster.TargetFinder.CurrentTarget;

            for (int i = 0; i < m_BulletNumberConfig.IntValue; i++)
            {
                var dir = target.CenterPosition - m_FirePoint.position;
                var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Quaternion rotateTarget = Quaternion.Euler(0, 0, angle);
                if (IsRandomDirection)
                {
                    rotateTarget = Quaternion.Euler(0, 0, Random.Range(0, 360));
                }

                CreateBullet(rotateTarget, Caster);
                yield return Timing.WaitForSeconds(m_Delay.FloatValue);
            }
        }

        protected virtual Bullet2D CreateBullet(Quaternion angle, ActorBase Caster)
        {
            var firePosition = m_FirePoint.position;
            var fireRotation = angle;

            GameObject gameObject = PoolFactory.Spawn(m_BulletPrefab, firePosition, fireRotation);

            Bullet2D bullet = gameObject.GetComponent<Bullet2D>();
            bullet.Owner = Caster;
            bullet.TargetLayer = Caster.EnemyLayerMask;


            // Add rotation noise
            Vector3 eulerAngles = bullet.Trans.eulerAngles;
            bullet.Trans.rotation = Quaternion.Euler(eulerAngles);
            var target = Caster.TargetFinder.CurrentTarget;
            if (target != null)
            {
                bullet.Target = target.Trans;
                bullet.TargetPosition = target.CenterPosition;
            }

            if (m_FirePoint != null)
            {
                bullet.Firepoint = m_FirePoint;
            }

            if (m_DamageDealer != null)
            {
                bullet.DamageDealer?.CopyData(m_DamageDealer);
            }
            bullet.SetSpeed(new Stat(m_BulletVelocityConfig.FloatValue));
            SetupBullet(bullet);
            bullet.StartBullet();

            return bullet;
        }

        private void SetupBullet(Bullet2D bullet)
        {
        }
    }
}
