using BehaviorDesigner.Runtime.Tasks;
using Game.GameActor;

public class SkillIndexReadyDecision : Conditional
{
    public int Skill;

    ActorBase actor;
    public bool result = true;
    public override void OnAwake()
    {
        base.OnAwake();
        actor = GetComponent<ActorBase>();
    }
    public override TaskStatus OnUpdate()
    {
        var skill = actor.SkillEngine.GetSkill(Skill);
        if (skill == null) return TaskStatus.Running;
        return skill.CanCast ? TaskStatus.Success : TaskStatus.Running;
    }
}
