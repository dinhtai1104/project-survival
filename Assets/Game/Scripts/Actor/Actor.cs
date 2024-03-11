using Core;
using Cysharp.Threading.Tasks;
using Framework;
using Sirenix.OdinInspector;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public class Actor : MonoBehaviour
    {
        [SerializeField] private LayerMask m_EnemyLayerMask;
        [SerializeField] private LayerMask m_AllyLayerMask;
        [SerializeField] private bool m_AI;
        [SerializeField] private bool m_IsDead;
        [SerializeField] private float m_Width = 1f;
        [SerializeField] private float m_Height = 1f;
        [SerializeField] private Transform m_GraphicTrans;
        [SerializeField] private Transform m_HealthTrans;
        [SerializeField] private Rigidbody2D m_RigidBody;
        [SerializeField] private Collider2D m_Collider;

        private Transform m_Trans;
        private Transform m_CenterTrans;
        private bool m_IsInitialize = false;

        [ShowInInspector]
        private IStatGroup m_Stat = new StatGroup();
        private IStatusEngine m_Status;
        private IAnimationEngine m_Animation;
        private IFsm m_Fsm;
        private IMovementEngine m_Movement;
        private ITargetFinder m_TargetFinder;
        private IDamageCalculator m_DamageCalculator;
        private IHealth m_Health;
        private IGraphicEngine m_Graphic;
        private ITagger m_Tagger;
        private IInputHandler m_Input;
        private IBrain m_Brain;
        private ISkillCaster m_SkillCaster;
        private ISharedEngine m_SharedEngine;
        private IRVO m_RVO;

        private static readonly IStatGroup NullStat = new NullStatGroup();
        private static readonly IStatusEngine NullStatus = new NullStatusEngine();
        private static readonly IAnimationEngine NullAnimation = new NullAnimationEngine();
        private static readonly IFsm NullFsm = new NullFsm();
        private static readonly IMovementEngine NullMovement = new NullMovementEngine();
        private static readonly ITargetFinder NullTargetFinder = new NullTargetFinder();
        private static readonly IDamageCalculator NullDamageCalculator = new NullDamageCalculator();
        private static readonly IHealth NullHealth = new NullHealth();
        private static readonly IGraphicEngine NullGraphic = new NullGraphicEngine();
        private static readonly ITagger NullTagger = new NullTagger();
        private static readonly IInputHandler NullInput = new NullInputHandler();
        private static readonly IBrain NullBrain = new NullBrain();
        private static readonly ISkillCaster NullSkillCaster = new NullSkillCaster();
        private static readonly IRVO NullRVO = new NullRVO();
        public bool AI
        {
            get { return m_AI; }
            set { m_AI = value; }
        }

        public bool IsDead
        {
            set { m_IsDead = value; }
            get { return m_IsDead; }
        }

        public UIHealthHud HealthHud { private set; get; }

        public LayerMask EnemyLayerMask
        {
            get => m_EnemyLayerMask;
            set => m_EnemyLayerMask = value;
        }

        public LayerMask AllyLayerMask
        {
            get => m_AllyLayerMask;
            set => m_AllyLayerMask = value;
        }

        public Collider2D Collider => m_Collider;
        public float Width => m_Width;
        public float Height => m_Height;
        public Transform GraphicTrans => m_GraphicTrans != null ? m_GraphicTrans : m_Trans;
        public Transform HealthTrans => m_HealthTrans != null ? m_HealthTrans : m_Trans;
        public Transform CenterTransform => m_CenterTrans;
        public virtual Vector3 CenterPosition => GraphicTrans.position + new Vector3(0, m_Height / 2f, 0);
        public virtual Vector3 TopPosition => GraphicTrans.position + new Vector3(0, m_Height, 0);
        public virtual Vector3 BotPosition => GraphicTrans.position;

        public virtual Vector3 FrontPosition
        {
            get
            {
                Vector3 frontPos = GraphicTrans.position;
                frontPos.x += Movement.DirectionSign * m_Width / 2f;
                return frontPos;
            }
        }

        public virtual Vector3 RearPosition
        {
            get
            {
                Vector3 rearPos = GraphicTrans.position;
                rearPos.x -= Movement.DirectionSign * m_Width / 2;
                return rearPos;
            }
        }

        public Transform Trans => m_Trans;
        public Rigidbody2D RigidBody => m_RigidBody;
        public ITagger Tagger => m_Tagger ?? NullTagger;
        public IStatGroup Stats => m_Stat ?? NullStat;
        public IStatusEngine Status => m_Status ?? NullStatus;
        public IAnimationEngine Animation => m_Animation ?? NullAnimation;
        public IFsm Fsm => m_Fsm ?? NullFsm;
        public IMovementEngine Movement => m_Movement ?? NullMovement;
        public ITargetFinder TargetFinder => m_TargetFinder ?? NullTargetFinder;
        public IDamageCalculator DamageCalculator => m_DamageCalculator ?? NullDamageCalculator;
        public IHealth Health => m_Health ?? NullHealth;
        public IGraphicEngine Graphic => m_Graphic ?? NullGraphic;
        public IInputHandler Input => m_Input ?? NullInput;
        public IBrain Brain => m_Brain ?? NullBrain;
        public ISkillCaster SkillCaster => m_SkillCaster ?? NullSkillCaster;
        public ISharedEngine Shared => m_SharedEngine;
        public IRVO RVO => m_RVO ?? NullRVO;

        [SerializeField] private TeamModel m_TeamModel;
        public TeamModel TeamModel => m_TeamModel;

        public void Prepare()
        {
            IsDead = false;
        }

        [Button]
        public virtual void Init(TeamModel teamModel)
        {
            gameObject.SetActive(true);
            this.m_TeamModel = teamModel;
            EnemyLayerMask = teamModel.EnemyLayerMask;
            AllyLayerMask = teamModel.AllyLayerMask;
            m_IsInitialize = true;

            Animation?.Init(this);
            Movement?.Init(this);
            TargetFinder?.Init(this);
            Fsm?.Init(this);
            DamageCalculator?.Init(this);
            Health?.Init(this);
            Graphic?.Init(this);
            Status?.Init(this);
            Input?.Init(this);
            SkillCaster?.Init(this);
            Brain?.Init(this);
            Input.Active = true;
            Shared.ClearAll();
            RVO.Init(this);

            Health.Initialized = false;
            Brain.Lock = false;
            Input.Lock = false;
            SkillCaster.IsLocked = false;
            Status.Lock = false;
            Brain.Lock = false;
        }

        protected virtual void Awake()
        {
            m_Trans = transform;
            m_CenterTrans = new GameObject("Center").transform;
            m_CenterTrans.position = CenterPosition;
            m_CenterTrans.parent = m_Trans;

            m_Animation = GetComponent<IAnimationEngine>();
            m_Fsm = GetComponent<IFsm>();
            m_Movement = GetComponent<IMovementEngine>();
            m_TargetFinder = GetComponent<ITargetFinder>();
            m_DamageCalculator = GetComponent<IDamageCalculator>();
            m_Health = GetComponent<IHealth>();
            m_Graphic = GetComponent<IGraphicEngine>();
            m_Status = GetComponent<IStatusEngine>();
            m_Input = GetComponent<IInputHandler>();
            m_SkillCaster = GetComponent<ISkillCaster>();
            m_Brain = GetComponent<IBrain>();
            m_Tagger = GetComponent<ITagger>();
            m_RVO = GetComponent<IRVO>();
            m_SharedEngine = new ShareValueEngine();
        }

        protected virtual void Update()
        {
            if (!m_IsInitialize) return;
            Movement?.OnUpdate();
            TargetFinder?.OnUpdate();
            Fsm?.OnUpdate();
            Status?.OnUpdate();
            Input?.OnUpdate();
            Brain?.OnUpdate();
            SkillCaster?.OnUpdate();
        }


        protected virtual void OnDestroy()
        {
            Animation?.Clear();
            TargetFinder?.Clear();
            Stats?.RemoveAllStats();
            Graphic?.ClearFlashColor();
            Status?.ClearAllStatus();
            Reset();
        }

        public virtual void OnEnable()
        {
            if (!IsDead) return;
            Reset();
        }


        public virtual void OnDisable()
        {
            IsDead = true;
            Brain.Lock = true;
            Animation.Lock = true;
            SkillCaster.IsLocked = true;
            Movement.LockMovement = true;
            TargetFinder.Clear();
            SetActiveCollider(false);
        }


        public void SetActiveCollider(bool enable)
        {
            if (m_Collider != null) m_Collider.enabled = enable;
        }

        public void SetColliderTrigger(bool trigger)
        {
            if (m_Collider != null) m_Collider.isTrigger = trigger;
        }

        public virtual void Reset()
        {
            IsDead = false;
            Animation.Lock = false;
            Movement.LockMovement = false;
            TargetFinder?.Clear();
            Graphic?.ClearFlashColor();
            Graphic?.SetGraphicAlpha(1f);
            Fsm?.BackToDefaultState();
            Stats?.ClearAllModifiers();
            Status?.ClearAllStatus();
            SetActiveCollider(true);
        }

        public virtual void Destroy()
        {
           
        }

        [Button]
        public void SetStat(string stat, float value)
        {
            if (Stats.HasStat(stat))
            {
                Stats.SetBaseValue(stat, value);
            }
            else
            {
                Stats.AddStat(stat, value);
            }

            Stats.GetStat(stat).RecalculateValue();
        }
    }
}