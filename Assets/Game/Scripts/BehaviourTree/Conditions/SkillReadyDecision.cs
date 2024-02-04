using BehaviorDesigner.Runtime.Tasks;
using Game.GameActor;

public class SkillReadyDecision : Conditional
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
        return actor.SkillEngine.CanCastSkill==result ? TaskStatus.Success : TaskStatus.Running;
    }
}
