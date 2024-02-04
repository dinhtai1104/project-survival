using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace Game.GameActor
{
    public class PlayerMoveHandler : MoveHandler
    {
        private const string SmokeJumpTouchGroundEffect = "VFX_SmokeJumpTouchGround";
        private const string StartMoveEffect = "VFX_StartMove";
        private const string DoubleJumpEffect = "VFX_DoubleJump";
        private const string SmokeJumpEffect = "VFX_SmokeJump";
        [SerializeField]
        private Vector2 descendThreshold = new Vector2(0.5f, 0.5f);
        [SerializeField]
        private Vector2 groundBoxCheckSize = new Vector2(0.2f, 0.1f);
        [SerializeField]
        private float groundMaxDistanceCheck = 0.15f;

        [SerializeField]
        private Vector2 wallBoxCheckSize = new Vector2(0.1f, 0.2f);
        [SerializeField]
        private float wallMaxDistanceCheck = 0.15f;
        [SerializeField]
        private float maxFallSpeed=-1,dropThreshold=-0.1f;

        public float moveAcceleration = 0.2f;

        LayerMask groundMask,wallMask,softPlatformMask;
       
        Collider2D playerCollider;

        float lastClimbTime = 0;

        protected Vector2 velocity,additionalForce;
        protected float boostSpeed = 0;

        Vector2 wallVector;
    
        public float climbDrag;
        public float gravityScale;

        private Effect.EffectAbstract climbPS;
        private Keyframe[] accelerationKeys;
        public override void SetUp(Character character)
        {
            base.SetUp(character);
            groundMask = LayerMask.GetMask("Platform", "Ground");
            wallMask = LayerMask.GetMask("Ground");
            softPlatformMask = LayerMask.GetMask("Platform");

            character.Stats.AddListener(StatKey.JumpCount, OnChangeStat);
            character.Stats.AddListener(StatKey.ClimbDrag, OnChangeClimbDrag);


            maxJump = (int)character.Stats.GetValue(StatKey.JumpCount);
            climbDrag = character.Stats.GetValue(StatKey.ClimbDrag);

            gravityScale = character.GetRigidbody().gravityScale;
            playerCollider = character.GetComponent<Collider2D>();

            Pool.GameObjectSpawner.Instance.Get("VFX_MoveOnGround", res =>
            {
                climbPS = res.GetComponent<Effect.EffectAbstract>();
            });

            accelerationKeys = accelerationCurve.keys;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ReleaseClimb();
            ReleaseJump();
        }
        private void OnChangeClimbDrag(float newValue)
        {
            climbDrag = newValue;
        }

        private void OnChangeStat(float newValue)
        {
            maxJump = (int)character.Stats.GetValue(StatKey.JumpCount);
        }

        private Vector3 left = new Vector3(0, 180);
        private Vector3 right = new Vector3(0, 0);
        public override void Move(Vector2 moveDirection, float ratio)
        {
            if (Locked) return;
            if (MoveSpeed == 0 || character.IsDead()) return;
            if (!isMoving)
            {
                acceTime = 0;

                isMoving = true;
                if (isGrounded)
                {
                    character.AnimationHandler.SetRun();
                    Pool.GameObjectSpawner.Instance.Get(StartMoveEffect, res =>
                    {
                        res.GetComponent<Effect.EffectAbstract>().Active(character.GetPosition()).SetRotation(moveDirection.x > 0 ? right : left);
                    });
                }
                Messenger.Broadcast(EventKey.ContinueMovement, character as ActorBase);

                onCharacterMoved?.Invoke(this);
               
                character.SetFacing(moveDirection.x > 0 ? 1 : -1);
            }
            else if (isMoving && move.x * moveDirection.x <= 0)
            {
                character.SetFacing(moveDirection.x == 0 ? move.x < 0 ? 1 : -1 : moveDirection.x > 0 ? 1 : -1);
            }

            //handle Descending
            //0.95 threshold
            if (!isDescending &&moveDirection.y*ratio < descendThreshold.y  && HasTouchSoftGround() && IsGrounded() )
            {

                Descend();
                return;
            }

            //set move velocity
            move = moveDirection.normalized;
            move.x = (move.x < -0.05f ? -1 : (move.x>0.05f?1:move.x*10))* MoveSpeed;
            if (move.SqrMagnitude() != 0)
            {
                lastMove = move;
            }
        }
        public override void AddForce(Vector2 force)
        {
            if (Locked) return;
            //lastJump = Time.time;

            character.AnimationHandler.SetJump(0);
            //additionalForce =force;
            character.GetRigidbody().AddForce(force, ForceMode2D.Impulse);
        }
        public override void Stop()

        {
            if (!isMoving) return;
            isMoving = false;
            lastMove = move;
            boostSpeed = 0;
            move.x = 0;
            move.y = character.GetRigidbody().velocity.y;
            acceTime = accelerationKeys[2].time;
            character.GetRigidbody().velocity = move;
            if (!character.IsDead() && isGrounded)
                character.AnimationHandler.SetIdle();

            if (isClimbing)
            {
                ReleaseClimb();
            }
            Messenger.Broadcast(EventKey.StopMovement, character as ActorBase);

            onCharacterStopped?.Invoke(this);

        }
        void ReleaseClimb()
        {
            if (!isClimbing) return;
            if(climbPS!=null)
                climbPS.Stop();
            BonusDrag=0;
            isClimbing = false;
        }
        public override void Jump(Vector2 direction,float power)
        {
        }
        public override void Jump()
        {
            if (Locked) return;
            isHoldingJump = true;
            isFallingDown = false;
            isFallingFromLastJump = false;
            if (character == null || character.IsStun() || character.IsDead() || Time.time - lastJump < 0.25f || (!IsGrounded() && jumpIndex >= maxJump)) return;

            BonusDrag = 0;
            ReleaseClimb();

            if (!IsGrounded())
            {
                Pool.GameObjectSpawner.Instance.Get(DoubleJumpEffect, res =>
                {
                    res.GetComponent<Effect.EffectAbstract>().Active(character.GetPosition());
                });
            }
            else
            {
                Pool.GameObjectSpawner.Instance.Get(SmokeJumpEffect, res =>
                {
                    res.GetComponent<Effect.EffectAbstract>().Active(character.GetPosition());
                });
            }
           
            isGrounded = false;
            lastJump = Time.time;
            character.AnimationHandler.SetJump(jumpIndex);
            if (character.soundData != null && character.soundData.jumpSFXs.Length != 0)
                character.SoundHandler.PlayOneShot(character.soundData.jumpSFXs[UnityEngine.Random.Range(0, character.soundData.jumpSFXs.Length)], 1);

            // prevent duplicate the y velocity
            character.GetRigidbody().velocity = new Vector2(character.GetRigidbody().velocity.x, 0);
            //add force

            float jumpForce = Mathf.Sqrt(this.JumpForce * -2 * (Physics2D.gravity.y * gravityScale * character.GetRigidbody().mass));

            character.GetRigidbody().AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);


            if (Time.time - lastClimbTime < 0.2f && wallVector.x * move.x < 0)
            {
                boostSpeed = 3;
            }
            jumpIndex++;
            Messenger.Broadcast(EventKey.OnCharacterJump, (ActorBase)character);

        }

        public override void ReleaseJump()
        {
            isHoldingJump = false;
            isFallingFromLastJump = false;
            if (character.GetRigidbody().velocity.y > 0)
            {
                character.GetRigidbody().velocity = new Vector2(character.GetRigidbody().velocity.x, character.GetRigidbody().velocity.y / 2f);
            }

            onCharacterJumpReleased?.Invoke(this);
        }

        public override void HoldJump()
        {
        }


        bool isDescending = false;
        public override  void Descend()
        {
            isDescending = true;
            Action().ContinueWith(()=> { isDescending = false; }).Forget();

            async UniTask Action()
            {
                Collider2D collider = softPlatformCollider;
                onCharacterDescend?.Invoke(this);
                character.GetRigidbody().velocity = new Vector2(character.GetRigidbody().velocity.x, 0);
                Physics2D.IgnoreCollision(playerCollider, collider, true);
                move.y = 0;
                await UniTask.Delay(400,delayTiming:PlayerLoopTiming.FixedUpdate);
                //float timer = 0.4f;
                //while (timer > 0)
                //{
                //    timer -= Time.fixedDeltaTime;
                //    if (IsGrounded() || isHoldingJump)
                //    {
                //        break;
                //    }
                //    await UniTask.Yield();
                //}
                Physics2D.IgnoreCollision(playerCollider, collider, false);
                softPlatformCollider = null;
                Messenger.Broadcast(EventKey.CharacterDesend);
            }
          



        }

        public override void ResetJump()
        {
            jumpIndex = 0;
        }

        [SerializeField]
        private AnimationCurve accelerationCurve;
        float acceTime = 0;

        protected override void FixedUpdate()
        {
            if (Locked) return;
            if (character == null || character.IsStun() || character.IsDead() ) return;

            if (!isGrounded && Time.time-lastJump>0.2f)
            {
                isGrounded = IsGrounded();
                if (isGrounded)
                {
                    move.y = 0;
                    HandleLanding();

                  
                }
                else
                {
                    if (isMoving && !isClimbing && Time.time - lastClimbTime > 0.35f && IsWall() && climbDrag != 0)
                    {
                        HandleClimb();
                    }
                    else if (isClimbing && !IsWall())
                    {
                        ReleaseClimb();
                    }
                    else if (isClimbing)
                    {
                        lastClimbTime = Time.time;
                    }
                    else
                    {
                        if (!isClimbing && !isFallingDown && character.GetRigidbody().velocity.y < 0)
                        {
                            isFallingDown = true;
                            character.AnimationHandler.SetJumpDown();
                        }
                     
                    }
                }
            }
            else
            {
                isGrounded = IsGrounded();
            }




            character.GetRigidbody().gravityScale =gravityScale*( character.GetRigidbody().velocity.y < dropThreshold ?1.5f:1);
            velocity.y = character.GetRigidbody().velocity.y<0?Mathf.Max(character.GetRigidbody().velocity.y,maxFallSpeed):character.GetRigidbody().velocity.y;

            float velX = move.x * (SpeedMultiply + boostSpeed) * accelerationCurve.Evaluate(move.x != 0 ? Mathf.Min(acceTime, accelerationKeys[1].time) : Mathf.Max(acceTime, accelerationKeys[3].time));
            velocity.x = Mathf.Abs(velX)<Mathf.Abs(character.GetRigidbody().velocity.x)?character.GetRigidbody().velocity.x:velX;
            acceTime += Time.fixedDeltaTime;

            //set player velocity
            character.GetRigidbody().velocity = Vector2.ClampMagnitude(velocity+additionalForce,60);

            //HandleJetpack();
            HandleFallDrag();

            //add ons
            foreach(var addOn in addOnInstances)
            {
                addOn.OnFixedUpdate(this);
            }

            //
            boostSpeed -= Time.fixedDeltaTime * 10;
            boostSpeed = boostSpeed <= 0 ? 0 : boostSpeed;
            if (Mathf.Abs(additionalForce.x) >0.1f || Mathf.Abs(additionalForce.y) > 0.1f)
            {
                additionalForce = Vector2.Lerp(additionalForce, zero, 0.3f);
            }
           

        }
        Vector2 zero = Vector2.zero;
        bool isDragged = false;
        void HandleFallDrag()
        {
            if (character.PropertyHandler.GetProperty(EActorProperty.IsTrapedBySpiderWeb, 0L) != 0)
            {
                return;
            }
            if (character.GetRigidbody().velocity.y < 0 && character.PropertyHandler.GetProperty(EActorProperty.FallDrag, 0) != 0)
            {
                character.GetRigidbody().gravityScale = gravityScale*character.PropertyHandler.GetProperty(EActorProperty.FallDrag, 0)/100f;
                isDragged = true;
            }
           
        }
        void HandleLanding()
        {
            ResetJump();
            ReleaseClimb();
            if (character.soundData != null && character.soundData.landSFXs.Length != 0)
                character.SoundHandler.PlayOneShot(character.soundData.landSFXs[UnityEngine.Random.Range(0, character.soundData.landSFXs.Length)], 1);
            isFallingDown = false;
            //currentFuel = MaxFuel;
            if (isMoving)
            {
                character.AnimationHandler.SetLand();
            }
            else
            {
                character.GetRigidbody().velocity = new Vector2(0, character.GetRigidbody().velocity.y);
                character.AnimationHandler.SetLandIdle();
            }

            if (character.AnimationHandler.GetAnimator().gameObject.activeSelf)
            {
                Pool.GameObjectSpawner.Instance.Get(SmokeJumpTouchGroundEffect, res =>
                {
                    res.GetComponent<Effect.EffectAbstract>().Active(character.GetPosition());
                });
            }

            onCharacterLanded?.Invoke(this);
            Messenger.Broadcast(EventKey.CharacterLanded);
        }
        void HandleClimb()
        {
            isClimbing = true;
            ResetJump();
            character.AnimationHandler.SetClimb();
            BonusDrag = this.climbDrag;
            wallVector = move;
            if (climbPS != null)
            {
                climbPS.Active(character.GetMidTransform().position + new Vector3(wallVector.x > 0 ? 0.5f : -0.5f, 0)).SetParent(character.GetTransform());
            }
        }

        public bool IsGrounded()
        {
            if (Physics2D.BoxCast(character.GetPosition(), groundBoxCheckSize, 0, -character.GetTransform().up, groundMaxDistanceCheck, groundMask))
            {
                return true;
            }
            return false;
        }
        Collider2D softPlatformCollider;
        public bool HasTouchSoftGround()
        {

            var hit = Physics2D.BoxCast(character.GetPosition(), groundBoxCheckSize, 0, -character.GetTransform().up, groundMaxDistanceCheck, softPlatformMask);
            if (hit.collider!=null)
            {
                softPlatformCollider=hit.collider;
                return true;
            }
            return false;
        }
        public bool IsWall()
        {
            if (Physics2D.BoxCast(character.GetMidTransform().position, wallBoxCheckSize, 0, character.GetTransform().right, wallMaxDistanceCheck, wallMask))
            {
                return true;
            }
            return false;
        }
    }
}

namespace Game.GameActor
{
}