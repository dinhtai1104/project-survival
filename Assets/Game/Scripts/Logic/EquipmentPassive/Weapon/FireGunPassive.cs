using Game.GameActor;

public class FireGunPassive : WeaponPassive
{
    public ValueConfigSearch flameStatus_Duration;
    public ValueConfigSearch flameStatus_DmgMul;
    public ValueConfigSearch flameStatus_DmgTickrate;
    
    
    public ValueConfigSearch epic_flameStatus_DmgMul;

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
        //if (Rarity < ERarity.Epic) return;
        var flame = await defender.StatusEngine.AddStatus(attacker, EStatus.Flame, this);
        if (flame != null)
        {
            flame.Init(attacker, defender);
            flame.SetDuration(flameStatus_Duration.FloatValue);

            ((FlameStatus)flame).SetCooldown(flameStatus_DmgTickrate.FloatValue);
            float dmgMul = 1f;
            if (Rarity < ERarity.Epic)
            {
                dmgMul = flameStatus_DmgMul.FloatValue;
            }
            if (Rarity >= ERarity.Epic)
            {
                dmgMul = epic_flameStatus_DmgMul.FloatValue;
            }

            ((FlameStatus)flame).SetDmgMul(dmgMul);
        }
    }

}
