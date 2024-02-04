using BehaviorDesigner.Runtime.Tasks;
using Game.AI.State;
using UnityEngine;

[TaskCategory("Enemy")]
public class SkillAction : Action
{
    [SerializeField] private SharedActor Human;
    [SerializeField] private int Index;

    public bool IsChangeToCastSkill = true;

    public override TaskStatus OnUpdate()
    {
        var enemy = Human.Value;
        if (enemy)
        {
            bool castResult = enemy.SkillEngine.CastSkill(Index);
            if (castResult)
            {
                if (IsChangeToCastSkill)
                {
                    enemy.Machine.ChangeState<ActorCastSkillState>();
                }
                return TaskStatus.Success;
            }
        }


        return TaskStatus.Failure;
    }
}