using BehaviorDesigner.Runtime.Tasks;
using Game.GameActor;

public class CheckSkillBusyDecision : Conditional
{
    ActorBase actor;
    public bool result = true;
    public override void OnAwake()
    {
        base.OnAwake();
        actor = GetComponent<ActorBase>();
    }
    public override TaskStatus OnUpdate()
    {
        return actor.SkillEngine.GetSkill(0).IsExecuting == result ? TaskStatus.Success : TaskStatus.Failure;
    }
}