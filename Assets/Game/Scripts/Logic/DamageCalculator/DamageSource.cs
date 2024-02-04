using UnityEngine;
namespace Game.GameActor
{
    [System.Serializable]
    public class DamageSource
    {
        public ActorBase Attacker;
        public ActorBase Defender;
        public IDamageDealer dealer;

        public EDamageSource _damageSource = EDamageSource.Weapon;

        public Vector2 posHit = Vector2.one * -1000; // avoid
        public EDamage _damageType = EDamage.Normal;
        public Stat _damage;
        public bool IsCrit = false;
        public float Value
        {
            set { _damage.BaseValue = value; _damage.RecalculateValue(); }
            get { return _damage.Value; }
        }
        // Add affix to this damage
        public void AddModifier(StatModifier mod)
        {
            _damage.AddModifier(mod);
        }

        public void RemoveModifier(StatModifier mod)
        {
            _damage.RemoveModifier(mod);
        }
        public DamageSource(ActorBase attacker,ActorBase defender,float baseDamage,IDamageDealer dealer)
        {
            this.Attacker = attacker;
            this.Defender = defender;
            _damage = new Stat(baseDamage);
            this.dealer = dealer;
        }
        public Color GetColorDmg() => _damageType.GetColor();
        public DamageSource() { }

        public override string ToString()
        {
            return "Attaker:" + Attacker.gameObject.name + " Target: " + Defender.gameObject.name;
        }
    }
    [System.Serializable]
    public class ModifierSource
    {
        [SerializeField] private Stat stat;
        public Stat Stat => stat;
        public float Value
        {
            set { stat.BaseValue = value; }
            get { return stat.Value; }
        }
        // Add affix to this damage
        public void AddModifier(StatModifier mod)
        {
            stat.AddModifier(mod);
            stat.RecalculateValue();
        }

        public void RemoveModifier(StatModifier mod)
        {
            stat.RemoveModifier(mod);
            stat.RecalculateValue();
        }

        public ModifierSource(Stat stat)
        {
            this.stat = stat;
        }

        public ModifierSource(float value)
        {
            stat = new Stat();

            stat.BaseValue=stat.Value = value;
        }
    }
}