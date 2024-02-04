using Game.Fsm;
using Sirenix.OdinInspector;
using TypeReferences;
using UnityEngine;

namespace Game.AI
{
    [System.Serializable]
    public class BrainNeuronTransition
    {
        [SerializeField, ClassExtends(typeof(IState))]
        private ClassTypeReference _state;

        [SerializeField]
        private BrainTransition[] _transitions;

        public System.Type StateType { get { return _state.Type; } }
        public BrainTransition[] Transitions { get { return _transitions; } }
    }
}