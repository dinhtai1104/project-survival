using Assets.Game.Scripts.Core.SceneMemory.Memory;
using Assets.Game.Scripts.Dungeon;
using com.sparkle.core;
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
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Gameplay
{
    public abstract class BaseGameplayScene : BaseSceneController<GameSceneManager>
    {
        [SerializeField] private TeamManager m_TeamManager;
        private GameplayLevelUpHandler m_GameplayLevelUpHandler;
        private WeaponFactory m_WeaponFactory;
        public TeamManager TeamManager => m_TeamManager;

        private ActorBase m_MainPlayer;
        protected PlayerGameplayData m_LocalPlayerData;
        private bool _checkingResult;
        protected bool EndGame => !_checkingResult;
        public PlayerActor MainPlayer => m_MainPlayer as PlayerActor;

        public WeaponFactory WeaponFactory { get => m_WeaponFactory; set => m_WeaponFactory = value; }
        public GameplayLevelUpHandler GameplayLevelUpHandler => m_GameplayLevelUpHandler;

        protected abstract bool CheckWinCondition();
        protected abstract bool CheckLoseCondition();
        protected abstract void OnWin();
        protected abstract void OnLose();
        protected override void OnEnter()
        {
            base.OnEnter();
            _checkingResult = true;
            CreateTeam();
            EquipWeapon();
        }
        /// <summary>
        /// Create Ally + Enemies Team ==== Create player
        /// </summary>
        private void CreateTeam()
        {
            m_TeamManager = new TeamManager();

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


            var pickupRange = PoolFactory.Spawn(GetRequestedAsset<GameObject>(StatKey.PickupRange), m_MainPlayer.transform).GetComponent<IPassive>();
            m_MainPlayer.Passive.AddPassive(pickupRange);

            var regeneration = PoolFactory.Spawn(GetRequestedAsset<GameObject>(StatKey.HpRegeneration), m_MainPlayer.transform).GetComponent<IPassive>();
            m_MainPlayer.Passive.AddPassive(regeneration);

        }
        private void EquipWeapon()
        {
            var listWea = new List<WeaponActor>();

            var smgEntity = DataManager.Base.Weapon.Get("8")[ERarity.Common];

            for (int i = 0; i < 1; i++)
            {
                // Spawn Weapon
                var weaponIns = m_WeaponFactory.CreateWeapon(smgEntity);
                listWea.Add(weaponIns);
            }
            (MainPlayer as PlayerActor).WeaponHolder.SetupWeapon(listWea);
        }
        private void SetupPlayerActor(ActorBase player)
        {
            player.Stats.Copy(SceneManager.PlayerData.PlayerStats);
            // Add Passive Here
        }
        public ActorBase SpawnPlayerActor()
        {
            var prefab = GetRequestedAsset<GameObject>("player").GetComponent<ActorBase>();
            var playerActor = PoolFactory.Spawn(prefab, Vector3.zero, Quaternion.identity);
            playerActor.Prepare();

            return playerActor;
        }

        public override UniTask RequestAssets()
        {
            var asynchorours = new List<UniTask>();
            WeaponFactory = new WeaponFactory(SceneManager, this);
            if (!GameArchitecture.GetService<IShortTermMemoryService>().RetrieveMemory<LocalPlayerMemory>(out var playerData))
            {
                playerData = new LocalPlayerMemory(PlayerGameplayData.CreateTest());
            }

            this.m_LocalPlayerData = playerData.LocalPlayerData;

            var playerPath = "Player/Player0.prefab";
            var taskPlayer = RequestAsset<GameObject>("player", playerPath);
            asynchorours.Add(taskPlayer);
            // Level
            var expLevel = DataManager.Base.LevelExpGameplay.Dictionary.Values.ToList();
            m_GameplayLevelUpHandler = new GameplayLevelUpHandler(new ExpHandler(0, expLevel));

            var pickupRange = AddressableName.StatPassive.AddParams(StatKey.PickupRange);
            asynchorours.Add(RequestAsset<GameObject>(StatKey.PickupRange, pickupRange));

            var regeneration = AddressableName.StatPassive.AddParams(StatKey.HpRegeneration);
            asynchorours.Add(RequestAsset<GameObject>(StatKey.HpRegeneration, regeneration));

            return UniTask.WhenAll(asynchorours);
        }

        protected virtual UniTask RequestPlayer(PlayerGameplayData localPlayerData)
        {
            // Request character
            var path = "Character/" + localPlayerData.Character;
            RequestAsset<ActorBase>("main-player", path);


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
