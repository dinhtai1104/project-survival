using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.GameActor.Buff
{
    public class TimeSkipBuff : AbstractBuff
    {
        public ValueConfigSearch SkipChance;
        public AssetReferenceGameObject effectRef;
        private void OnEnable()
        {
            Messenger.AddListener<ActorBase, int, int>(EventKey.OnCastSkill, OnCastSkill);
        }

        private void OnCastSkill(ActorBase caster, int totalCast, int maxCast)
        {
            if (caster.GetCharacterType() == ECharacterType.Drone)
            {
                if (UnityEngine.Random.Range(0f, 1f) < SkipChance.FloatValue)
                {
                    ResetCooldown(caster);
                }
            }
        }
        async UniTask ResetCooldown(ActorBase caster)
        { 
            await UniTask.Delay(250);
            await UniTask.WaitUntil(() => !caster.SkillEngine.GetSkill(0).IsExecuting);
            //restore cooldown
            caster.SkillEngine.GetSkill(0).SetCoolDown(caster.SkillEngine.GetSkill(0).GetCoolDown() - 1f);
            Game.Pool.GameObjectSpawner.Instance.Get(effectRef.RuntimeKey.ToString(), obj =>
            {
                obj.GetComponent<Effect.EffectAbstract>().Active(caster.GetMidTransform().position).SetParent(caster.GetTransform());
            });
        }

        private void OnDisable()
        {
            Messenger.RemoveListener<ActorBase, int, int>(EventKey.OnCastSkill, OnCastSkill);

        }
        public override void Play()
        {
        }
       
    } 
}