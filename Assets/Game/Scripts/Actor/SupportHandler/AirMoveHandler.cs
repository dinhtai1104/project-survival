using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GameActor
{
    public class AirMoveHandler : MoveHandler
    {
        protected Vector2 velocity;
        protected Vector2 boostVelocity;
        public override void SetUp(Character character)
        {
            base.SetUp(character);

        }

        public override void Move(Vector2 moveDirection, float ratio)
        {
            if (MoveSpeed == 0) return;
            moveDirection.Normalize();
            if (!isMoving)
            {
                isMoving = true;
                character.AnimationHandler.SetRun();

                character.SetFacing(moveDirection.x > 0 ? 1 : -1);
            }
            else if (isMoving && move.x * moveDirection.x <= 0)
            {
                character.SetFacing(moveDirection.x == 0 ? move.x < 0 ? 1 : -1 : moveDirection.x > 0 ? 1 : -1);
            }

            move = moveDirection * MoveSpeed*ratio;
            if (move.SqrMagnitude() != 0)
            {
                lastMove = move;
            }
        }

        public override void Stop()

        {
            if (!isMoving) return;
            isMoving = false;
            lastMove = move;
            move.x = 0;
            move.y = 0;
            character.GetRigidbody().velocity = move;
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
        Vector2 zero = Vector2.zero;
        protected override void FixedUpdate()
        {
            if (character == null || character.IsStun() || character.IsDead()) return;


            velocity.y = move.y * (SpeedMultiply);
            velocity.x = move.x * (SpeedMultiply);

            //set player velocity
            character.GetRigidbody().velocity = (velocity  + boostVelocity) * (unscaleTime ? 1 : GameTime.Controller.TIME_SCALE);

            boostVelocity = Vector2.Lerp(boostVelocity, zero, 0.1f);

        }

      


    }
}