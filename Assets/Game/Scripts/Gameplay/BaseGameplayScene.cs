using Assets.Game.Scripts.Core.SceneMemory.Memory;
using Core;
using Cysharp.Threading.Tasks;
using Engine;
using Framework;
using Manager;
using SceneManger;
using System;
using UnityEngine;

namespace Gameplay
{
    public abstract class BaseGameplayScene : BaseSceneController<GameSceneManager>
    {
        [SerializeField] private TeamManager m_TeamManager;
        public TeamManager TeamManager => m_TeamManager;

        private Actor m_MainPlayer;
        protected PlayerGameplayData LocalPlayerData;
        private bool _checkingResult;
        protected bool EndGame => !_checkingResult;

        public Actor MainPlayer => m_MainPlayer;



        protected abstract bool CheckWinCondition();
        protected abstract bool CheckLoseCondition();
        protected abstract void OnWin();
        protected abstract void OnLose();

        protected override void OnEnter()
        {
            m_TeamManager = new TeamManager();
            base.OnEnter();

            //Create Team
            var playerTeam = new TeamModel(ConstantValue.PlayerTeamId, Layers.Player_Int);
            var enemyTeam = new TeamModel(ConstantValue.MonsterTeamId, Layers.Enemy_Int);

            // Create relationship
            playerTeam.AddEnemyTeam(enemyTeam);
            enemyTeam.AddEnemyTeam(playerTeam);

            m_TeamManager.AddTeam(playerTeam);
            m_TeamManager.AddTeam(enemyTeam);

            m_MainPlayer = SpawnPlayerActor();
            SetupPlayerActor(m_MainPlayer);
            m_MainPlayer.Init(TeamManager.GetTeamModel(ConstantValue.PlayerTeamId));


            m_TeamManager.AddActor(playerTeam.TeamId, m_MainPlayer);
        }

        private void SetupPlayerActor(Actor player)
        {
            player.Stat.Copy(SceneManager.PlayerData.PlayerStats);
            player.Movement.Speed = player.Stat.GetStat(StatKey.Speed);
        }

        public Actor SpawnPlayerActor()
        {
            var prefab = GetRequestedAsset<GameObject>("player").GetComponent<Actor>();
            var playerActor = PoolManager.Instance.Spawn(prefab, Vector3.zero, Quaternion.identity);
            playerActor.Prepare();

            return playerActor;
        }

        public override UniTask RequestAssets()
        {
            var playerPath = "Player/Player0.prefab";
            var taskPlayer = RequestAsset<GameObject>("player", playerPath);
            return taskPlayer;
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
