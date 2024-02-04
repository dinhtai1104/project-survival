using com.mec;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Tasks
{
    public class Task : MonoBehaviour
    {
        public bool IsIgnore = false;
        [SerializeField]
        private bool _waitForParallelTasksCompletion;

        [SerializeField]
        private bool _completeWhenAllChildrenStop;

        [SerializeField]
        private Task _nextTask;

        [SerializeField]
        private Task[] _parallelTasks;

        public Task NextTask { set { _nextTask = value; } get { return _nextTask; } }
        public Task[] ParallelTasks { protected set { _parallelTasks = value; } get { return _parallelTasks; } }
        public bool CompleteWhenAllChildrenStop { protected set { _completeWhenAllChildrenStop = value; } get { return _completeWhenAllChildrenStop; } }
        public bool IsRunning { set; get; }
        public bool ForceInterruptTask { set; get; }

        public bool IsCompleted { set; get; } = false;

        public virtual void UnityEnable() { }
        public virtual void UnityDisable() { }
        public virtual void UnityAwake() { }
        public virtual void UnityStart() { }

        public async void BeginEvent()
        {
            await Begin();
        }

        public virtual async UniTask Begin()
        {
            IsCompleted = false;
            IsRunning = true;
            await UniTask.Yield();
        }

        public virtual async UniTask End()
        {
            IsCompleted = true;
            IsRunning = false;
            Timing.KillCoroutines(gameObject);
            await UniTask.Yield();
        }

        public virtual void Run()
        {
            if (IsCompleted || !IsRunning) return;
            if (_completeWhenAllChildrenStop)
            {
                bool childrenComplete = true;
                foreach (var parallelTask in _parallelTasks)
                {
                    if (parallelTask.GetInstanceID() != GetInstanceID() && !parallelTask.IsCompleted)
                    {
                        childrenComplete = false;
                        break;
                    }
                }

                if (childrenComplete)
                    IsCompleted = true;
            }
        }

        public virtual void SetupTaskRelationship()
        {
            // Find next task
            int siblingIndex = transform.GetSiblingIndex();

            if (siblingIndex != transform.parent.childCount - 1)
            {
                _nextTask = transform.parent.GetChild(siblingIndex + 1).GetComponent<Task>();
            }

            // Find parallel tasks
            _parallelTasks = GetComponentsInChildren<Task>();
        }

        public virtual bool HasTask()
        {
            return true;
        }

        [Button]
        private void CompleteNow()
        {
            IsCompleted = true;
        }
    }
}