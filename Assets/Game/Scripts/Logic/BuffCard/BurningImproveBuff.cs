using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.GameActor.Buff
{
    public class BurningImproveBuff : AbstractBuff
    {
        public ValueConfigSearch Total ;
        public ValueConfigSearch Duration ;
        public ValueConfigSearch Tickrate ;
        public ValueConfigSearch DamageMultiply ;

        float duration;
        public EStatus StatusType;
        Effect.EffectAbstract effect;
        private void OnEnable()
        {
            count = 0;
            Messenger.AddListener<int, int>(EventKey.GameStart, OnGameStart);
            duration = Duration.FloatValue;
        }

       
        private void OnDisable()
        {
            Messenger.RemoveListener<int, int>(EventKey.GameStart, OnGameStart);
        }

        private void OnGameStart(int dungeon, int stage)
        {
            count = Total.IntValue;
            time = 0;
        }

        int count = 0;
        float time = 0;
        public override void OnUpdate(float dt)
        {
            base.OnUpdate(dt);
            if (count > 0 && Time.time-time>duration)
            {
                var spawner = Game.Controller.Instance.gameController.GetEnemySpawnHandler();
                foreach (var enemy in spawner.enemies)
                {
                    enemy.StatusEngine.AddStatus(Caster, StatusType, this).ContinueWith(status =>
                    {
                        status.SetDmgMul(DamageMultiply.FloatValue);
                        status.SetDuration(Duration.FloatValue);
                        status.SetCooldown(Tickrate.FloatValue);
                    }).Forget();
                }

                count--;
                time = Time.time;
            }
        }

        public override void Play()
        {
        }
       
    } 
}