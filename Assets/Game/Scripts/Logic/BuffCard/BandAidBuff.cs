using Game.GameActor.Buff;
using System;
using UnityEngine;

namespace Game.BuffCard
{
    public class BandAidBuff : AbstractBuff
    {
        private void OnEnable()
        {
            Messenger.AddListener<int,int>(EventKey.GameStart, OnGameStart);
        }
        private void OnDisable()
        {
            Messenger.RemoveListener<int, int>(EventKey.GameStart, OnGameStart);
            Logger.Log("DISABLE");
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            Logger.Log("OnDestroy");

        }
        private void OnGameStart(int dungeon,int stage)
        {
            var hpPercentHeal = GetValue(StatKey.Hp);
            var value = Caster.HealthHandler.GetMaxHP() * hpPercentHeal;
            Debug.Log("[Buff] Band Aid added after caster: " + value);

            Caster.Heal(value);
        }

        public override void Play()
        {

        }
    }
}