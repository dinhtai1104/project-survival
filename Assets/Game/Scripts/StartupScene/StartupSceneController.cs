using Assets.Game.Scripts.Adventure;
using Core;
using Cysharp.Threading.Tasks;
using Manager;
using SceneManger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.StartupScene
{
    public class StartupSceneController : BaseSceneController<GameSceneManager>
    {
        public override async UniTask RequestAssets()
        {
            await GameArchitecture.Instance.Init();
        }

        private void Start()
        {
            FindObjectOfType<GameSceneManager>()?.InitGame();
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            LoadingScene();
        }

        private void LoadingScene()
        {
            SceneManager.LoadSceneAsync<AdventureSceneController>();
        }
    }
}
