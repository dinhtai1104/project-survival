public class Enemy26SkillTask : SkillTaskShotBullet
{
    public ValueConfigSearch bullet2Angle;

    protected override void ReleaseBullet()
    {
        var target = Caster.FindClosetTarget();
        var angle = GameUtility.GameUtility.GetAngle(Caster, target);

        base.ReleaseBullet(angle);
        base.ReleaseBullet(angle + bullet2Angle.SetId(Caster.gameObject.name).FloatValue);
    }
}