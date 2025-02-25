using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TypeReferences;

namespace Engine
{
    [System.Serializable]
    public class BrainTransition
    {
        [SerializeField] private BrainDecision m_Decision;

        [SerializeField]
        [ClassExtends(typeof(IState))] private ClassTypeReference m_TrueState;

        [SerializeField]
        [ClassExtends(typeof(IState))] private ClassTypeReference m_FalseState;

        public BrainDecision Decision
        {
            get { return m_Decision; }
        }

        public System.Type TrueState
        {
            get { return m_TrueState.Type == null ? null : m_TrueState.Type; }
        }

        public System.Type FalseState
        {
            get { return m_FalseState.Type == null ? null : m_FalseState.Type; }
        }
    }
}