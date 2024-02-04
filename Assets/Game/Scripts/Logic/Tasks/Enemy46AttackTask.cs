using Cysharp.Threading.Tasks;
using Game.GameActor;
using UnityEngine;

public class Enemy46AttackTask : SkillTask
{
    public ValueConfigSearch TotalBullet,SpreadAngle;
    public override async UniTask Begin()
    {
        TotalBullet.SetId(Caster.gameObject.name);
        SpreadAngle.SetId(Caster.gameObject.name);
        await base.Begin();
        Attack();
        

    }

    async UniTask Attack()
    {
        var target = Caster.Sensor.CurrentTarget;
        if(target==null)
        {
            IsCompleted = true;
            return;
        }
        int skippedBullet = UnityEngine.Random.Range(-TotalBullet.IntValue / 2, TotalBullet.IntValue / 2+1);
        for(int i = -TotalBullet.IntValue/2; i < TotalBullet.IntValue/2f; i++)
        {
            Logger.Log("SHOOT: " + i + " " + skippedBullet);
            if (i == skippedBullet) continue;

            ReleaseBullet(Quaternion.AngleAxis(i*SpreadAngle.IntValue,Vector3.forward)*(target.GetMidTransform().position- Caster.WeaponHandler.DefaultAttackPoint.position).normalized,target);
            await UniTask.WaitUntil(() => !Caster.AttackHandler.isBusy);
        }

        IsCompleted = true;

    }
    void ReleaseBullet(Vector3 direction,ITarget target)
    {
        Logger.Log("=>>> " + direction);
        Caster.AttackHandler.Trigger(direction, target);
    }

}