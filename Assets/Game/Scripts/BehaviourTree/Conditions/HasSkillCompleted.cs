
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Enemy")]
public class HasSkillCompleted : HasConditional
{
    [SerializeField] private SharedActor Actor;
    [SerializeField] private int Index;
    public override TaskStatus OnUpdate()
    {
        var skill = Actor.Value.SkillEngine.GetSkill(Index);

        if (!skill.IsExecuting)
        {
            return TaskStatus.Success;
        }

        return base.OnUpdate();
    }
}