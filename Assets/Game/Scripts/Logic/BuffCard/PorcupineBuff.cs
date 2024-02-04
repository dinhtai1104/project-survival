using com.mec;
using Game.GameActor;
using Game.GameActor.Buff;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PorcupineBuff : AbstractBuff
{
    private void OnEnable()
    {
        Messenger.AddListener<ActorBase, ActorBase>(EventKey.AttackEventByWeapon, OnTriggerAttack);

    }
    private void OnDisable()
    {
        Messenger.RemoveListener<ActorBase, ActorBase>(EventKey.AttackEventByWeapon, OnTriggerAttack);
        Timing.KillCoroutines(gameObject);
    }

    private void OnTriggerAttack(ActorBase attacker, ActorBase defender)
    {
        if (defender != Caster) return;

        Timing.RunCoroutine(_ShootBack(attacker, defender), gameObject);
        
    }

    private IEnumerator<float> _ShootBack(ActorBase attacker, ActorBase defender)
    {
        for (int i = 0; i < GetValue(StatKey.Number); i++)
        {
            var dir = attacker.GetMidTransform().position - defender.GetMidTransform().position;
            Caster.AttackHandler.Trigger(dir.normalized,attacker);
            yield return Timing.WaitForSeconds(0.2f);
        }
    }

    public override void Play()
    {
    }
}
