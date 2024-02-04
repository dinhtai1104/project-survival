using Cysharp.Threading.Tasks;
using Effect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Game.GameActor
{
    public abstract class MoveHandler : MonoBehaviour, IMoveHandler
    {
        /// <summary>
        /// Locked Movement For Update
        /// </summary>
        /// 
        public bool Locked { set; get; } = false;
        public delegate void OnJumpState();
        public OnJumpState onStartJump;
        public OnJumpState onLoopJump;
        public OnJumpState onStartFallJump;
        public OnJumpState onFallLoopJump;
        public OnJumpState onFallGroundJump;

        public delegate void OnChanged(MoveHandler moveHandler);

        public OnChanged onCharacterMoved;
        public OnChanged onCharacterStopped;
        public OnChanged onCharacterLanded;
        public OnChanged onCharacterJump;
        public OnChanged onCharacterJumpReleased;
        public OnChanged onCharacterDescend;


        protected Character character;
        public bool isMoving = false, isGrounded = false, isClimbing, unscaleTime = false;
        public bool isFallingDown = false, isFallingFromLastJump = false, isHoldingJump;
        public int jumpIndex = 0;
        public int maxJump;
        public float lastJump = 0;
        public Vector2 move = Vector2.zero;
        public Vector2 lastMove = Vector2.zero;
        private float moveSpeed;
        protected float speedMultiply = 1;
        protected float drag=1;
        protected float bonusDrag=0;
        public float BonusDrag
        {
            get => bonusDrag;
            set
            {
                bonusDrag = value;
                Drag =drag+bonusDrag;
            }
        }
        public float Drag
        {
            get => drag;
            set
            {
                character.GetRigidbody().drag = value;
            }
        }
        public Character Character { get => character; }
        public float MoveSpeed { get => character.Stats.GetValue(StatKey.SpeedMove); }
        public float JumpForce { get => character.Stats.GetValue(StatKey.JumpForce); }
        public float SpeedMultiply { get => speedMultiply; set => speedMultiply = value; }

        public List<MoveAddOn> addOns = new List<MoveAddOn>();
        public List<MoveAddOn> addOnInstances = new List<MoveAddOn>();

        CancellationTokenSource canncellation;
        public virtual void SetUp(Character character)
        {
            this.character = character;
            addOnInstances.Clear();
            lastMove = move = Vector2.zero;
            foreach (var addOn in addOns)
            {
                AddAddOn(addOn);
            }
        }
        public void AddAddOn(MoveAddOn addOn)
        {
            addOnInstances.Add(addOn.SetUp(this));
        }
        public virtual void MoveToPosition(Vector2 position, float followSpeed)
        {
        }
        public abstract void Move(Vector2 moveDirection, float ratio);

        public abstract void Stop();

        public virtual void AddForce(Vector2 force) { }
        public abstract void Jump();

        public abstract void Jump(Vector2 direction, float power);

        public virtual void ResetJump() { }
        public abstract void ReleaseJump();
        public abstract void HoldJump();
        public virtual void ClearBoostVelocity()
        {
        }
        public abstract void Descend();
        public virtual void AddJetpack(float fuel) { }

        protected abstract void FixedUpdate();



        //

        private void OnEnable()
        {
            canncellation = new CancellationTokenSource();
        }

        protected virtual void OnDisable()
        {
            if (canncellation != null)
            {
                canncellation.Cancel();
                //character.Stats.RemoveListener(StatKey.SpeedMove, OnUpdateSpeedMove);
            }

        }

        public virtual void Stop(bool isUseAnimIdle)
        {
            if (isUseAnimIdle)
            {
                Stop();
            }
        }

        private void OnDestroy()
        {
            if (canncellation != null)
            {
                canncellation.Cancel();
                canncellation.Dispose();
                //character.Stats.RemoveListener(StatKey.SpeedMove, OnUpdateSpeedMove);
            }
        }
    }
}