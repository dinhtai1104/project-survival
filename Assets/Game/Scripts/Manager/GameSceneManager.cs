using Core;
using Cysharp.Threading.Tasks;
using Gameplay;
using SceneManger;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Manager
{
    public class GameSceneManager : BaseSceneManager
    {
        private PlayerData m_PlayerData;

        public PlayerData PlayerData { get => m_PlayerData; set => m_PlayerData = value; }
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            m_PlayerData = new PlayerData();
        }

        private async UniTaskVoid Start()
        {
            await Addressables.InitializeAsync();
            Application.targetFrameRate = 60;

            UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
            Input.simulateMouseWithTouches = true;
        }
    }
}
