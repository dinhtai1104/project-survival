using BehaviorDesigner.Runtime.Tasks;
using Game.GameActor;

public class NormalAttackAction : Action
{
    ActorBase actor;
    public override void OnAwake()
    {
        base.OnAwake();
        actor = GetComponent<ActorBase>();
    }
    public override void OnStart()
    {
        base.OnStart();

    }
    public override TaskStatus OnUpdate()
    {
        //actor.AttackHandler.Trigger();
        actor.SkillEngine.CastSkill(0);
        return TaskStatus.Success;
    }
}
