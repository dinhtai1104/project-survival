using Game.Fsm;
using Sirenix.OdinInspector;
using System;
using TypeReferences;
using UnityEngine;

namespace Game.AI
{
    [Serializable]
    public class BrainTransition
    {
        [SerializeField]
        private BrainDecision _decision;

        [SerializeField, ClassExtends(typeof(IState)), GUIColor("RGB(0, 1, 0)")]
        private ClassTypeReference _trueBranch;

        [SerializeField, ClassExtends(typeof(IState)), GUIColor("RGB(0.4, 0.5, 0)")]
        private ClassTypeReference _falseBranch;

        public BrainDecision Decision
        {
            get { return _decision; }
        }

        public Type TrueBranchState
        {
            get { return _trueBranch.Type == null ? null : _trueBranch.Type; }
        }

        public Type FalseBranchState
        {
            get { return _falseBranch.Type == null ? null : _falseBranch.Type; }
        }
    }
}