using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;


[TaskCategory("Enemy")]
public class HasSkillCast : HasConditional
{
    [SerializeField] private SharedActor Actor;

    public override TaskStatus OnUpdate()
    {
        var skillEngine = Actor.Value.SkillEngine;
        var skill = skillEngine.CanCastSkill;
        if (!skill)
        {
            return TaskStatus.Failure;
        }
        if (skill)
        {
            return TaskStatus.Success;
        }

        return base.OnUpdate();
    }
}