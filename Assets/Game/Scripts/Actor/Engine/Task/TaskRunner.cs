using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public class TaskRunner : MonoBehaviour, IObserver
    {
        [SerializeField] private bool m_AutoStart;
        [SerializeField] private bool m_DestroyAfterCompletion;
        [SerializeField] private bool m_DeactiveAfterCompletion;
        [SerializeField] private Task m_StartingTask;
        [SerializeField] private Task m_CurrentTask;
        [SerializeField] private bool m_IsRunning;

        private Task[] m_Tasks;

        public bool DeactiveAfterCompletion
        {
            set { m_DeactiveAfterCompletion = value; }
            get { return m_DeactiveAfterCompletion; }
        }

        public bool DestroyAfterCompletion
        {
            set { m_DestroyAfterCompletion = value; }
            get { return m_DestroyAfterCompletion; }
        }

        public delegate void CompleteDelegate();

        public CompleteDelegate OnComplete;

        public bool IsRunning
        {
            get { return m_IsRunning; }
        }

        public Task CurrentTask
        {
            get { return m_CurrentTask; }
        }

        public Task[] Tasks
        {
            get { return m_Tasks; }
        }

        private void Awake()
        {
            m_Tasks = GetComponentsInChildren<Task>();
            if (m_Tasks.Length > 0)
            {
                foreach (var task in m_Tasks)
                {
                    task.SetupTaskRelationship();
                }

                m_StartingTask = m_Tasks[0];
            }
        }

        public void RunTask()
        {
            if (m_IsRunning || m_StartingTask == null) return;

            foreach (var task in m_Tasks)
            {
                task.IsCompleted = false;
                task.IsRunning = false;
            }

            m_CurrentTask = m_StartingTask;
            m_IsRunning = true;

            foreach (var task in m_CurrentTask.ParallelTasks)
            {
                task.Begin();
            }

            GameArchitecture.GetService<ITaskRunnerMgrService>().Subscribe(this);
        }

        public void StopTask()
        {
            if (!m_IsRunning) return;

            m_IsRunning = false;
            m_CurrentTask = null;

            foreach (var task in m_Tasks)
            {
                if (task.IsRunning)
                {
                    task.End();
                }

                task.IsCompleted = false;
                task.IsRunning = false;
                task.ForceInterruptTask = false;
            }

            OnComplete?.Invoke();

            GameArchitecture.GetService<ITaskRunnerMgrService>().Unsubscribe(this);

            if (m_DeactiveAfterCompletion)
            {
                gameObject.SetActive(false);
            }

            if (m_DestroyAfterCompletion)
            {
                Destroy(gameObject);
            }
        }

        public virtual void Init()
        {
        }

        public virtual void Clear()
        {
        }

        public virtual void Notify()
        {
            if (m_IsRunning)
            {
                // Current task complete
                if (m_CurrentTask.IsCompleted)
                {
                    // Then stop parallel tasks
                    for (int i = 0; i < m_CurrentTask.ParallelTasks.Length; ++i)
                    {
                        var task = m_CurrentTask.ParallelTasks[i];
                        if (!task.Ignore)
                        {
                            task.End();
                        }
                    }

                    // Prevent current task being null when gameobject was destroyed in End method
                    if (m_CurrentTask == null)
                    {
                        StopTask();
                        return;
                    }

                    // If there is next task then run it
                    if (m_CurrentTask.NextTask != null)
                    {
                        m_CurrentTask = m_CurrentTask.NextTask;

                        if (m_CurrentTask.Ignore)
                        {
                            if (m_CurrentTask.NextTask != null)
                            {
                                m_CurrentTask = m_CurrentTask.NextTask;
                                foreach (var task in m_CurrentTask.ParallelTasks)
                                {
                                    if (!task.Ignore)
                                    {
                                        task.Begin();
                                    }
                                }
                            }
                            else
                            {
                                StopTask();
                            }
                        }
                        else
                        {
                            foreach (var task in m_CurrentTask.ParallelTasks)
                            {
                                if (!task.Ignore)
                                {
                                    task.Begin();
                                }
                            }
                        }
                    }
                    // If there is no next task then stop
                    else
                    {
                        StopTask();
                    }
                }
                else
                {
                    if (m_CurrentTask != null)
                    {
                        foreach (var task in m_CurrentTask.ParallelTasks)
                        {
                            if (!task.Ignore)
                            {
                                task.Run();
                            }

                            if (task.ForceInterruptTask)
                            {
                                StopTask();
                            }
                        }
                    }
                }
            }
        }
    }
}