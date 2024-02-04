using Game.GameActor;
using UnityEngine;

public class BombObject : BulletSimpleDamageObject, IRadiusRequire
{
    
    [SerializeField] private DamageExplode damageExplodePrefab;

    public Stat Radius { set; get; }

    protected override void OnTriggerHit(Collider2D collider, ITarget target)
    {
        var explosion = PoolManager.Instance.Spawn(damageExplodePrefab);
        explosion.transform.position = transform.position;
        explosion.Init(Caster);
        explosion.SetSize(Radius.Value);
        explosion.SetDmg(DmgStat.Value);
        explosion.Explode();
    }
}