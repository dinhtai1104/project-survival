using Cysharp.Threading.Tasks;
using Effect;
using Game;
using Game.AI.State;
using Game.Pool;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.GameActor
{
    public class Character : ActorBase,  IOnVisible
    {
        private readonly Vector3 left = new Vector3(0, 180);
        private readonly Vector3 right = new Vector3(0, 0);

        //
        protected CancellationTokenSource cancellation;


      

        private Transform _transform;
        public Transform midTransform, frontTransform,lookTransform;
        public Transform aimTransform, weaponHolder;
        private Rigidbody2D rb;

        protected ActorBase lastDamageSource;

        public Vector2 lookDirection = Vector2.zero;

        [SerializeField]
        private AssetReference deathEffect;

        //


        protected override void OnDisable()
        {
            base.OnDisable();
            if (cancellation != null)
            {
                cancellation.Cancel();
            }
            this.SkillEngine.CancelAllSkill();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
         
            if (cancellation != null)
            {
                cancellation.Cancel();
                cancellation.Dispose();
                cancellation = null;
            }

        }
        protected override void OnEnable()
        {
            base.OnEnable();
            cancellation = new CancellationTokenSource();
        }
        public void Clear()
        {
            PropertyHandler.ClearAll();
        }





        #region get

        public override bool IsThreat()
        {
            return true;
        }
        public override bool CanFocusOn()
        {
            return PropertyHandler.GetProperty(EActorProperty.Trackable,1)==1;
        }

        public override ECharacterType GetCharacterType()
        {
            return ECharacterType.Player;
        }
        public bool IsStun()
        {
            return StatusEngine.HasStatus<StunStatus>();
        }
        public bool IsKnockBack()
        {
            return StatusEngine.HasStatus<KnockBackStatus>();
        }
        public override Transform GetMidTransform()
        {
            return midTransform;
        }
        public override  Vector3 GetPosition()
        {
            return GetTransform().position;
        }
        public override Vector3 GetDamagePosition()
        {
            return GetMidTransform().position;
        }
        public override int GetFacingDirection()
        {
            return GetTransform().localEulerAngles.y == 0 ? 1 : -1;
        }
        public override Transform GetLookTransform()
        {
            return lookTransform;
        }
        public override Transform GetAimTransform()
        {
            return aimTransform;
        }
        public override Transform GetTransform()
        {

            if (_transform == null)
            {
                _transform = transform;
            }
            return _transform;
        }
        public override Rigidbody2D GetRigidbody()
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody2D>();
            }
            return rb;
        }
        public override Vector2 GetLookDirection()
        {
            if (lookDirection.SqrMagnitude() != 0)
                return lookDirection;
            return MoveHandler.lastMove.normalized;
        }
        #endregion
        #region set

        public override void SetFacing(float direction)
        {
            if (lookDirection.SqrMagnitude() != 0) return;
            GetTransform().localEulerAngles = direction > 0 ? right : left;
        }
        
        public void SetLookDirection(float x, float y)
        {
            lookDirection.Set(x, y);
        }
    
        #endregion
        #region setup
        //set up handler and weapon
        protected virtual async UniTask SetUp()
        {
            Clear();
            PrepareBehaviours();
            await this.WeaponHandler.SetUp(this);
            this.BehaviourHandler?.SetUp(this);
            this.AttackHandler?.SetUp(this);
            this.MoveHandler?.SetUp(this);
            this.AnimationHandler?.SetUp(this);
            this.AnimationHandler?.SetIdle();
        }

        //setup character base on a stat
        public virtual async UniTask SetUp(IStatGroup baseStat)
        {
            IsReady = false;
            Stats = baseStat;

            HealthHandler = new CharacterHealthHandler((int)Stats.GetValue(StatKey.Hp, 0),0);
            HealthHandler.SetActor(this as ActorBase);
            HealthHandler.onHealthDepleted += Dead;
            await SetUp();

            onUpdate?.Invoke(this);
            SetFacing(-1);
        }

        #endregion

       
        public override async UniTask Revive(Vector3 spawnPosition)
        {
            GetRigidbody().velocity = Vector2.zero;
            PropertyHandler.AddProperty(EActorProperty.Vunerable, 0);
            PropertyHandler.AddProperty(EActorProperty.Dead, 0);
            PropertyHandler.AddProperty(EActorProperty.Stun, 0);

            HealthHandler.RestoreHealth();
            this.AnimationHandler.SetIdle();
            SetPosition(spawnPosition);
            onUpdate?.Invoke(this);
            Game.Pool.GameObjectSpawner.Instance.Get("VFX_Revive", obj =>
            {
                obj.GetComponent<Effect.EffectAbstract>().Active(GetPosition());
            });
            SetActive(true);
            this.BehaviourHandler.SetUp(this);
            //await this.WeaponHandler.SetUp(this);
            await UniTask.Delay(800,cancellationToken:cancellation.Token);
            //// Active HealBar
            Messenger.Broadcast(EventKey.ActorSpawn, (ActorBase)this,true, -1);
            AttackHandler.Active();
            StartBehaviours();

            await UniTask.Delay((int)(new ValueConfigSearch("Player_ReviveInvunerableTime").FloatValue*1000),cancellationToken:cancellation.Token);
            PropertyHandler.AddProperty(EActorProperty.Vunerable, 1);

        }

        //this character got hit by an attacker

        public override bool GetHit(DamageSource damageSource, IDamageDealer dealer)
        {

            if (StatusEngine.HasStatus<ImmuneStatus>())
            {
                Messenger.Broadcast(EventKey.ImmuneAttack, damageSource.Attacker, damageSource.Defender);
                return false;
            }
            if(DamageHandler.GetHit(  dealer, damageSource))
            {
                if (damageSource.Attacker != null)
                {
                    lastDamageSource = damageSource.Attacker;
                }
                onGetHit?.Invoke( damageSource, dealer);
                Messenger.Broadcast<DamageSource, IDamageDealer>(EventKey.GetHit, damageSource, dealer);
                return true;
            }
            return false;
        }
        protected virtual void PlayDeadSFX()
        {
            if (soundData != null && soundData.dieSFXs != null && soundData.dieSFXs.Length != 0 && Random.Range(0f, 1f) > 0.7f)
                this.SoundHandler.PlayOneShot(soundData.dieSFXs[Random.Range(0, soundData.dieSFXs.Length)], 1f);
        }
        protected virtual void PlayDeadEffect()
        {
            GameObjectSpawner.Instance.Get((deathEffect == null || (deathEffect != null && string.IsNullOrEmpty(deathEffect.RuntimeKey.ToString()))) ? "VFX_CharacterDead" : deathEffect.RuntimeKey.ToString(), res =>
            {
                Vector3 rd = Random.insideUnitCircle / 3f;
                rd.y = Mathf.Abs(rd.y);
                res.GetComponent<Effect.EffectAbstract>().Active(GetMidTransform().position);
            });
        }
        public virtual void Dead()
        {
            if (IsDead()) return;
            Logger.Log("dead state");
            Machine.ChangeState<ActorDeadState>();

            PropertyHandler.AddProperty(EActorProperty.Dead, 1);


            PlayDeadEffect();
            PlayDeadSFX();


            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 6;

            WeaponHandler.Stop();
            this.MoveHandler.Stop();
            this.AttackHandler.Stop();
            this.AnimationHandler.SetDead();
            this.SkillEngine.CancelAllSkill();
            onUpdate?.Invoke(this);
            //onDie?.Invoke(this, lastDamageSource);
            //onActorDie?.Invoke();
            CallDieEvent().Forget();
            async UniTask CallDieEvent()
            {
                await UniTask.DelayFrame(5);
                onDie?.Invoke(this, lastDamageSource);
                onActorDie?.Invoke();
                onSelfDie?.Invoke(this as ActorBase);

            }
            HighLight(false);
        }

        protected override async void OnReviveEvent()
        {
            base.OnReviveEvent();
            var reviveTimes = Stats.GetValue(StatKey.Revive);
            if (reviveTimes > 0)
            {
                onRevive?.Invoke(true);
                await Revive(transform.position);
                return;
            }
            onRevive?.Invoke(false);
        }

        public virtual void OnDead() 
        {
            WeaponHandler.Destroy();
            BehaviourHandler.Destroy();

            SetActive(false);
            OnReviveEvent();
        }

        public virtual void OnVisible(bool isVisible)
        {
            PropertyHandler.AddProperty(EActorProperty.Visible, isVisible ? 1 : 0);
        }

        public override void SetFacing(ActorBase target)
        {
            if (target == null) return;
            var dir = target.GetPosition().x - GetPosition().x;
            SetFacing(dir);
        }
    }
}