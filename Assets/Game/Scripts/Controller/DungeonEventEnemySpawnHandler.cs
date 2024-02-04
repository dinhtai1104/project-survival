using BehaviorDesigner.Runtime.Tasks.Unity.UnityPlayerPrefs;
using com.mec;
using Cysharp.Threading.Tasks;
using Game.GameActor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Level
{
    public class DungeonEventEnemySpawnHandler : EnemySpawnHandler
    {
 
        public override UniTask<Character> SpawnSingle(string enemyId, int level, Vector2 position, float delay = 0, bool isStartBehaviourNow = true, bool usePortal = false, string vfx_spawn = "",int healthBarGroup=-1)
        {
            var data = GameController.Instance.GetDungeonEntity();
            var session = GameController.Instance.GetSession() as DungeonEventSessionSave;
            var config = DataManager.Base.DungeonEventConfig.Get(session.Type)[session.CurrentDungeon];
            var levelNew = config.EnemyLevel * level;

            return base.SpawnSingle(enemyId, Mathf.CeilToInt(levelNew), position, delay, isStartBehaviourNow, usePortal, vfx_spawn);
        }
    }
}