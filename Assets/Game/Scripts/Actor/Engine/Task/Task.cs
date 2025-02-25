using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public class Task : MonoBehaviour
    {
        [SerializeField] private bool m_ManuallySetUpRelation;
        [SerializeField] private bool m_Ignore;
        [SerializeField] private bool m_Standalone;
        [SerializeField] private bool m_IsCompleted;
        [SerializeField] private bool m_CompleteWhenAllChildrenStop;
        [SerializeField] private Task m_NextTask;
        [SerializeField] private Task[] m_ParallelTasks;

        public bool IsStandalone
        {
            get { return m_Standalone; }
        }

        public Task NextTask
        {
            set { m_NextTask = value; }
            get { return m_NextTask; }
        }

        public Task[] ParallelTasks
        {
            protected set { m_ParallelTasks = value; }
            get { return m_ParallelTasks; }
        }

        public bool CompleteWhenAllChildrenStop
        {
            protected set { m_CompleteWhenAllChildrenStop = value; }
            get { return m_CompleteWhenAllChildrenStop; }
        }

        public bool IsRunning { set; get; }

        public bool Ignore
        {
            set { m_Ignore = value; }
            get { return m_Ignore; }
        }

        public bool ManuallySetUpRelation
        {
            set { m_ManuallySetUpRelation = value; }
            get { return m_ManuallySetUpRelation; }
        }

        public bool ForceInterruptTask { set; get; }

        public bool IsCompleted
        {
            get { return m_IsCompleted; }

            set { m_IsCompleted = value; }
        }

        protected virtual void Awake()
        {
        }

        protected virtual void Start()
        {
        }

        protected virtual void OnDestroy()
        {
        }

        public virtual void Begin()
        {
            if (m_Ignore)
            {
                IsCompleted = true;
                IsRunning = false;
            }
            else
            {
                IsCompleted = false;
                IsRunning = true;
            }
        }

        public virtual void End()
        {
            if (m_Ignore) return;

            IsCompleted = true;
            IsRunning = false;
        }

        public virtual void Run()
        {
            if (m_Ignore || m_IsCompleted) return;

            if (m_CompleteWhenAllChildrenStop)
            {
                bool childrenComplete = true;
                foreach (var parallelTask in m_ParallelTasks)
                {
                    if (parallelTask.GetInstanceID() != GetInstanceID() && !parallelTask.IsCompleted)
                    {
                        childrenComplete = false;
                        break;
                    }
                }

                if (childrenComplete) IsCompleted = true;
            }
        }

        public virtual void SetupTaskRelationship()
        {
            if (m_ManuallySetUpRelation || m_Standalone)
            {
                return;
            }

            int siblingIndex = transform.GetSiblingIndex();
            if (siblingIndex != transform.parent.childCount - 1)
            {
                m_NextTask = transform.parent.GetChild(siblingIndex + 1).GetComponent<Task>();
            }

            m_ParallelTasks = GetComponentsInChildren<Task>();
        }
    }
}