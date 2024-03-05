using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UnityEngine.InputSystem.XInput.XInputController;
using UnityEngine.AddressableAssets;
using UnityEngine;

namespace SceneManger.Loading
{
    public class SimpleLoadingScene : BaseLoadingScene
    {
        [SerializeField] private ProgressBar.UIProgressBar _progressBar;

        public override void StartLoading(BaseSceneManager sceneManager)
        {
            base.StartLoading(sceneManager);
            GameSceneManager gameSceneManager = (GameSceneManager)sceneManager;
            _progressBar.gameObject.SetActive(false);

            _progressBar.SetValue(0f);
        }

        public override void EndLoading()
        {
            base.EndLoading();
            _progressBar.SetValue(1f);
            
            PoolManager.Instance.Despawn(gameObject);
        }

        public override void LoadingProgress(float percentage)
        {
            _progressBar.SetValue(percentage);
        }
    }
}
