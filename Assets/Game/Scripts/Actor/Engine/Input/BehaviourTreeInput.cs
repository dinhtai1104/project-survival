using BehaviorDesigner.Runtime;
using Mosframe;
using UnityEngine;

namespace Engine
{
    public class BehaviourTreeInput : BaseInputHandler
    {
        [SerializeField] private BehaviorTree _behaviorTree;

        public override void Init(Actor actor)
        {
            base.Init(actor);
            _behaviorTree.SetVariableValue("Actor", actor);
        }


        public void SetVariableToTree(string id, object value)
        {
            _behaviorTree.SetVariableValue(id, value);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            _behaviorTree.EnableBehavior();
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            _behaviorTree.DisableBehavior();
        }
    }
}