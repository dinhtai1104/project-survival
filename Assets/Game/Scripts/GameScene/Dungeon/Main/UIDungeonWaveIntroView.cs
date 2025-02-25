using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Scripts.GameScene.Dungeon.Main
{
    public class UIDungeonWaveIntroView : MonoBehaviour
    {
        [SerializeField] private Text m_WaveIdTxt;
        [SerializeField] private UITweenRunner m_Runner;
        
        public async UniTask Show(string title, float duration = 2f)
        {
            this.m_WaveIdTxt.text = title;
            m_Runner.Transitions = GetComponentsInChildren<UITweenBase>();
            gameObject.SetActive(true);

            m_Runner.Show().Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(duration));
            Hide();
        }

        public void Hide()
        {
            m_Runner.Close().ContinueWith(()=>
            {
                gameObject.SetActive(false);
            }).Forget();
        }

    }
}
