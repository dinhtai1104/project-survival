using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.GameActor.Buff
{
    public class AllInBuff : AbstractBuff
    {
        public ValueConfigSearch Rate;
        Effect.EffectAbstract effect;
        private void OnEnable()
        {
            Messenger.AddListener<int, int>(EventKey.GameStart, OnGameStart);
            Messenger.AddListener<ActorBase, bool, int>(EventKey.ActorSpawn, OnSpawn);
        }

       
        private void OnDisable()
        {
            Messenger.RemoveListener<int, int>(EventKey.GameStart, OnGameStart);
            Messenger.RemoveListener<ActorBase, bool, int>(EventKey.ActorSpawn, OnSpawn);
        }

        private void OnGameStart(int dungeon, int stage)
        {
           
        }
        private void OnSpawn(ActorBase actor, bool active, int group)
        {

            if(actor.GetCharacterType()==ECharacterType.Enemy || actor.GetCharacterType() == ECharacterType.Boss)
            {
                actor.HealthHandler.SetHealth(actor.HealthHandler.GetMaxHP() / 2f);
            }
        }


        public override void Play()
        {

            Caster.HealthHandler.SetHealth(Caster.HealthHandler.GetHealth()* Rate.FloatValue);

            Caster.Stats.AddModifier(StatKey.Hp, new StatModifier(EStatMod.PercentAdd, -Rate.FloatValue), this);

        }
       
    } 
}