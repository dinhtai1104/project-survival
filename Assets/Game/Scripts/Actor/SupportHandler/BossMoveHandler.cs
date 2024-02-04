using Game.GameActor;
using UnityEngine;

public class BossMoveHandler : GroundMoveHandler
{

    public override void Jump(Vector2 direction, float power)
    {
        this.jumpForce = power;
        //base.Jump(direction, power);
        Jump();
    }
}