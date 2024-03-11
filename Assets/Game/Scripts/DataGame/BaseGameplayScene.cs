using Assets.Game.Scripts.Core.SceneMemory.Memory;
using Core;
using Cysharp.Threading.Tasks;
using Engine;
using Engine.Weapon;
using Framework;
using Manager;
using Pool;
using SceneManger;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay
{
    public abstract class BaseGameplayScene : BaseSceneController<GameSceneManager>
    {
        [SerializeField] private TeamManager m_TeamManager;
        private WeaponFactory m_WeaponFactory;
        public TeamManager TeamManager => m_TeamManager;

        private Actor m_MainPlayer;
        protected PlayerGameplayData m_LocalPlayerData;
        private bool _checkingResult;
        protected bool EndGame => !_checkingResult;
        public PlayerActor MainPlayer => m_MainPlayer as PlayerActor;

        public WeaponFactory WeaponFactory { get => m_WeaponFactory; set => m_WeaponFactory = value; }

        protected abstract bool CheckWinCondition();
        protected abstract bool CheckLoseCondition();
        protected abstract void OnWin();
        protected abstract void OnLose();
        protected override void OnEnter()
        {
            base.OnEnter();
            m_TeamManager = new TeamManager();
            _checkingResult = true;

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

            EquipWeapon();
        }

        private void EquipWeapon()
        {
            var listWea = new List<WeaponActor>();

            var smgEntity = DataManager.Base.Weapon.Get("1")[ERarity.Legendary];
            var prefab = GetRequestedAsset<GameObject>(smgEntity.IdEquipment).GetComponent<WeaponActor>();

            for (int i = 0; i < 2; i++)
            {
                // Spawn Weapon
                var weaponIns = PoolManager.Instance.Spawn(prefab);
                weaponIns.Prepare();
                weaponIns.Init(TeamManager.GetTeamModel(ConstantValue.PlayerTeamId));
                weaponIns.InitOwner(MainPlayer);

                // TODO:
                // Test -> Get Shotgun weapon
                weaponIns.InitWeapon(smgEntity);

                listWea.Add(weaponIns);
            }
            (MainPlayer as PlayerActor).WeaponHolder.SetupWeapon(listWea);
        }

        private void SetupPlayerActor(Actor player)
        {
            player.Stats.Copy(SceneManager.PlayerData.PlayerStats);
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
            WeaponFactory = new WeaponFactory(SceneManager, this);
            if (!Architecture.Get<ShortTermMemoryService>().RetrieveMemory<LocalPlayerMemory>(out var playerData))
            {
                playerData = new LocalPlayerMemory(PlayerGameplayData.CreateTest());
            }

            this.m_LocalPlayerData = playerData.LocalPlayerData;

            var playerPath = "Player/Player0.prefab";
            var taskPlayer = RequestAsset<GameObject>("player", playerPath);

            foreach (var weapon in DataManager.Base.Weapon.Dictionary)
            {
                var id = weapon.Value[ERarity.Common];
                RequestAsset<GameObject>(id.IdEquipment, id.PrefabPath);
            }

            //// Request weapon
            //var pathWp = "Weapon/{0}.prefab";
            //RequestAsset<GameObject>("weapon-0", pathWp.AddParams("Axe-2"));
            //RequestAsset<GameObject>("weapon-1", pathWp.AddParams("Axe"));
            //RequestAsset<GameObject>("weapon-2", pathWp.AddParams("Pistol"));
            //RequestAsset<GameObject>("weapon-3", pathWp.AddParams("AK"));
            //RequestAsset<GameObject>("weapon-4", pathWp.AddParams("AK"));
            //RequestAsset<GameObject>("weapon-5", pathWp.AddParams("SMG"));

            return taskPlayer;
        }

        protected virtual UniTask RequestPlayer(PlayerGameplayData localPlayerData)
        {
            // Request character
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
