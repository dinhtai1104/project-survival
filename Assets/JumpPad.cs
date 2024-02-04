using Game.GameActor;
using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public ValueConfigSearch Force;
    public ECharacterType targetType;
    public MMF_Player activeFb;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ITarget target = collision.GetComponentInParent<ITarget>();
        if(target!=null && !target.IsDead() && targetType.Contains(target.GetCharacterType()))
        {
            Launch((ActorBase)target);
        }
    }

    void Launch(ActorBase target)
    {
        target.GetRigidbody().velocity = new Vector2(target.GetRigidbody().velocity.x, 0);
        target.MoveHandler.ResetJump();
        target.MoveHandler.AddForce(Vector2.up*Force.FloatValue);

        activeFb?.PlayFeedbacks();
    }
}
