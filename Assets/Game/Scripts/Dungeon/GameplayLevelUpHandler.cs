using Common;
using Cysharp.Threading.Tasks;
using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Dungeon
{
    [System.Serializable]
    public class GameplayLevelUpHandler : IDisposable
    {
        private ExpHandler m_ExpHandler;
        private UIExpBar m_ExpBar;
        private int LevelUpInWave = 0;

        public GameplayLevelUpHandler(ExpHandler expHandler)
        {
            m_ExpHandler = expHandler;
        }

        public void RegisterUI(UIExpBar bar)
        {
            m_ExpBar = bar;
        }

        private void OnExpChanged(long obj)
        {
            if (m_ExpBar)
            {
                m_ExpBar.SetValue(m_ExpHandler.LevelProgress);
            }
        }

        public void StartTracking()
        {
            m_ExpHandler.OnLevelChanged += OnLevelUp;
            m_ExpHandler.OnExpChanged += OnExpChanged;

            if (m_ExpBar)
            {
                m_ExpBar.SetValue(0);
                m_ExpBar.SetLevel(m_ExpHandler.CurrentLevel);
            }
        }

        private void OnLevelUp(int from, int to)
        {
            Debug.Log($"Level Up From {from} to {to}");
            LevelUpInWave++;
            if (m_ExpBar)
            {
                m_ExpBar.SetLevel(m_ExpHandler.CurrentLevel);
                m_ExpBar.SetValue(m_ExpHandler.LevelProgress);
            }
        }

        public void Dispose()
        {
            m_ExpHandler.OnLevelChanged -= OnLevelUp;
            m_ExpHandler.OnExpChanged -= OnExpChanged;
        }

        public int GetLevelInWave() => LevelUpInWave;
        public void Reset() => LevelUpInWave = 0;

        public void AddExp(int exp)
        {
            m_ExpHandler.Add(exp);
        }
    }
}
