using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public class Brain : MonoBehaviour, IBrain
    {
        [SerializeField] private float m_UpdateInterval = 0.1f;
        [SerializeField] private BrainSO m_Brain;

        private float m_AiTimer;

        public BrainTransition[] GlobalTransitions
        {
            get { return m_Brain.m_GlobalTransitions; }
        }

        public BrainLocalTransition[] LocalTransitions
        {
            get { return m_Brain.m_LocalTransitions; }
        }

        public Actor Owner { get; private set; }
        public bool Lock { get; set; }

        public void Init(Actor actor)
        {
            Owner = actor;
        }

        public void OnUpdate()
        {
            if (Lock) return;

            m_AiTimer += Time.deltaTime;
            if (!(m_AiTimer >= m_UpdateInterval))
            {
                return;
            }

            m_AiTimer = 0f;
            bool globalResult = false;
            if (m_Brain.m_GlobalTransitions != null)
            {
                globalResult = CheckGlobalTransitions();
            }

            if (globalResult || m_Brain.m_LocalTransitions == null || !Owner.AI)
            {
                return;
            }

            foreach (var local in m_Brain.m_LocalTransitions)
            {
                if (!Owner.Fsm.IsCurrentState(local.StateType)) continue;
                CheckLocalTransitions(local.Transitions);
                break;
            }
        }

        private bool CheckGlobalTransitions()
        {
            foreach (var transition in m_Brain.m_GlobalTransitions)
            {
                bool result = transition.Decision.Decide(Owner);

                if (result)
                {
                    if (transition.TrueState == null) continue;
                    if (!Owner.Fsm.IsCurrentState(transition.TrueState))
                    {
                        Owner.Fsm.ChangeState(transition.TrueState);
                    }

                    return true;
                }
                else
                {
                    if (transition.FalseState == null) continue;
                    if (!Owner.Fsm.IsCurrentState(transition.FalseState))
                    {
                        Owner.Fsm.ChangeState(transition.FalseState);
                    }

                    return false;
                }
            }

            return false;
        }

        private void CheckLocalTransitions(IEnumerable<BrainTransition> transitions)
        {
            foreach (var transition in transitions)
            {
                bool result = transition.Decision.Decide(Owner);
                if (result)
                {
                    if (transition.TrueState != null && !Owner.Fsm.IsCurrentState(transition.TrueState))
                    {
                        Owner.Fsm.ChangeState(transition.TrueState);
                    }
                }
                else
                {
                    if (transition.FalseState != null && !Owner.Fsm.IsCurrentState(transition.FalseState))
                    {
                        Owner.Fsm.ChangeState(transition.FalseState);
                    }
                }
            }
        }
    }
}