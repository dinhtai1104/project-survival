using Game.GameActor;
using Game.GameActor.Buff;
using System;
using UnityEngine;

public class FreezeBloodBuff : AbstractBuff
{
    public ValueConfigSearch Duration;
    float lastActiveTime = 0;
    private BuffSave buffSave;
    private void OnEnable()
    {
        Messenger.AddListener<ActorBase, ActorBase, DamageSource>(EventKey.BeforeHit,OnBeforeHit);
    }

    

    private void OnDisable()
    {
        Messenger.RemoveListener<ActorBase, ActorBase, DamageSource>(EventKey.BeforeHit, OnBeforeHit);
    }
    private void OnBeforeHit(ActorBase attacker, ActorBase current, DamageSource source)
    {
        if (attacker == Caster) return;
        //if this hit is fatal

        if (source.Value >= current.HealthHandler.GetHealth() && buffSave.CanActiveAgain)
        {
            //if buff is not activated
            if (lastActiveTime == 0)
            {
                source._damage.BaseValue = current.HealthHandler.GetHealth() - 1;
                source._damage.ClearModifiers();
                lastActiveTime = Time.time;
                
                
            }
            else
            {
                if (Time.time - lastActiveTime < Duration.FloatValue)
                {
                    source._damageType = EDamage.Missed;
                    source._damage.BaseValue = 0;
                    source._damage.ClearModifiers();
                }
                else
                {
                    if (buffSave.CanActiveAgain)
                    {
                        buffSave.CanActiveAgain = false;
                        GameController.Instance.GetSession().Save();
                    }
                }
            }
        }
    }

    public override void Play()
    {
        lastActiveTime = 0;
        var save = GameController.Instance.GetSession().buffSession.Dungeon.BuffEquiped;
        buffSave = save[EBuff.FreezeBlood];
    }
}