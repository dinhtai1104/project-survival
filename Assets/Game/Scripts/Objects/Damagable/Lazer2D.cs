using DG.Tweening;
using Engine;
using ExtensionKit;
using MoreMountains.Feedbacks;
using Pool;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Damagable
{
    public class Lazer2D : MonoBehaviour
    {
        private ActorBase m_Owner;
        [SerializeField] private DamageDealer m_DamageDealer;
        [SerializeField] private LayerMask m_TargetLayer;
        [SerializeField] private UnityEvent<ActorBase> m_HitTargetEvent;
        [SerializeField] private float m_DamageInterval = 0.15f;
        public LayerMask TargetLayer
        {
            set { m_TargetLayer = value; }
            get { return m_TargetLayer; }
        }
        public DamageDealer DamageDealer
        {
            set { m_DamageDealer = value; }
            get { return m_DamageDealer; }
        }

        [ShowInInspector, ReadOnly]
        public ActorBase Owner
        {
            set
            {
                m_Owner = value;
                if (m_DamageDealer != null)
                {
                    m_DamageDealer.Owner = value;
                }
            }
            get { return m_Owner; }
        }

        public float DamageInterval
        {
            get => m_DamageInterval;
            set => m_DamageInterval = value;
        }

        public float LazerLength { set; get; }

        private float m_DamageLastInvoke;
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            Action(other);
        }
        private void OnTriggerStay2D(Collider2D collision)
        {
            Action(collision);
        }

        protected void Action(Collider2D other)
        {
            if (m_TargetLayer.Contains(other.gameObject))
            {
                if (!other.TryGetComponent<ActorBase>(out var target)) return;
                if (Time.time - m_DamageLastInvoke < m_DamageInterval) return;
                if (m_DamageDealer != null)
                {
                    var hitResult = m_DamageDealer.DealDamage(Owner, target);

                    if (hitResult.Success)
                    {
                        OnImpact(target);
                        m_HitTargetEvent.Invoke(target);
                    }
                }
                m_DamageLastInvoke = Time.time;
            }
        }

        protected virtual void OnImpact(ActorBase target)
        {
        }

        public void StartLazer()
        {
            this.LocalScaleX(0);
            transform.DOScaleX(LazerLength, 0.2f);
        }
    }
}
