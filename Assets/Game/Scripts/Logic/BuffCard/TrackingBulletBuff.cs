using Game.GameActor;
using Game.GameActor.Buff;
using System.Collections.Generic;

public class TrackingBulletBuff : AbstractBuff
{
    private void OnEnable()
    {
        Messenger.AddListener<ActorBase, BulletBase, List<ModifierSource>>(EventKey.PreFire, OnPreFire);

    }
    private void OnDisable()
    {
        Messenger.RemoveListener<ActorBase, BulletBase, List<ModifierSource>>(EventKey.PreFire, OnPreFire);
    }

  
    void OnPreFire(ActorBase actor, BulletBase bullet, List<ModifierSource> modifiers)
    {

        if (actor == Caster)
        {
            if (modifiers.Count > 3)
            {
                modifiers[3].Value = (BuffData.Level + 1) * GetValue(StatKey.Rate);
                modifiers[3].Stat.RecalculateValue();
            }
        }
    }


    public override void Play()
    {

    }
}