using Assets.Game.Scripts.Core.Data.Database;
using com.assets.loader.addressables;
using com.assets.loader.core;
using Cysharp.Threading.Tasks;
using Engine;
using Manager;
using SceneManger;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Framework
{
    public class EnemyFactory
    {
        private readonly EnemyTable m_EnemyDatabase;
        private IAssetLoader m_AssetLoader;

        public EnemyFactory(EnemyTable monsterDatabase)
        {
            m_AssetLoader = new AddressableAssetLoader();
            m_EnemyDatabase = monsterDatabase;
        }

        public async UniTask<EnemyActor> CreateEnemy(string id, int enemyLevel, Vector2 position, CancellationToken token = default)
        {
            var monsterData = m_EnemyDatabase.Get(id);
            var monsterPath = monsterData.Path;
            var monsterPrefab = await m_AssetLoader.LoadAsync<EnemyActor>(monsterPath).Task;

            return CreateEnemy(monsterData, monsterPrefab, enemyLevel, position);
        }

        public EnemyActor CreateEnemy(EnemyEntity monsterData, EnemyActor monsterPrefab, int monsterLevel, Vector2 position)
        {
            var monster = CreateBaseEnemy(monsterPrefab, monsterData.Tags, position);
            monster.MonsterData = monsterData;
            monster.MonsterLevel = monsterLevel;

            // apply monster stat
            var stats = monster.Stat;
            stats.SetBaseValue(StatKey.Hp, monsterData.Hp);
            stats.SetBaseValue(StatKey.Damage, monsterData.Damage);
            stats.SetBaseValue(StatKey.Speed, monsterData.Speed);
            stats.SetBaseValue(StatKey.AttackSpeed, monsterData.AttackSpeed);

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
            EnemyActor monster = PoolManager.Instance.Spawn(prefab, position, Quaternion.identity);
            monster.AI = true;

            var stats = monster.Stat;
            stats.AddStat(StatKey.Hp, 1f);
            stats.AddStat(StatKey.Damage, 0f);
            stats.AddStat(StatKey.MovementSpeed, 0f, 0.3f);
            stats.AddStat(StatKey.AttackSpeed, 1f, 0.3f);

            foreach (var tag in tags)
            {
                monster.Tagger.AddTag(tag);
            }

            return monster;
        }
    }
}
