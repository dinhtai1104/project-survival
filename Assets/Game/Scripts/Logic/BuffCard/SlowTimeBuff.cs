using Game.GameActor;
using Game.GameActor.Buff;
using System;
using System.Collections.Generic;

public class SlowTimeBuff : AbstractBuff
{
    private StatModifier statModifier;
    protected virtual void OnEnable()
    {
        Messenger.AddListener<ActorBase, BulletBase, List<ModifierSource>>(EventKey.PreFire, OnPreFire);
    }

    private void OnPreFire(ActorBase firer, BulletBase bullet, List<ModifierSource> mod)
    {
        if (firer == Caster) return;
        Logger.Log("ON PRE FIRE: " + mod.Count);
        if (mod.Count > 0)
        {
            Logger.Log(">: " + mod[0].Value);

            mod[0].AddModifier(statModifier);
            Logger.Log(">>>>>>>: " + mod[0].Value);

        }
    }

    protected virtual void OnDisable()
    {
        Messenger.RemoveListener<ActorBase, BulletBase, List<ModifierSource>>(EventKey.PreFire, OnPreFire);
    }

  
    public override void Play()
    {
        statModifier = new StatModifier(EStatMod.PercentAdd, -GetValue(StatKey.SpeedBullet));
    }
}
