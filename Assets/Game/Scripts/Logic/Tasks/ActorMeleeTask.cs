using Cysharp.Threading.Tasks;
using Game.GameActor;
using Game.Tasks;

public class ActorMeleeTask : SkillTask
{
    public override async UniTask Begin()
    {
        await base.Begin();
        var target = Caster.Sensor.CurrentTarget;
        if (target != null)
        {
            //UnityEngine.Debug.DrawLine(Caster.GetMidTransform().position, target.GetMidTransform().position, UnityEngine.Color.blue, 2);
            Caster.AttackHandler.Trigger((target.GetMidTransform().position - Caster.GetMidTransform().position).normalized, target);
            int direction = target.GetPosition().x > Caster.GetPosition().x ? 1 : -1;
            ((Character)Caster).SetFacing(direction);
        }
        IsCompleted = true;
    }
}
