
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Enemy")]
public class HasSkill : HasConditional
{
    [SerializeField] private int Index;
    [SerializeField] private SharedActor Actor;
    public override TaskStatus OnUpdate()
    {
        var skillEngine = Actor.Value.SkillEngine;
        var skill = skillEngine.GetSkill(Index);
        if (skill == null)
        {
            return TaskStatus.Failure;
        }
        if (skill.CanCast)
        {
            return TaskStatus.Success;
        }

        return base.OnUpdate();
    }
}