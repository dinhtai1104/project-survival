using UnityEngine;

namespace SceneManger.Transition
{
    public abstract class BaseSceneTransition : MonoBehaviour
    {
        private bool _isDone;

        public bool IsDone
        {
            protected set
            {
                // Only call method once
                if (!_isDone && value)
                    EndTransition();

                _isDone = value;
            }
            get { return _isDone; }
        }

        public float Progress { protected set; get; }

        public virtual void StartTransition()
        {
            _isDone = false;
            DontDestroyOnLoad(this);
        }

        protected virtual void EndTransition()
        {
        }
    }
}