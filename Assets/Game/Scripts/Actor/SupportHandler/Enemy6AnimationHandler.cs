public class Enemy6AnimationHandler : EnemyAnimationHandler
{
    public override void SetShoot()
    {
        anim.AnimationState.SetAnimation(1, "attack/combo_1_1", false);
        anim.AnimationState.AddAnimation(1, "attack/combo_1_2", true,0);
    }
}
