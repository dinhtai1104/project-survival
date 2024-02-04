using System;
using System.Collections.Generic;

namespace Game.GameActor.Buff
{
    public class FreezeAttackBuff : AbstractBuff
    {
        public ValueConfigSearch freezeAttack_Duration = new ValueConfigSearch("Buff_FreezeAttack_Duration");
        private StatModifier modifier = new StatModifier(EStatMod.PercentAdd, 0);
        
        private void OnEnable()
        {
            Messenger.AddListener<ActorBase, ActorBase>(EventKey.AttackEventByWeapon, OnAttackSlow);
        }
        private void OnDisable()
        {
            Messenger.RemoveListener<ActorBase, ActorBase>(EventKey.AttackEventByWeapon, OnAttackSlow);
        }

        private async void OnAttackSlow(ActorBase attacker, ActorBase defender)
        {
            if (Caster == attacker)
            {
                var status = (await defender.StatusEngine.AddStatus(attacker, EStatus.Freeze, this));
                if (status != null)
                {
                    status.Init(attacker, defender);
                    status.SetDuration(freezeAttack_Duration.FloatValue);
                    var list = new List<AttributeStatModifier>()
                    {
                        new AttributeStatModifier { StatKey = StatKey.SpeedMove, Modifier = modifier }
                    };
                    ((FreezeStatus)status).AddFreezeStat(list);
                }
            }
        }
        public override void Play()
        {
            modifier.Value = GetValue(StatKey.SpeedMove);
        }
    }
}