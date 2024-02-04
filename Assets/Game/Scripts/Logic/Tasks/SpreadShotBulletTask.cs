using com.mec;
using System.Collections.Generic;
using UnityEngine;

public class SpreadShotBulletTask : RangeShotTask
{
    protected override IEnumerator<float> _Shot(Transform pos)
    {
        float angle = 0;
        float angleIncre = 360f / bulletNumber.IntValue;

        var target = Caster.FindClosetTarget();
        if (target == null)
        {
            target = GameController.Instance.GetMainActor();
        }
        if (target != null)
        {
            for (int i = 0; i < (bulletNumber.IntValue == 0 ? 1 : bulletNumber.IntValue); i++)
            {
                ReleaseBullet(pos, angle);
                angle += angleIncre;
                yield return Timing.WaitForSeconds(bulletDelayBtwShot.FloatValue);
            }
        }
        if (RunOnStart)
        {
            IsCompleted = true;
        }
    }
}