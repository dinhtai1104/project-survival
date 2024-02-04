using UnityEngine;


namespace AI.StateMachine
{
    public abstract class BaseState : ScriptableObject
    {
        public int id;
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (id == 0)
            {
                UnityEditor.EditorUtility.SetDirty(this);
                id = name.GetHashCode();
            }
        }
#endif
        public bool CompareTo(BaseState other)
        {
            return id == other.id;
        }

        public void Clone(BaseState original)
        {
            this.id = original.id;
            this.name = original.name;
        }
        public abstract BaseState Init(StateMachineHandler stateMachineHandler);
        public abstract void Execute(StateMachineHandler stateMachineHandler);
        public abstract void OnStart(StateMachineHandler stateMachineHandler);
        public abstract void OnEnd(StateMachineHandler stateMachineHandler);
    }
}