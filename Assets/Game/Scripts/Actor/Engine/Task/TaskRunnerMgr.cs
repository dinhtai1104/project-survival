using Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public class TaskRunnerMgr : Service
    {
        private List<TaskRunner> m_TaskRunners;
        private HashSet<int> m_Ids;

        public override void OnInit()
        {
            base.OnInit();
            m_Ids = new HashSet<int>();
            m_TaskRunners = new List<TaskRunner>(30);
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

        public override void OnUpdate()
        {
            for (int i = 0; i < m_TaskRunners.Count; ++i)
            {
                m_TaskRunners[i].Notify();
            }
        }

        public override void OnDispose()
        {
            base.OnDispose();
            m_Ids.Clear();
            m_TaskRunners.Clear();
        }
    }
}