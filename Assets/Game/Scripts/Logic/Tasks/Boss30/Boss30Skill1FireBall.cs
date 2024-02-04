using Game.GameActor;
using UnityEngine;

public class Boss30Skill1FireBall : BulletSimpleDamageObject, IRadiusRequire
{
    public DamageExplode damageExplodePrefab;
    public Stat Radius { set; get; }

    protected override void OnTriggerHit(Collider2D collider, ITarget target)
    {
        var eff = PoolManager.Instance.Spawn(damageExplodePrefab);
        eff.transform.position = transform.position;
        eff.Init(Caster);
        eff.SetSize(Radius.Value);
        eff.SetDmg(DmgStat.Value);

        eff.Explode();
    }
}