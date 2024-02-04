using BehaviorDesigner.Runtime.Tasks;
using Game.AI.State;
using UnityEngine;

[TaskCategory("Enemy")]
public class CastRandomSkill : Action
{
    [SerializeField] private SharedActor Human;
    [SerializeField] private int Index;

    public override TaskStatus OnUpdate()
    {
        var enemy = Human.Value;
        if (enemy)
        {
            bool castResult = enemy.SkillEngine.CastSkillRandom();
            if (castResult)
            {
                enemy.Machine.ChangeState<ActorCastSkillState>();
                return TaskStatus.Success;
            }
        }


        return TaskStatus.Failure;
    }
}