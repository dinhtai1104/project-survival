using Engine;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AIState
{
    public class ActorRandomMovementState : BaseState
    {
        [SerializeField] private string m_RunAnimation = "move";
        [SerializeField] private string m_IdleAnimation = "idle";

        [Header("Use Bound Movement")]
        public bool m_Global = true;

        [Header("Bound custom")]
        [SerializeField, ShowIf("m_Global", true)] private Bound2D m_Bound;
        [SerializeField] private float m_RestDuration = 0.5f;
        [SerializeField] private float m_MaxMovingDuration = 3f;

        private Vector3 m_Destination = Vector3.zero;
        private float m_RestTimer;
        private float m_MovingTimer; 
        private bool m_ReachTarget;

        public bool ReachTaget { get { return m_ReachTarget; } }
        public bool IsMovingDurationEnd { set; get; }

        public override void Enter()
        {
            base.Enter();
            m_ReachTarget = false;
            m_RestTimer = 0f;
            m_MovingTimer = 0f;
            m_Destination = RandomDestination();
        }

        public override void Execute()
        {
            base.Execute();
            m_MovingTimer += Time.deltaTime;

            if (m_MovingTimer >= m_MaxMovingDuration)
            {
                m_MovingTimer = 0f;
                IsMovingDurationEnd = true;
            }


            if (!m_ReachTarget)
            {
                if (!IsMovingDurationEnd)
                {
                    Actor.Animation.EnsurePlay(0, m_RunAnimation, true);
                    Actor.Movement.MoveTo(m_Destination);

                    // If actor have reached destination
                    if (Actor.BotPosition.Compare(m_Destination))
                        m_ReachTarget = true;
                }
                else
                {
                    m_ReachTarget = true;
                }
            }
            else
            {
                m_RestTimer += Time.deltaTime;
                Actor.Animation.EnsurePlay(0, m_IdleAnimation, true);

                if (m_RestTimer >= m_RestDuration)
                {
                    IsMovingDurationEnd = false;
                    m_RestTimer = 0f;
                    m_ReachTarget = false;
                    m_Destination = RandomDestination();
                }
            }
        }

        public override void Exit()
        {
            base.Exit();
            m_ReachTarget = true;
            m_RestTimer = 0f;
            m_MovingTimer = 0f;
            IsMovingDurationEnd = false;
            Actor.Animation.EnsurePlay(0, m_IdleAnimation, true);
        }

        private Vector3 RandomDestination()
        {
            Vector3 dest = Actor.BotPosition;

            if (m_Global)
                dest = MathUtils.RandomPointInBound2D(Actor.Movement.MovementBound);
            else
                dest = MathUtils.Bound2D(Actor.BotPosition + MathUtils.RandomPointInBound2D(m_Bound), Actor.Movement.MovementBound, Actor.Width, -Actor.Width, Actor.Height, -Actor.Height);

            if (Actor.Movement.MovementBound.Contains(dest))
                return dest;

            return Actor.BotPosition;
        }
    }
}
