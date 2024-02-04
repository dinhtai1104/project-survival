using Game.GameActor;
using Game.GameActor.Buff;

namespace Game.BuffCard
{
    public class DoubleShotBuff : AbstractBuff
    {
        public float loopSpacing=0.05f;
        private void OnEnable()
        {
            Messenger.AddListener<ActorBase, ActorBase, DamageSource>(EventKey.BeforeHit, OnBeforeHit);
        }
        private void OnDisable()
        {
            Messenger.RemoveListener<ActorBase, ActorBase, DamageSource>(EventKey.BeforeHit, OnBeforeHit);
        }

        private void OnBeforeHit(ActorBase attacker, ActorBase defender, DamageSource damageSource)
        {
            if (attacker != Caster) return;
            if (damageSource._damageType != EDamage.Normal && damageSource._damageType != EDamage.Critital) return;
            damageSource._damage.AddModifier(new StatModifier(EStatMod.Percent, GetValue(StatKey.ReduceDmg)));
        }



        public override void Play()
        {
            var patterns=((GunBase)Caster.WeaponHandler.CurrentWeapon).GetShootPattern();   
            foreach(Spread_ShootPattern pattern in patterns)
            {
                pattern._loop = 1+ (int)(GetValue(StatKey.Number)*(BuffData.Level+1)) ;
                pattern._loopSpacing = loopSpacing; 
            }
            Caster.Stats.AddModifier(StatKey.FireRate, BuffData.StatRefer.GetModifier(StatKey.FireRate), this);
        }
    }
}