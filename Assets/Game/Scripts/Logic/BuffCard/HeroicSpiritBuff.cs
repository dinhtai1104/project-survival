using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.GameActor.Buff
{
    public class HeroicSpiritBuff : AbstractBuff
    {
        public ValueConfigSearch IncreaseRate,MaxRate;
        public AssetReferenceGameObject effectRef;
        Effect.EffectAbstract effect;
        private void OnEnable()
        {
            Messenger.AddListener<int, int>(EventKey.GameStart, OnGameStart);
            ActorBase.onDie += OnDie;
        }

       

        private void OnDisable()
        {
            Messenger.RemoveListener<int, int>(EventKey.GameStart, OnGameStart);
            ActorBase.onDie -= OnDie;
        }
        int totalEnemy = 0;

        private void OnGameStart(int dungeon, int stage)
        {
            Caster.Stats.RemoveModifiersFromSource(this);

            var spawner =Game.Controller.Instance.gameController.GetEnemySpawnHandler();
            totalEnemy = spawner.enemies.Count;
            float totalBonus = Mathf.Min(totalEnemy * IncreaseRate.FloatValue, MaxRate.FloatValue);

            Caster.Stats.AddModifier(StatKey.Dmg, new StatModifier(EStatMod.PercentAdd, totalBonus), this);

            Game.Pool.GameObjectSpawner.Instance.Get(effectRef.RuntimeKey.ToString(), obj =>
            {
                effect = obj.GetComponent<Effect.EffectAbstract>();
                effect.Active(Caster.GetTransform().position).SetParent(Caster.GetTransform());
            });

            Logger.Log("ADD DMG " + totalBonus);
        }
        private void OnDie(ActorBase target, ActorBase attacker)
        {
            if (target == Caster) return;
            if (totalEnemy <= 0) return;

            totalEnemy--;
            Caster.Stats.AddModifier(StatKey.Dmg, new StatModifier(EStatMod.PercentAdd, -IncreaseRate.FloatValue), this);
            Logger.Log("------- DMG " + (-IncreaseRate.FloatValue));

        }

        public override void Play()
        {
        }
       
    } 
}