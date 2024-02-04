using Game.GameActor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMoveHandler : MoveHandler
{
    private Vector2 destination;
    float followSpeed;
    public override void SetUp(Character character)
    {
        base.SetUp(character);

    }
    public override void MoveToPosition(Vector2 position,float followSpeed)
    {
        if (!isMoving&& Vector3.Distance(character.GetPosition(), destination) >= 0.2f)
        {
            isMoving = true;
            character.AnimationHandler.SetRun();
           
            this.followSpeed = followSpeed;
        }
        destination = position;

    }
    public override void Move(Vector2 moveDirection, float ratio)
    {
       
        //moveDirection.Normalize();
        //if (!isMoving)
        //{
        //    isMoving = true;
        //    character.AnimationHandler.SetRun();

        //   
        //    character.SetFacing(moveDirection.x > 0 ? 1 : -1);
        //}
        //else if (isMoving && move.x * moveDirection.x <= 0)
        //{
        //    character.SetFacing(moveDirection.x == 0 ? move.x < 0 ? 1 : -1 : moveDirection.x > 0 ? 1 : -1);
        //}

        //move = moveDirection * MoveSpeed * ratio;
        //if (move.SqrMagnitude() != 0)
        //{
        //    lastMove = move;
        //}
    }

    public override void Stop()

    {
        if (!isMoving) return;
        isMoving = false;
        lastMove = move;
        if (!character.IsDead())
            character.AnimationHandler.SetIdle();
       

    }
    public override void Jump(Vector2 direction, float power)
    {
    }
    public override void Jump()
    {
    }

    public override void ReleaseJump()
    {
    }

    public override void HoldJump()
    {
    }
    public override void Descend()
    {
    }
    protected override void FixedUpdate()
    {
        if (character == null || character.IsStun() || character.IsDead()||!isMoving) return;


        if (Vector3.Distance(character.GetPosition(), destination) < 0.2f)
        {
            Stop();
        }
        else
        {
            character.SetPosition(Vector3.Lerp(character.GetPosition(), destination, followSpeed));
        }
    }


}
