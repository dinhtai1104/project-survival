using com.sparkle.core;
using Core;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    [Service(typeof(ITaskRunnerMgrService))]
    public class TaskRunnerMgr : MonoBehaviour, ITaskRunnerMgrService
    {
        private List<TaskRunner> m_TaskRunners;
        private HashSet<int> m_Ids;

        public int Priority => 1;

        public bool Initialized { get; set; }

        public UniTask OnInitialize(IArchitecture architecture)
        {
            m_Ids = new HashSet<int>();
            m_TaskRunners = new List<TaskRunner>();
            Initialized = true;
            return UniTask.CompletedTask;
        }

        public void Subscribe(TaskRunner observer)
        {
            if (m_Ids.Contains(observer.GetInstanceID())) return;

            m_TaskRunners.Add(observer);
            m_Ids.Add(observer.GetInstanceID());
        }

        public void Unsubscribe(TaskRunner observer)
        {
            if (!m_Ids.Contains(observer.GetInstanceID())) return;

            m_TaskRunners.Remove(observer);
            m_Ids.Remove(observer.GetInstanceID());
        }

        public void OnUpdate()
        {
            for (int i = 0; i < m_TaskRunners.Count; ++i)
            {
                m_TaskRunners[i].Notify();
            }
        }
    }
}