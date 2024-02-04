using Game.GameActor;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.GameActor.Buff
{
    public abstract class AbstractBuff : MonoBehaviour, IBuff
    {
        /// <summary>
        /// Dùng cho các buff không phải buff card
        /// </summary>
        [SerializeField] private EBuff BuffType;
        public EBuff BuffKey => BuffType;
        private BuffEntity _buffEntity;
        [ShowInInspector]
        private BuffAtrData _buffData;
        public BuffAtrData BuffData => _buffData;
        private ActorBase _caster;
        public ActorBase Caster => _caster;
        public string EffectId;
        public bool IsPause { set; get; }
        public BuffEntity BuffEntity { get => _buffEntity; set => _buffEntity = value; }

        protected bool IsExist = false;

        public virtual void Initialize(ActorBase Caster, BuffEntity entity, int stageStart)
        {
            if (IsExist) return;
            IsExist = true;
            _caster = Caster;
            IsPause = false;
            _buffEntity = entity;
            BuffType = entity.Type;

            _buffData = new BuffAtrData(entity.Id, entity.Type, stageStart);
        }
        public float GetValue(StatKey statKey)
        {
            return BuffData.StatRefer.GetModifier(statKey).Value;
        }
        public abstract void Play();
        public void BeforePlay()
        {
            Caster.Stats.RemoveModifiersFromSource(this);
        }
        protected virtual void OnDestroy()
        {
            Caster.Stats.RemoveModifiersFromSource(this);
        }
        public void LevelUp()
        {
            _buffData.LevelUp();
        }
        public float GetCooldownAbility()
        {
            float rs = BuffData.GetValueStat(StatKey.Cooldown);
            return rs;
        }
        public float GetBaseDamage()
        {
            float rs = Caster.Stats.GetValue(StatKey.Dmg);
            return rs;
        }
        public virtual void OnUpdate(float dt) { }

        public virtual void Exit()
        {
            IsExist = false;
            Caster.Stats.RemoveModifiersFromSource(this);
            PoolManager.Instance.Despawn(gameObject);
        }

        public StatModifier GetModifier(StatKey statKey)
        {
            return BuffData.StatRefer.GetModifier(statKey);
        }
        public override string ToString()
        {
            return BuffType.ToString();
        }
    }
}