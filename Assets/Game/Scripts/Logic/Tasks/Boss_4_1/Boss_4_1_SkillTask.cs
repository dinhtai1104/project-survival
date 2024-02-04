using Spine;

public class Boss_4_1_SkillTask : SkillTaskShotBullet
{
    public ValueConfigSearch chaseRate;
    protected override void ReleaseBullet()
    {
        var target = Caster.FindClosetTarget();
        if (target == null) return;

        var angle = GameUtility.GameUtility.GetAngle(Caster, target);

        var bullet = ReleaseBullet(angle);
        bullet.transform.localScale = bulletSize.Vector2Value;
        bullet.Movement.TrackTarget(chaseRate.FloatValue, target.GetMidTransform());
    }
}