using Assets.Game.Scripts.Core.Data.Database;
using com.assets.loader.addressables;
using com.assets.loader.core;
using Cysharp.Threading.Tasks;
using Engine;
using Manager;
using Pool;
using SceneManger;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Framework
{
    public class EnemyFactory
    {
        private readonly EnemyTable m_EnemyDatabase;
        private BaseSceneManager m_GameScene;


        public EnemyFactory(EnemyTable monsterDatabase)
        {
            m_EnemyDatabase = monsterDatabase;
            m_GameScene = GameSceneManager.Instance;
        }

        public EnemyActor CreateEnemy(string id, int enemyLevel, Vector2 position, CancellationToken token = default)
        {
            var monsterData = m_EnemyDatabase.Get(id);
            var monsterPath = monsterData.Path;
            //var monsterPrefab = await ResourcesLoader.Instance.LoadAsync<GameObject>(monsterPath);
            var monsterPrefab = m_GameScene.GetAsset<GameObject>(id);

            return CreateEnemy(monsterData, monsterPrefab.GetComponent<EnemyActor>(), enemyLevel, position);
        }

        public EnemyActor CreateEnemy(EnemyEntity monsterData, EnemyActor monsterPrefab, int monsterLevel, Vector2 position)
        {
            var monster = CreateBaseEnemy(monsterPrefab, monsterData.Tags, position);
            monster.Prepare();
            monster.EntityData = monsterData;
            monster.MonsterLevel = monsterLevel;

            // apply monster stat
            var stats = monster.Stats;
            stats.RemoveAllStats();
            stats.AddStat(StatKey.Hp, monsterData.Hp);
            stats.AddStat(StatKey.Damage, monsterData.Damage);
            stats.AddStat(StatKey.Speed, monsterData.Speed);
            stats.AddStat(StatKey.AttackSpeed, monsterData.AttackSpeed);
            stats.AddStat(StatKey.AttackRange, monsterData.AttackRange);

            // Apply level stat
            stats.AddModifier(StatKey.Hp, new StatModifier(EStatMod.Flat, monsterData.iHp * monsterLevel), this);
            stats.AddModifier(StatKey.Damage, new StatModifier(EStatMod.Flat, monsterData.iDamage * monsterLevel), this);
            stats.AddModifier(StatKey.Speed, new StatModifier(EStatMod.Flat, monsterData.iSpeed * monsterLevel), this);
            stats.AddModifier(StatKey.AttackSpeed, new StatModifier(EStatMod.Flat, monsterData.iAttackSpeed * monsterLevel), this);

            stats.CalculateStats();

            return monster;
        }

        private EnemyActor CreateBaseEnemy(EnemyActor prefab, IEnumerable<string> tags, Vector3 position)
        {
            EnemyActor monster = PoolFactory.Spawn(prefab, position, Quaternion.identity);
            monster.AI = true;
            foreach (var tag in tags)
            {
                if (monster.Tagger.HasTag(tag) == false)
                {
                    monster.Tagger.AddTag(tag);
                }
            }

            return monster;
        }
    }
}
