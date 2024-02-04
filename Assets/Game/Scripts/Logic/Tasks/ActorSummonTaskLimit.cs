using Cysharp.Threading.Tasks;
using Game.GameActor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Logic.Tasks
{
    public class ActorSummonTaskLimit : ActorSummonTask
    {
        public List<ActorBase> miniEnemies = new List<ActorBase>();
        public ValueConfigSearch maxMiniEnemies;
        public string vfxSpawn = "";

        public override void UnityAwake()
        {
            base.UnityAwake();
            Caster.onSelfDie += OnDie;
        }

        private void OnDie(ActorBase current)
        {
            Caster.onSelfDie -= OnDie;
            foreach (var enemies in miniEnemies)
            {
                enemies.DeadForce();
            }
            miniEnemies.Clear();
        }

        public async override UniTask Begin()
        {
            maxMiniEnemies.SetId(Caster.gameObject.name);
            if (miniEnemies.Count >= maxMiniEnemies.IntValue)
            {
                this.Skill.Stop();
                return;
            }
            await base.Begin();
        }

        protected override void Spawn(Vector3 pos)
        {
            var enemyhandler = GameController.Instance.GetEnemySpawnHandler();
            enemyhandler.SpawnSingle(enemyId, (int)Caster.GetStatValue(StatKey.Level), pos, usePortal: true, vfx_spawn: vfxSpawn)
                .ContinueWith(enemyMini =>
                {
                    miniEnemies.Add(enemyMini);
                    enemyMini.onSelfDie += MiniDie;
                }).Forget();
        }

        private void MiniDie(ActorBase current)
        {
            miniEnemies.Remove(current);
            current.onSelfDie -= MiniDie;
        }
    }
}
