using Game.GameActor;
using Game.GameActor.Buff;
using System;

public class AngelWingsBuff : AbstractBuff
{
    private void OnEnable()
    {
        Messenger.AddListener<EControlCode, ActorBase>(EventKey.PressKeyControl, OnPressKeyControl);
        Messenger.AddListener<EControlCode, ActorBase>(EventKey.ReleaseKeyControl, OnReleaseKeyControl);
    }

    private void OnDisable()
    {
        Messenger.AddListener<EControlCode, ActorBase>(EventKey.PressKeyControl, OnPressKeyControl);
        Messenger.AddListener<EControlCode, ActorBase>(EventKey.ReleaseKeyControl, OnReleaseKeyControl);
    }

    private void OnPressKeyControl(EControlCode code, ActorBase actor)
    {
        if (actor != Caster) return;
        if (code != EControlCode.Jump) return;
        //% gravity
        Caster.PropertyHandler.AddProperty(EActorProperty.FallDrag, (int)GetValue(StatKey.FallDrag));
       
    }

    private void OnReleaseKeyControl(EControlCode code, ActorBase actor)
    {
        if (actor != Caster) return;
        if (code != EControlCode.Jump) return;
        Caster.PropertyHandler.AddProperty(EActorProperty.FallDrag, 0);

    }

    public override void Play()
    {
    }
}