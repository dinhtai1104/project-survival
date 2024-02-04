using Cysharp.Threading.Tasks;
using ExtensionKit;
using Game.GameActor;
using Game.Pool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Handler
{
    public class EffectHandler : MonoBehaviour
    {
        [SerializeField]
        private string normalGetHitKey = "VFX_GetHit";
        private void OnEnable()
        {
            Messenger.AddListener<DamageSource, IDamageDealer>(EventKey.GetHit, OnGetHit);
            Messenger.AddListener<ECharacterType,Vector2>(EventKey.ActorSpawnPortal, OnActorSpawn);
            Messenger.AddListener<ECharacterType,Vector2, string>(EventKey.ActorSpawnPortalWithCustomVFX, OnActorSpawn);

        }

        private void OnActorSpawn(ECharacterType characterType, Vector2 position, string vfx)
        {
            if (characterType == ECharacterType.Enemy)
            {
                Pool.GameObjectSpawner.Instance.Get(vfx, res =>
                {
                    res.GetComponent<Effect.EffectAbstract>().Active(position);
                });
            }
        }

        private void OnDisable()
        {
            Messenger.RemoveListener<DamageSource, IDamageDealer>(EventKey.GetHit, OnGetHit);
            Messenger.RemoveListener<ECharacterType,Vector2>(EventKey.ActorSpawnPortal, OnActorSpawn);
            Messenger.RemoveListener<ECharacterType, Vector2, string>(EventKey.ActorSpawnPortalWithCustomVFX, OnActorSpawn);
        }
        private void OnActorSpawn(ECharacterType characterType,Vector2 position)
        {
            if (characterType== ECharacterType.Enemy)
            {
                Pool.GameObjectSpawner.Instance.Get("VFX_SpawnEnemy", res =>
                {
                    res.GetComponent<Effect.EffectAbstract>().Active(position);
                });
            }
        }

        private void OnGetHit(DamageSource damageSource, IDamageDealer damageDealer)
        {
            Vector3 dealerPos = damageDealer == null ? damageSource.Defender.GetMidTransform().position : damageDealer.GetDamagePosition();
            Pool.GameObjectSpawner.Instance.Get(normalGetHitKey, res =>
            {
                res.GetComponent<Effect.EffectAbstract>().Active(dealerPos);
            });

            if (damageSource.Defender.GetCharacterType() == ECharacterType.Player)
            {
                HitEffect.Instance.SetUp();
            }
        }

        public static void GetEffect(string eff, Action<Pool.PoolObject> pool)
        {
            if (eff.IsNotNullAndEmpty())
            {
                GameObjectSpawner.Instance.Get(eff, pool);
            }
        }
    }
}
