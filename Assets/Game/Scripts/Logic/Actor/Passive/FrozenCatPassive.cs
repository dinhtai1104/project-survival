using Game.GameActor.Buff;
using System.Collections.Generic;

namespace Game.GameActor.Passive
{
    public class FrozenCatPassive : AbstractBuff
    {
        private StatModifier statModifier;
        public ValueConfigSearch freezeAttack_Duration = new ValueConfigSearch("Buff_FreezeAttack_Duration");
        public HashSet<ActorBase> targets=new HashSet<ActorBase>();
        protected  void OnEnable()
        {
            Messenger.AddListener<ActorBase, ActorBase>(EventKey.AttackEventByWeapon, OnAttackSlow);
            Messenger.AddListener<ActorBase, BulletBase, List<ModifierSource>>(EventKey.PreFire, OnPreFire);
        }

        private void OnPreFire(ActorBase firer, BulletBase bullet, List<ModifierSource> mod)
        {
            if (firer == Caster || !firer.StatusEngine.HasStatus<FreezeStatus>()) return;
            Logger.Log("ON PRE FIRE: " + mod.Count);
            if (mod.Count > 0)
            {
                Logger.Log(">: " + mod[0].Value+" "+statModifier.Type+" "+statModifier.Value);

                mod[0].AddModifier(statModifier);
                Logger.Log(">>>>>>>: " + mod[0].Value);

            }
        }

        protected virtual void OnDisable()
        {
            Messenger.RemoveListener<ActorBase, BulletBase, List<ModifierSource>>(EventKey.PreFire, OnPreFire);
            Messenger.RemoveListener<ActorBase, ActorBase>(EventKey.AttackEventByWeapon, OnAttackSlow);
        }


        public override void Play()
        {
            statModifier = new StatModifier(EStatMod.PercentAdd, -GetValue(StatKey.SpeedBullet));
        }


        List<AttributeStatModifier> list = new List<AttributeStatModifier>()
        {
        };
        private async void OnAttackSlow(ActorBase attacker, ActorBase defender)
        {
            if (Caster == attacker)
            {
                var status = (await defender.StatusEngine.AddStatus(attacker, EStatus.Freeze, this));
                if (status != null)
                {
                    status.Init(attacker, defender);
                    status.SetDuration(freezeAttack_Duration.FloatValue);
                    
                    ((FreezeStatus)status).AddFreezeStat(list);
                }
            }
        }
    }
}