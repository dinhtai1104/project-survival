using UnityEngine;

public class ExplodeBullet : SmallBullet
{
    //public DamageExplode damageExplode;
    //private float explodeDmgMul;
    //private float explodeSize;

    //public void SetDmgExplodeMul(float dmgExplodeMul)
    //{
    //    this.explodeDmgMul = dmgExplodeMul;
    //}
    //public void SetExplodeSize(float size)
    //{
    //    this.explodeSize = size;
    //}

    //protected override void Impact(Collider2D collision)
    //{
    //    base.Impact(collision);
    //    var expl = PoolManager.Instance.Spawn(damageExplode);
    //    expl.transform.position = transform.position;
    //    expl.Init(character);
    //    expl.SetDmg(base.character.Stats.GetValue(StatKey.DPS) * explodeDmgMul);
    //    expl.SetSize(explodeSize);
    //    expl.Explode();
    //}
}