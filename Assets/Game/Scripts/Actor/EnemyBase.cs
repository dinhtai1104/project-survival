using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace Game.GameActor
{
    public class EnemyBase : Character, IBatchUpdate
    {
        [SerializeField]
        protected MoreMountains.Feedbacks.MMF_Player dieFb,getHitFb;
        public string impactEffectId = "VFX_Impact_Blood 2";
        protected override async UniTask SetUp()
        {
            await base.SetUp();
            Stats.AddListener(StatKey.Dmg, OnSetNewDPS);
            Stats.CalculateStats();
        }

        private void OnSetNewDPS(float obj)
        {
        }

        public override ECharacterType GetCharacterType()
        {
            return ECharacterType.Enemy;
        }
       

        protected float effectTime = 0;
        public override bool GetHit( DamageSource damageSource, IDamageDealer dealer)
        {
            if (base.GetHit( damageSource, dealer))
            {
                PlayGetHitSFX();
                getHitFb?.PlayFeedbacks();
                int direction = damageSource.Attacker.GetMidTransform().position.x < damageSource.Defender.GetMidTransform().position.x ? 1 : -1;
                Pool.GameObjectSpawner.Instance.Get(impactEffectId, res =>
                {
                    var effect=res.GetComponent<Effect.EffectAbstract>().Active(GetMidTransform().position);

                    effect.transform.localScale = new Vector3(direction, 1, 1);
                });

                return true;
            }
            return false;
        }

        protected virtual void PlayGetHitSFX()
        {
            if (Time.time - effectTime > 0.25f)
            {
                if (soundData != null && UnityEngine.Random.Range(0f, 1f) > 0.9f)
                {
                    this.SoundHandler.PlayOneShot(soundData.hurtSFXs[UnityEngine.Random.Range(0, soundData.hurtSFXs.Length)], 1f);
                    effectTime = Time.time;
                }
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            //UpdateSlicer.Instance.RegisterSlicedUpdate(this, UpdateSlicer.UpdateMode.Auto);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            Stats.RemoveListener(StatKey.Dmg, OnSetNewDPS);
        }
        protected override void OnDisable()
        {
            base.OnDisable();

            //UpdateSlicer.Instance.DeregisterSlicedUpdate(this);
        }
        public override void OnVisible(bool isVisible)
        {

        }
        public override void Dead()
        {
            if (IsDead()) return;
            base.Dead();
            dieFb?.PlayFeedbacks();
            GetRigidbody().gravityScale = 5;
        }
        public void BatchUpdate()
        {
            if (IsDead() || IsStun()) return;

          
        }

        public void BatchFixedUpdate()
        {
        }
    }
}