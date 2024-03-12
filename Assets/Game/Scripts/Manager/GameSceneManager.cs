using Core;
using Gameplay;
using SceneManger;
using UnityEngine;

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

        private void Start()
        {
            Application.targetFrameRate = 60;
        }
    }
}
