using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace Game.GameActor
{
    public class GroundMoveHandler : MoveHandler
    {
        [SerializeField] private string VFX_Jump = "VFX_SmokeJump";
        [SerializeField] private string VFX_DoubleJump = "VFX_DoubleJump";
        [SerializeField] private string VFX_OnLand = "VFX_SmokeJumpTouchGround";

        [SerializeField]
        private Vector2 groundBoxCheckSize = new Vector2(0.2f, 0.1f);
        [SerializeField]
        private float groundMaxDistanceCheck = 0.15f;

        LayerMask groundMask;
        protected Vector2 velocity;
        protected Vector2 boostVelocity;
        bool isFallingDown = false;
        protected int jumpIndex = 0;
        protected int maxJump=1;
        protected float lastJump = 0;

        protected float jumpForce = 3;
        float gravityScale = 3;
        public override void SetUp(Character character)
        {
            base.SetUp(character);
            groundMask = LayerMask.GetMask("Platform", "Ground");

        }
        public override void ClearBoostVelocity()
        {
            base.ClearBoostVelocity();
            boostVelocity = Vector3.zero;
        }
        void HandleLanding()
        {
            jumpIndex = 0;
            if (character.soundData != null && character.soundData.landSFXs.Length != 0)
                character.SoundHandler.PlayOneShot(character.soundData.landSFXs[UnityEngine.Random.Range(0, character.soundData.landSFXs.Length)], 1);
            isFallingDown = false;
            if (isMoving)
            {
                character.AnimationHandler.SetLand();
            }
            else
            {
                character.GetRigidbody().velocity = new Vector2(0, character.GetRigidbody().velocity.y);
                character.AnimationHandler.SetLandIdle();
            }

            Pool.GameObjectSpawner.Instance.Get(VFX_OnLand, res =>
            {
                res.GetComponent<Effect.EffectAbstract>().Active(character.GetPosition(), character.transform.localScale.x);
            });

            onCharacterLanded?.Invoke(this);
        }
        public override void AddForce(Vector2 force)
        {
            if (Locked) return;
            lastJump = Time.time;
          
            character.GetRigidbody().AddForce(force, ForceMode2D.Impulse);
        }
       
        public override void Move(Vector2 moveDirection, float ratio)
        {
            if (Locked) return;
            if (MoveSpeed == 0 || character.IsDead()) return;
            moveDirection.y = 0;
            moveDirection.Normalize();
            if (!isMoving)
            {
                isMoving = true;
                //if (isGrounded)
                    character.AnimationHandler.SetRun();
               
                character.SetFacing(moveDirection.x > 0 ? 1 : -1);
            }
            else if (isMoving && move.x * moveDirection.x <= 0)
            {
                character.SetFacing(moveDirection.x == 0 ? move.x < 0 ? 1 : -1 : moveDirection.x > 0 ? 1 : -1);
            }

            move = moveDirection * MoveSpeed;
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
            move.y = character.GetRigidbody().velocity.y;
            character.GetRigidbody().velocity = move;
            if (!character.IsDead() && isGrounded)
                character.AnimationHandler.SetIdle();
           

        }
        public override void Stop(bool isUseAnimIdle)
        {
            if (!isUseAnimIdle)
            {
                if (!isMoving) return;
                isMoving = false;
                lastMove = move;
                move.x = 0;
                move.y = character.GetRigidbody().velocity.y;
                character.GetRigidbody().velocity = move;
            }
            else
            {
                base.Stop(isUseAnimIdle);
            }
        }
        public override void Jump(Vector2 direction,float power)
        {
            //if (Locked) return;
            lastJump = Time.time;
            boostVelocity = direction*power;
        }
        public override void Jump()
        {
            if (/*!isMoving ||*/ character == null || character.IsStun() || character.IsDead()) return;

            if (!IsGrounded() && jumpIndex >= maxJump) return;
            if (Time.time - lastJump < 0.25f) return;

            if (!IsGrounded())
            {
                Pool.GameObjectSpawner.Instance.Get(VFX_DoubleJump, res =>
                {
                    res.GetComponent<Effect.EffectAbstract>().Active(character.GetPosition(), character.transform.localScale.x);
                });
            }
            else
            {
                Pool.GameObjectSpawner.Instance.Get(VFX_Jump, res =>
                {
                    res.GetComponent<Effect.EffectAbstract>().Active(character.GetPosition(), character.transform.localScale.x);
                });
            }
            // Invoke start jump
            onStartFallJump?.Invoke();

            isGrounded = false;
            lastJump = Time.time;
            character.AnimationHandler.SetJump(jumpIndex);
            if (character.soundData != null && character.soundData.jumpSFXs.Length != 0)
                character.SoundHandler.PlayOneShot(character.soundData.jumpSFXs[UnityEngine.Random.Range(0, character.soundData.jumpSFXs.Length)], 1);

            // prevent duplicate the y velocity
            character.GetRigidbody().velocity = new Vector2(character.GetRigidbody().velocity.x, 0);
            //add force

            float jumpForce = Mathf.Sqrt(this.jumpForce * -2 * (Physics2D.gravity.y * gravityScale * character.GetRigidbody().mass));

            character.GetRigidbody().AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            jumpIndex++;
        }

        public override void ReleaseJump()
        {
            character.GetRigidbody().velocity = new Vector2(character.GetRigidbody().velocity.x, character.GetRigidbody().velocity.y / 2f);

        }

        public override void HoldJump()
        {
        }
        public override void Descend()
        {
        }
        Vector2 zero = Vector2.zero;

        private float lastPosY = 0;
        protected override void FixedUpdate()
        {
            if (Locked) return;
            if (character == null || character.IsKnockBack() || character.IsDead()) return;

            if (!isGrounded && Time.time - lastJump > 0.2f)
            {
                isGrounded = IsGrounded();
                if (isGrounded)
                {
                    if (character.soundData != null && character.soundData.landSFXs.Length != 0)
                        character.SoundHandler.PlayOneShot(character.soundData.landSFXs[UnityEngine.Random.Range(0, character.soundData.landSFXs.Length)], 1);

                    HandleLanding();
                    character.AnimationHandler.SetLand();
                    onCharacterLanded?.Invoke(this);
                    onFallGroundJump?.Invoke();
                }
            }
            else
            {
                isGrounded = IsGrounded();
            }

            if (!isGrounded && Time.time - lastJump > 1f)
            {
                onLoopJump?.Invoke();
            }



            velocity.y = character.GetRigidbody().velocity.y;
            velocity.x = move.x * (SpeedMultiply);
            if (isGrounded == false) {
                if (velocity.y < 0)
                {
                    onStartFallJump?.Invoke();
                }
                if (velocity.y < -2)
                {
                    onFallLoopJump?.Invoke();
                }
            }

            //set player velocity
            Vector3 v = (velocity + boostVelocity);
            v.x *= (unscaleTime ? 1 : GameTime.Controller.TIME_SCALE);
            character.GetRigidbody().velocity = v;
            boostVelocity = Vector2.Lerp(boostVelocity, zero, 0.1f);

            lastPosY = character.GetPosition().y;
        }

        //
        public bool IsGrounded()
        {
            var currentPosY = character.GetPosition().y;
            if (Physics2D.BoxCast(character.GetPosition(), groundBoxCheckSize, 0, -character.GetTransform().up, groundMaxDistanceCheck, groundMask))
            {
                return true;
            }

            return false;
        }

       
    }
}