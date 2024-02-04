using Cysharp.Threading.Tasks;
using Game.GameActor;
using System.Collections.Generic;

public class Enemy23SkillTask : Enemy19SkillTask
{
    public PatternBulletHellBase patternPrefab;
    protected async override void ReleaseBullet()
    {
        var pattern = PoolManager.Instance.Spawn(patternPrefab);
        pattern.transform.position = Caster.GetMidTransform().position;
        pattern.Prepare(bulletPrefab, Caster);
        await UniTask.Delay(100);
        pattern.SetSizeBullet(bulletSize.FloatValue);
        pattern.SetDmgBullet(new Stat(Caster.Stats.GetValue(StatKey.Dmg) * bulletDmg.FloatValue));
        pattern.SetMaxHit(bulletReflect.IntValue);

        var speed = new Stat(bulletVelocity.FloatValue);
        var listModi = new List<ModifierSource>() { new ModifierSource(speed) };
        Messenger.Broadcast(EventKey.PreFire, Caster, (BulletBase)null, listModi);
        pattern.SetSpeed(speed);

        pattern.Play();
    }
}