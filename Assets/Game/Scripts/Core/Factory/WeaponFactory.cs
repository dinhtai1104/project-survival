using Assets.Game.Scripts.Core.Data.Database.Equipment.Weapon;
using Cysharp.Threading.Tasks;
using Engine.Weapon;
using Gameplay;
using Manager;
using Pool;
using SceneManger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Framework
{
    public class WeaponFactory
    {
        private GameSceneManager SceneManager;
        private BaseGameplayScene GameplayScene;
        public WeaponFactory(GameSceneManager sceneManager, BaseGameplayScene gameplayScene)
        {
            SceneManager = sceneManager;
            GameplayScene = gameplayScene;
        }

        /// <summary>
        /// Create weapon
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async UniTask<WeaponActor> CreateWeapon(WeaponEntity entity)
        {
            var taskRequest = SceneManager.RequestAsync<GameObject>(entity.IdEquipment, $"{entity.PrefabPath}");
            await taskRequest;

            var prefab = SceneManager.GetAsset<GameObject>(entity.IdEquipment);
            var weaponActor = PoolManager.Instance.Spawn(prefab.GetComponent<WeaponActor>());
            var mainPlayer = GameplayScene.MainPlayer;

            weaponActor.Init(mainPlayer.TeamModel);
            weaponActor.InitWeapon(entity);
            weaponActor.InitOwner(mainPlayer);


            return weaponActor;
        }
    }
}
