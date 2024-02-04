using Game.GameActor;
using System.Collections.Generic;

public class FreezeStatus : BaseStatus
{
    private List<AttributeStatModifier> freezesStat;

    public void AddFreezeStat(List<AttributeStatModifier> freezes)
    {
        this.freezesStat = freezes;
        foreach (var stat in freezesStat)
        {
            Target.Stats.AddModifier(stat.StatKey, stat.Modifier, this);
        }
    }

    public override void Init(ActorBase source, ActorBase target)
    {
        base.Init(source, target);
    }
    protected override void Release()
    {
        foreach (var stat in freezesStat)
        {
            Target.Stats.RemoveModifier(stat.StatKey, stat.Modifier);
        }
        base.Release();
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
