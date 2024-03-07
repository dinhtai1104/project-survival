using Common;
using ProgressBar;
using System;
using UnityEngine;
using UnityEngine.UI;
using Engine;
using Assets.Game.Scripts.GameScene.ControlView;
using Cysharp.Threading.Tasks;

namespace Assets.Game.Scripts.GameScene.Dungeon.Main
{
    public class UIDungeonMainPanel : GamePanel
    {
        [SerializeField] private Text m_WaveNameTxt;
        [SerializeField] private UIControlView m_ControlView;
        [SerializeField] private UITimerView m_TimerWaveLeft;
        [SerializeField] private UIHealthBar m_PlayerHealthBar;
        [SerializeField] private UIProgressBar m_PlayerExpBar;
        [SerializeField] private UIResourceView m_PickleResourceView;
        [SerializeField] private UIDungeonWaveIntroView m_WaveIntroView;
        private TimeSpan m_TimerCache;

        public override void PostInit()
        {
            base.PostInit();
            m_WaveIntroView.gameObject.SetActive(false);
        }

        public async UniTask StartWave(string title, int length, bool isFirstWaveEnterGame = false)
        {
            UpdateTimer(length);
            await m_WaveIntroView.Show(title);

            StartControl();
            m_WaveNameTxt.text = title;
        }

        public void UpdateTimer(long seconds)
        {
            m_TimerCache = TimeSpan.FromSeconds(seconds);
            m_TimerWaveLeft.SetTimer(m_TimerCache);
        }

        public void StopControl()
        {
            m_ControlView.gameObject.SetActive(false);
        }
        public void StartControl()
        {
            m_ControlView.gameObject.SetActive(true);
        }


        public UIHealthBar GetHealthPlayerBar() => m_PlayerHealthBar;
        public UIProgressBar GetExpPlayerBar() => m_PlayerExpBar;

        public void PauseOnClicked()
        {

        }
    }
}
