using Game.GameActor;
using Game.GameActor.Buff;
using System;

public class AngelWhisperBuff : PassiveBuff
{
    public override void Initialize(ActorBase Caster, BuffEntity entity, int stageBuff)
    {
        base.Initialize(Caster, entity, stageBuff);
        Caster.onRevive += OnReviveSuccess;
    }
    public override void Exit()
    {
        base.Exit();
    }

    private void OnReviveSuccess(bool success)
    {
        Caster.Stats.RemoveModifier(StatKey.Revive, GetModifier(StatKey.Revive));
        Caster.onRevive -= OnReviveSuccess;
    }
}
