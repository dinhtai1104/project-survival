using Cysharp.Threading.Tasks;
using Game.Damage;
using Game.Fsm;
using Game.GameActor.Buff;
using Game.Skill;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using AI.StateMachine;
using BehaviorDesigner.Runtime;
using Game.Pool;
using Game.AI.State;

namespace Game.GameActor
{
    public abstract class ActorBase : ObjectBase, ITarget, IDamageDealer
    {
        public delegate void OnReviveActor(bool success);
        public OnReviveActor onRevive;

        //global event for all actor
        public delegate void OnDie(ActorBase obj, ActorBase attacker);
        public static OnDie onDie;

        //event for actor
        public delegate void OnUpdate(ActorBase character);
        public OnUpdate onUpdate;
        public delegate void OnActorDie();
        public OnActorDie onActorDie;
        public delegate void OnGetHit(DamageSource damageSource, IDamageDealer damageDealer);
        public OnGetHit onGetHit;
        public delegate void OnActorSelfDie(ActorBase current);
        public OnActorSelfDie onSelfDie;

        public Transform healthBarPlace;

        [ShowInInspector]
        public float HealPercent => HealthHandler?.GetPercentHealth() ?? 0;
        public bool IsUseHealBar = true;

        //

        //handler
        [ShowInInspector]
        private HealthHandler healthHandler;
        private MoveHandler moveHandler;
        private AttackHandler attackHandler;
        private AnimationHandler animationHandler;
        private ObjectSoundHandler soundHandler;
        private ActorPropertyHandler propertyHandler;
        private ArmorHandler armorHandler;
        private DamageHandler damageHandler;
        private WeaponHandler weaponHandler;
        private ActorBehaviourHandler behaviourHandler;
        private BehaviorDesigner.Runtime.Behavior behaviourTree;
        private DetectTargetHandler sensor;
        //sound collection of this character
        public CharacterSoundData soundData;


        #region Services
        //main stat of this character
        private IBuffHandler NullBuffHandler = new NullBuffHandler();
        private IStatusEngine NullStatusEngine = new NullStatusEngine();
        private IDamageCalculator NullDamageCalculator = new NullDamageCalculator();
        private IFsm NullFsm = new NullFsm();
        private IInputHandler NullInput = new NullInput();
        private ISkillEngine NullSkill = new NullSkillEngine();
        private IPassiveEngine NullPassive = new NullPassiveEngine();
        private ITagger NullTagger = new NullTagger();

        private IBuffHandler _buff;
        [ShowInInspector]
        private IStatGroup stats;
        private IStatusEngine _status;
        private IDamageCalculator damageCalculator;
        private IFsm _fsm;
        private IInputHandler _inputHandler;
        private ISkillEngine _skillEngine;
        private IPassiveEngine _passiveEngine;
        private ITagger _tagger;
        public IStatGroup Stats
        {
            get
            {
                return stats;
            }
            set
            {
                stats = value;
            }
        }
        public IBuffHandler BuffHandler => _buff ?? NullBuffHandler;
        public IStatusEngine StatusEngine => _status ?? NullStatusEngine;

        public HealthHandler HealthHandler { get => healthHandler; set => healthHandler = value; }
        public MoveHandler MoveHandler { get { if (moveHandler == null) moveHandler = GetComponent<MoveHandler>(); return moveHandler; } }
        public AttackHandler AttackHandler { get { if (attackHandler == null) attackHandler = GetComponent<AttackHandler>(); return attackHandler; } }
        public AnimationHandler AnimationHandler { get { if (animationHandler == null) animationHandler = GetComponent<AnimationHandler>(); return animationHandler; } }
        public DetectTargetHandler Sensor { get { if (sensor == null) sensor = GetComponent<DetectTargetHandler>(); return sensor; } }
        public ObjectSoundHandler SoundHandler { get { if (soundHandler == null) soundHandler = GetComponentInChildren<ObjectSoundHandler>(); return soundHandler; } }
        public WeaponHandler WeaponHandler { get { if (weaponHandler == null) weaponHandler = GetComponentInChildren<WeaponHandler>(true); return weaponHandler; } }
        public ActorBehaviourHandler BehaviourHandler { get { if (behaviourHandler == null) behaviourHandler = GetComponentInChildren<ActorBehaviourHandler>(); return behaviourHandler; } }
        public ActorPropertyHandler PropertyHandler { get { if (propertyHandler == null) propertyHandler = new ActorPropertyHandler(); return propertyHandler; } }
        public ArmorHandler ArmorHandler { get { if (armorHandler == null) armorHandler = new ArmorHandler(); return armorHandler; } }
        public DamageHandler DamageHandler { get
            {
                if (!TryGetComponent<DamageHandler>(out damageHandler))
                {
                    damageHandler = gameObject.AddComponent<DamageHandler>();
                }
                return damageHandler;
            } }

        public BehaviorDesigner.Runtime.Behavior BehaviourTree { get { if (behaviourTree == null) behaviourTree = GetComponent<BehaviorDesigner.Runtime.BehaviorTree>(); return behaviourTree; } }



        public IFsm Machine => _fsm ?? NullFsm;
        public IInputHandler Input => _inputHandler ?? NullInput;
        public IDamageCalculator DamageCalculator => damageCalculator ?? NullDamageCalculator;
        public ISkillEngine SkillEngine => _skillEngine ?? NullSkill;
        public IPassiveEngine PassiveEngine => _passiveEngine ?? NullPassive;
        public ITagger Tagger => _tagger ?? NullTagger;
        #endregion
        protected virtual void OnDisable() { }
        protected virtual void OnEnable() { }
        protected virtual void OnDestroy()
        {
            WeaponHandler.Destroy();
            BehaviourHandler.Destroy();
        }

