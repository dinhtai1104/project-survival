using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.GameActor;
using UnityEngine;

public class RangeThrowBulletTask : RangeShotTask
{
    protected override void ReleaseBullet(Transform pos)
    {
        var target = Caster.FindClosetTarget();
        if (target != null)
        {
            var bullet = PoolManager.Instance.Spawn(bulletPrefab);
            bullet.transform.position = pos.position;
            bullet.SetCaster(Caster);
            bullet.SetMaxHit(1);
            bullet.DmgStat = new Stat(Caster.Stats.GetValue(StatKey.Dmg) * bulletDmg.SetId(Caster.gameObject.name).FloatValue);
            bullet.transform.localScale = Vector3.one * bulletSize.SetId(Caster.gameObject.name).FloatValue;

            var dir = GameUtility.GameUtility.CalcBallisticVelocityVector(pos.position, target.GetPosition(), 45, bulletVelocity.SetId(Caster.gameObject.name).FloatValue);
            bullet.Movement.Move(new Stat(0), dir);
            bullet.Play();
        }

        IsCompleted = true;
    }
}