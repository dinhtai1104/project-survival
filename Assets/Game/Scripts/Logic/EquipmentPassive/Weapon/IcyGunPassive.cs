using Game.GameActor;
using System.Collections.Generic;

public class IcyGunPassive : WeaponPassive
{
    public ValueConfigSearch freezeStatus_Stat;
    public ValueConfigSearch freezeStatus_Duration;

    public ValueConfigSearch epic_freezeStatus_Stat;
    public ValueConfigSearch legendary_freezeStatus_Stat;
    StatModifier stat = new StatModifier(EStatMod.PercentAdd, 0);

    private void OnEnable()
    {
        Messenger.AddListener<ActorBase, ActorBase>(EventKey.AttackEventByWeapon, OnAttackEvent);
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<ActorBase, ActorBase>(EventKey.AttackEventByWeapon, OnAttackEvent);
    }

    private async void OnAttackEvent(ActorBase attacker, ActorBase defender)
    {
        if (attacker != Caster) return;
        if (defender.Tagger.HasAnyTags(ETag.Boss, ETag.Elite, ETag.MiniBoss)) return;
        var status = await defender.StatusEngine.AddStatus(attacker, EStatus.Freeze, this);
        if (status != null)
        {
            status.Init(attacker, defender);
            status.SetDuration(freezeStatus_Duration.FloatValue);

            stat.Value = freezeStatus_Stat.FloatValue;

            if (Rarity >= ERarity.Epic)
            {
                stat.Value = epic_freezeStatus_Stat.FloatValue;
            }
            if (Rarity >= ERarity.Legendary)
            {
                stat.Value = legendary_freezeStatus_Stat.FloatValue;
            }

            var list = new List<AttributeStatModifier>()
            {
                new AttributeStatModifier { StatKey = StatKey.SpeedMove, Modifier = stat },
                new AttributeStatModifier { StatKey = StatKey.FireRate, Modifier = stat },
            };
            ((FreezeStatus)status).AddFreezeStat(list);
        }
    }
}
