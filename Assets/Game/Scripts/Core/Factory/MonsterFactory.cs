using Assets.Game.Scripts.Core.Data.Database;
using com.assets.loader.addressables;
using com.assets.loader.core;
using Cysharp.Threading.Tasks;
using Engine;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Framework
{
    public class MonsterFactory
    {
        private readonly EnemyTable m_MonsterDatabase;
        private IAssetLoader m_AssetLoader;

        public MonsterFactory(EnemyTable monsterDatabase)
        {
            m_AssetLoader = new AddressableAssetLoader();
            m_MonsterDatabase = monsterDatabase;
        }

        public async UniTask<MonsterActor> CreateMonster(int id, int monsterLevel, Vector2 position, CancellationToken token = default)
        {
            var monsterData = m_MonsterDatabase.Get(id);
            var monsterPath = monsterData.Path;
            var monsterPrefab = await m_AssetLoader.LoadAsync<MonsterActor>(monsterPath).Task;

            return CreateMonster(monsterData, monsterPrefab, monsterLevel, position);
        }

        public MonsterActor CreateMonster(EnemyEntity monsterData, MonsterActor monsterPrefab, int monsterLevel, Vector2 position)
        {
            var monster = CreateBaseMonster(monsterPrefab, monsterData.Tags, position);
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

        private MonsterActor CreateBaseMonster(MonsterActor prefab, IEnumerable<string> tags, Vector3 position)
        {
            MonsterActor monster = PoolManager.Instance.Spawn(prefab, position, Quaternion.identity);
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
