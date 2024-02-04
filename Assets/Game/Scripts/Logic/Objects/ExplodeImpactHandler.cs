using Game.GameActor;
using UnityEngine;

public class ExplodeImpactHandler : ImpactHandler
{
    

    public override void Impact(ITarget target)
    {
        var expl = PoolManager.Instance.Spawn(damageExplode);
        expl.transform.position = Base.GetTransform().position;
        expl.Init(Base.Caster);
        expl.SetDmg(Base.Caster.Stats.GetValue(StatKey.Dmg) * explodeDmgMul);
        expl.SetSize(explodeSize);
        expl.Explode();
    }
    public DamageExplode damageExplode;
    private float explodeDmgMul;
    private float explodeSize;

    public void SetDmgExplodeMul(float dmgExplodeMul)
    {
        this.explodeDmgMul = dmgExplodeMul;
    }
    public void SetExplodeSize(float size)
    {
        this.explodeSize = size;
    }

  
    

}
