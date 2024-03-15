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

        public BrainTransition[] CoreTransitions
        {
            get { return m_Brain.m_CoreTransitions; }
        }

        public BrainTransition[] GlobalTransitions
        {
            get { return m_Brain.m_GlobalTransitions; }
        }


        public BrainLocalTransition[] LocalTransitions
        {
            get { return m_Brain.m_LocalTransitions; }
        }

        public ActorBase Owner { get; private set; }
        public bool Lock { get; set; }

        public void Init(ActorBase actor)
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
            bool coreResult = false;
            if (CoreTransitions != null)
            {
                coreResult = CheckGlobalTransitions(CoreTransitions);
            }
            
            if (!coreResult && Owner.AI)
            {
                bool globalResult = false;
                if (GlobalTransitions != null)
                {
                    globalResult = CheckGlobalTransitions(GlobalTransitions);
                }

                if (globalResult || LocalTransitions == null)
                {
                    return;
                }

                foreach (var local in LocalTransitions)
                {
                    if (!Owner.Fsm.IsCurrentState(local.StateType)) continue;
                    CheckLocalTransitions(local.Transitions);
                    break;
                }
            }
        }

        private bool CheckGlobalTransitions(BrainTransition[] globalTransitions)
        {
            foreach (var transition in globalTransitions)
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