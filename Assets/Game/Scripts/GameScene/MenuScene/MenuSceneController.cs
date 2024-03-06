using Cysharp.Threading.Tasks;
using Manager;
using MenuScene.Main;
using SceneManger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ui.View;
using UnityEngine;

namespace MenuScene
{
    public class MenuSceneController : BaseSceneController<GameSceneManager>
    {
        public override UniTask RequestAssets()
        {
            // Nothing to load
            var asynchronous = new List<UniTask>();
            // Load view loading

            //var taskLoading = RequestAsset<GameObject>("loading_view", AddressableName.UILoadingView);
            //asynchronous.Add(taskLoading);

            return UniTask.WhenAll(asynchronous);
        }
        protected override void OnEnter()
        {
            base.OnEnter();
            PanelManager.CreateAsync<UIMainMenuPanel>(AddressableName.UIMainMenuPanel)
                .ContinueWith(panel => panel.Show()).Forget();
        }
        public async override UniTask Exit(bool reload)
        {
            await PanelManager.Instance.CloseAll();
            base.Exit(reload).Forget();
        }
    }
}
