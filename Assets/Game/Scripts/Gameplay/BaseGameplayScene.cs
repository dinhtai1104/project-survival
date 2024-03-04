using Assets.Game.Scripts.Core.SceneMemory.Memory;
using Core;
using Cysharp.Threading.Tasks;
using Engine;
using Manager;
using SceneManger;
using System;

namespace Gameplay
{
    public abstract class BaseGameplayScene : BaseSceneController<GameSceneManager>
    {
        protected PlayerGameplayData LocalPlayerData;
        private bool _checkingResult;
        protected bool EndGame => !_checkingResult;


        protected abstract bool CheckWinCondition();
        protected abstract bool CheckLoseCondition();
        protected abstract void OnWin();
        protected abstract void OnLose();


        public override UniTask RequestAssets()
        {
            LocalPlayerData = Architecture.Get<ShortTermMemoryService>().RetrieveMemory<LocalPlayerMemory>().LocalPlayerData;
            return RequestPlayer(LocalPlayerData);
        }

        protected virtual UniTask RequestPlayer(PlayerGameplayData localPlayerData)
        {
            var path = "Character/" + localPlayerData.Character;
            RequestAsset<Actor>("main-player", path);
            return UniTask.CompletedTask;
        }

        public override void Execute()
        {
            base.Execute();

            if (_checkingResult)
            {
                if (CheckWinCondition())
                {
                    OnWin();
                    _checkingResult = false;
                }

                if (CheckLoseCondition())
                {
                    OnLose();
                    _checkingResult = false;
                }

                if (EndGame) OnEndGame();
            }
        }

        protected virtual void OnEndGame()
        {
        }
    }
}
