using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TaskRunner))]
    public class MultiTaskSkill : Skill
    {
        private TaskRunner m_TaskRunner;
        private IStopHandler[] m_StopHandlers;

        public Task CurrentTask
        {
            get { return m_TaskRunner.CurrentTask; }
        }

        public Task[] Tasks
        {
            get { return m_TaskRunner.Tasks; }
        }

        protected override void OnInit()
        {
            base.OnInit();
            m_StopHandlers = GetComponentsInChildren<IStopHandler>();
            m_TaskRunner = GetComponent<TaskRunner>();
            m_TaskRunner.OnComplete += OnComplete;
        }

        private void OnDestroy()
        {
            if (m_TaskRunner != null && m_TaskRunner.OnComplete != null)
            {
                m_TaskRunner.OnComplete -= OnComplete;
            }
        }

        protected override void OnCasting()
        {
            base.OnCasting();
            if (m_TaskRunner != null && !m_TaskRunner.IsRunning)
            {
                m_TaskRunner.RunTask();
            }
        }

        public override void Reset()
        {
            base.Reset();
            if (m_TaskRunner != null) m_TaskRunner.StopTask();
        }

        public override void Stop()
        {
            base.Stop();
            if (m_TaskRunner == null) return;

            if (m_StopHandlers != null)
            {
                foreach (var handler in m_StopHandlers)
                {
                    handler.OnStop();
                }
            }

            if (m_TaskRunner != null) m_TaskRunner.StopTask();
        }

        protected override void OnExit()
        {
            base.OnExit();
            if (m_TaskRunner != null)
            {
                m_TaskRunner.StopTask();
            }
        }

        private void OnComplete()
        {
            IsExecuting = false;
        }
    }
}