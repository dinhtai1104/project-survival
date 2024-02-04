using BansheeGz.BGDatabase;
using Game.GameActor;
using System;
using UnityEngine;

public class FreezeControlStatus : BaseStatus
{
    public override void Init(ActorBase source, ActorBase target)
    {
        base.Init(source, target);
        Messenger.AddListener<ActorBase, ActorBase>(EventKey.AttackEventByWeapon, OnAttackEvent);
        if (Target)
        {
            Target.MoveHandler.Locked = true;
            Target.AttackHandler.active = false;
            InputController.InputController.ENABLED = false;
            Target.GetRigidbody().velocity = Vector3.zero;
            Target.GetRigidbody().isKinematic = true;
        }
        Messenger.Broadcast<bool>(EventKey.FreezeTime, true);
    }
    protected override void Release()
    {
        if (Target)
        {
            Target.MoveHandler.Locked = false;
            Target.AttackHandler.active = true;
            InputController.InputController.ENABLED = true;
            Target.GetRigidbody().isKinematic = false;
        }
        Messenger.Broadcast<bool>(EventKey.FreezeTime, false);
        Messenger.RemoveListener<ActorBase, ActorBase>(EventKey.AttackEventByWeapon, OnAttackEvent);
        base.Release();
    }

    private void OnAttackEvent(ActorBase attacker, ActorBase defenser)
    {
        if (defenser != null && defenser == Target)
        {
            Release();
        }
    }

    public override void OnUpdate(float deltaTime)
    {
    }

    public override void SetDmgMul(float dmgMul)
    {
    }

    public override void SetCooldown(float cooldown)
    {
    }
}