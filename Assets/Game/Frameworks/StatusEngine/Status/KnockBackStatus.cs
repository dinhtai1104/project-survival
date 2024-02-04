using UnityEngine;

public class KnockBackStatus : BaseStatus
{
    public Vector3 force;
    public void AddForce(Vector3 force)
    {
        Target.MoveHandler.AddForce(force);
    }
    public override void OnUpdate(float deltaTime)
    {
    }

    public override void SetCooldown(float cooldown)
    {
    }

    public override void SetDmgMul(float dmgMul)
    {
    }
}
