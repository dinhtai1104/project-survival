using com.mec;
using Foundation.Game.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework
{
    public class TickSystem
    {
        private long m_TotalTime;
        private long m_CurrentTime;
        private Action<long> m_TickAction;
        private Action<long> m_TickComplete;
        private CoroutineHandle m_Coroutine;

        public float Percentage => m_CurrentTime * 1.0f / m_TotalTime;
        public bool IsCompleted => m_CurrentTime <= 0;
        public bool IsPausing => m_Coroutine.IsAliveAndPaused;
        public bool IsRunning => !IsCompleted && m_Coroutine.IsRunning;

        public void Start(long time, Action<long> tickAction, Action<long> tickComplete)
        {
            Timing.KillCoroutines(m_Coroutine);
            m_TotalTime = time;
            m_CurrentTime = time;
            m_TickAction = tickAction;
            m_TickComplete = tickComplete;

            m_Coroutine = Timing.RunCoroutine(_Tick());
        }

        public void Start(DateTime timeEnd, Action<long> tickAction, Action<long> tickComplete)
        {
            Timing.KillCoroutines(m_Coroutine);
            m_TotalTime = (long)(timeEnd - UnbiasedTime.UtcNow).TotalSeconds;
            m_TickAction = tickAction;
            m_TickComplete = tickComplete;

            m_Coroutine = Timing.RunCoroutine(_Tick());
        }

        private IEnumerator<float> _Tick()
        {
            m_CurrentTime = m_TotalTime;
            while (m_CurrentTime > 0)
            {
                m_TickAction?.Invoke(m_CurrentTime);
                m_CurrentTime--;
                yield return Timing.WaitForSeconds(1f);
            }
            m_TickAction?.Invoke(m_CurrentTime);
            m_TickComplete?.Invoke(m_CurrentTime);
        }

        public void Stop()
        {
            Timing.KillCoroutines(m_Coroutine);
        }
        public void Pause()
        {
            Timing.PauseCoroutines(m_Coroutine);
        }
        public void Resume()
        {
            Timing.ResumeCoroutines(m_Coroutine);
        }
    }
}
