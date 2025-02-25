using Engine;
using Pool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Enemies.CastOnState
{
    public class ExplodeOnDeath : MonoBehaviour, IActionEnterState
    {
        [SerializeField] private BindConfig m_RadiusExplode = new BindConfig("[{0}]DeadExplodeRadius", 3);
        [SerializeField] private BindConfig m_DamageExplode = new BindConfig("[{0}]DeadExplodeDamage", 1.2f);
        [SerializeField] private GameObject m_Explode;

        [SerializeField] private DamageDealer m_DamageDealer = new DamageDealer();

        public void OnEnterState(ActorBase Actor)
        {
            m_RadiusExplode.SetId(Actor.name);
            m_DamageExplode.SetId(Actor.name);

            var explode = PoolFactory.Spawn(m_Explode, Actor.CenterPosition, Quaternion.identity)
                .GetComponent<Explosion2D>();
            explode.Owner = Actor;
            explode.DamageDealer.CopyData(m_DamageDealer);
            explode.DamageDealer.DamageSource.Value *= m_DamageExplode.FloatValue;

            explode.TargetLayer = Actor.EnemyLayerMask;
            explode.Radius = m_RadiusExplode.FloatValue;

            explode.StartExplosion();
        }
    }
}
