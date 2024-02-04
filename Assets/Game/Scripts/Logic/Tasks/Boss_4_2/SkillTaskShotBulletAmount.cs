public class SkillTaskShotBulletAmount : SkillTaskShotBullet
{
    public ValueConfigSearch amountBullet;
    public ValueConfigSearch angle;


    protected override void ReleaseBullet()
    {
        var angle = GetAngleToTarget();
        for (int i = -amountBullet.IntValue; i <= amountBullet.IntValue; i++)
        {
            var bullet = ReleaseBullet(angle + i * this.angle.FloatValue);

        }
    }
}