
using BehaviorDesigner.Runtime.Tasks;
using Game.AI.State;
using TypeReferences;

[TaskCategory("Enemy/Fsm")]
public class SetEnemy7003PatrolStateAction : SetStateAction
{
    protected override ClassTypeReference stateType => typeof(ActorEnemy7003PatrolState);
}


[TaskCategory("Enemy/Fsm")]
public class SetIdleStateAction : SetStateAction
{
    protected override ClassTypeReference stateType => typeof(ActorIdleState);
}
[TaskCategory("Enemy/Fsm")]
public class SetCastSkillStateAction : SetStateAction
{
    protected override ClassTypeReference stateType => typeof(ActorCastSkillState);
}
[TaskCategory("Enemy/Fsm")]
public class SetRandomMoveStateAction : SetStateAction
{
    protected override ClassTypeReference stateType => typeof(ActorRandomMoveState);
}

[TaskCategory("Enemy/Fsm")]
public class SetMoveToTargetStateAction : SetStateAction
{
    protected override ClassTypeReference stateType => typeof(ActorRunToTargetState);
}

[TaskCategory("Enemy/Fsm")]
public class SetFlyRandomStateAction : SetStateAction
{
    protected override ClassTypeReference stateType => typeof(ActorRandomFlyState);
}
[TaskCategory("Enemy/Fsm")]
public class SetFlyTurnAroundStateAction : SetStateAction
{
    protected override ClassTypeReference stateType => typeof(ActorFlyTurnAround);
}
[TaskCategory("Enemy/Fsm")]
public class SetRunTurnAroundStateAction : SetStateAction
{
    protected override ClassTypeReference stateType => typeof(ActorRunAroundState);
}
[TaskCategory("Enemy/Fsm")]
public class SetandomFlyInAreaItselfStateStateAction : SetStateAction
{
    protected override ClassTypeReference stateType => typeof(ActorRandomFlyInAreaItselfState);
}