        public abstract void SetFacing(float dir);
        public abstract void SetFacing(ActorBase target);
        public abstract Vector3 GetPosition();
        public abstract int GetFacingDirection();
        public abstract Vector2 GetLookDirection();
        public abstract Transform GetLookTransform();
        public abstract Transform GetMidTransform();
        public abstract Transform GetAimTransform();
        public abstract Transform GetTransform();
        public abstract Rigidbody2D GetRigidbody();

        public bool IsActived = false, IsReady = false, IsSilent = false;
        private ActorBase actorSpawner;

        // create clone of original behaviours
        protected void PrepareBehaviours()
        {
            //Initialize Services
            _buff = GetComponent<IBuffHandler>();
            _status = GetComponent<IStatusEngine>();
            damageCalculator = GetComponent<IDamageCalculator>();
            _fsm = GetComponent<IFsm>();
            _inputHandler = GetComponent<IInputHandler>();
            _skillEngine = GetComponent<ISkillEngine>();
            _passiveEngine = GetComponent<PassiveEngine>();
            _tagger = GetComponent<Tagger>();

            BuffHandler.Initialize(this);
            StatusEngine.Initialize(this);
            DamageCalculator.Initialize();
            Machine.Initialize(this);
            Input.Initialize();
            SkillEngine.Initialize(this);
            PassiveEngine.Initialize(this);


            if (BehaviourTree != null)
            {
                var variable = BehaviourTree.GetVariable("Actor");
                if (variable != null)
                {
                    variable.SetValue(this);
                }
                BehaviourTree.DisableBehavior();
            }
            onActorDie += OnDieEvent;
            IsActived = false;
            IsReady = true;
        }

        //actor start behaviours
        public virtual void StartBehaviours()
        {
            IsActived = true;
            BehaviourTree?.EnableBehavior();
            BehaviourHandler?.StartBehaviours();

        }
        protected virtual void OnReviveEvent()
        {
        }

        private void OnDieEvent()
        {
            StatusEngine.ClearAllStatuses();
            behaviourTree?.DisableBehavior();
            SkillEngine.InteruptCurrentSkill();
        }


        protected virtual void Update()
        {
            if (!IsActived) return;
            Machine.Ticks();
            if (IsDead())
            {
                Machine.ChangeState<ActorDeadState>();
                return;
            }

            SkillEngine.Ticks();

            Input.Ticks();
            PassiveEngine.Ticks();
            StatusEngine.Tick(Time.deltaTime);
            BuffHandler.Ticks();
        }
        private void LateUpdate()
        {
            if (!IsActived) return;
            if (IsDead()) return;
            StatusEngine.LateTick(Time.deltaTime);
        }
#if DEVELOPMENT
        [Button]
        public async void AddStatusTest(ActorBase source, EStatus status, object sourceCast)
        {
            var statusPrefab = await StatusEngine.AddStatus(source, status, sourceCast);
            if (statusPrefab != null)
            {
                statusPrefab.SetDuration(5);
            }
        }
        [Button]
        public void DeadForceTest()
        {
            var dmgSource = new DamageSource
            {
                Attacker = GameController.Instance.GetMainActor(),
                Defender = this,
                _damage = new Stat(99999999999)
            };
            GetHit(dmgSource, null);
        }
#endif

        public void DeadForce()
        {
            var dmgSource = new DamageSource
            {
                Attacker = GameController.Instance.GetMainActor(),
                Defender = this,
                _damage = new Stat(GetStatValue(StatKey.Hp))
            };
            GetHit(dmgSource, null);
        }

        public abstract bool GetHit(DamageSource damageSource, IDamageDealer dealer);
        public float GetHealthPoint()
        {
            return HealthHandler.GetHealth();
        }

        public bool IsDead()
        {
            return PropertyHandler.GetProperty(EActorProperty.Dead, 0) == 1;
        }

        public abstract bool IsThreat();
        public abstract bool CanFocusOn();

        public void HighLight(bool active)
        {
        }

        public abstract ECharacterType GetCharacterType();

        public abstract Vector3 GetDamagePosition();

        //
        public void SetPosition(Vector3 position)
        {
            GetTransform().position = position;
        }
        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
            if(HealthHandler!=null)
            this.HealthHandler.onUpdate?.Invoke(this.HealthHandler);
        }


        public abstract UniTask Revive(Vector3 spawnPosition);
        public ITarget FindClosetTarget()
        {
            return Sensor.CurrentTarget;
        }

        //
        private const string QuickHealEffect = "VFX_BuffHeal";
        private const string HealEffect = "VFX_Player_HealDrone";
        public virtual void Heal(float health, bool quickEffect = false, string VFX_Id = "")
        {
            this.HealthHandler.AddHealth(health);
            onUpdate?.Invoke(this);
            Logger.Log("ADDHEALTH:" + health);
            if (gameObject.activeSelf)
            {
                var healAddress = quickEffect ? QuickHealEffect : HealEffect;
                if (!string.IsNullOrEmpty(VFX_Id))
                {
                    healAddress = VFX_Id;
                }
                GameObjectSpawner.Instance.Get(healAddress, res =>
                {
                    res.GetComponent<HealEffect>().Active(GetPosition(), $"+{(int)health}").SetParent(GetTransform());
                });
            }
        }

        public void Teleport(Vector2 targetPos)
        {
            GetTransform().position = targetPos;
        }
        public virtual Vector3 GetMidPos()
        {
            return GetMidTransform().position;
        }

        public float GetStatValue(StatKey stat)
        {
            return Stats.GetValue(stat);
        }

        public virtual ActorBase GetActorSpawner() => actorSpawner;

        public void SetActorSpawner(ActorBase caster)
        {
            actorSpawner = caster;
        }
    }
}

