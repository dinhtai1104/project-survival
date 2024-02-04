using BehaviorDesigner.Runtime.Tasks;
using UnityEngine;

[TaskCategory("Enemy")]
public class HasAnySkillFinishVer2 : HasConditional
{
    [SerializeField] private SharedActor Actor;
    [SerializeField] private int Index;
    public override TaskStatus OnUpdate()
    {
        var skill = Actor.Value.SkillEngine.IsBusy;

        if (!skill)
        {
            return TaskStatus.Success;
        }

        return base.OnUpdate();
    }
}