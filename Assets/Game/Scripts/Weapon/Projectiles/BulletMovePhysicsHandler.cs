using UnityEngine;

public class BulletMovePhysicsHandler : BulletMoveHandler
{
    public override void Move(Stat speed, Vector2 move)
    {
        base.Move(speed, move);
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        rb.AddForce(move, ForceMode2D.Impulse);
    }
}
