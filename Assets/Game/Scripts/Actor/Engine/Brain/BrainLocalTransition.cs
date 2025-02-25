using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TypeReferences;

namespace Engine
{
    [System.Serializable]
    public class BrainLocalTransition
    {
        [SerializeField]
        [ClassExtends(typeof(IState))]
        private ClassTypeReference m_State;

        [SerializeField] private BrainTransition[] m_Transitions;

        public System.Type StateType
        {
            get { return m_State.Type; }
        }

        public BrainTransition[] Transitions
        {
            get { return m_Transitions; }
        }
    }
}