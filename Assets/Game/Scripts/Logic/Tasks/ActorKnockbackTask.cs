using Cysharp.Threading.Tasks;
using Game.GameActor;
using UnityEngine;

public class ActorKnockbackTask : SkillTask
{
    [SerializeField]
    private Vector2 jumpDirection;
    [SerializeField]
    private float jumpForce;
    public override async UniTask Begin()
    {
        ITarget target = Caster.GetComponent<DetectTargetHandler>().CurrentTarget;
        if (target != null) 
        { 
            int direction = target.GetPosition().x < Caster.GetPosition().x ? 1 : -1;
            Vector2 force = new Vector2(direction * jumpDirection.x,jumpDirection.y);
            Caster.MoveHandler.Jump(force, jumpForce);
        }
        await base.Begin();

        IsCompleted = true;
    }
}
