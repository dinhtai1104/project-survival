using Cysharp.Threading.Tasks;
using Game.GameActor;
using MoreMountains.Feedbacks;
using Spine;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Tasks
{
    public class JumpSkillTask : SkillTask
    {
        public ValueConfigSearch JumpCooldown;
        public float jumpForce;
        private float time = 0;
        private bool IsJump = false;

        [SerializeField] private MMFeedback feedback;

        [Header("Animation")]
        [SerializeField] private string startJump;
        [SerializeField] private string loopJumping;
        [SerializeField] private string highestJumping;
        [SerializeField] private string Falling;
        [SerializeField] private string fallGrounded;

        public override async UniTask Begin()
        {
            IsJump = false;
            time = Time.time;
            await base.Begin();

            Caster.AnimationHandler.SetJump(0);
            Caster.AnimationHandler.onEventTracking += OnTriggerEvent;
            Caster.MoveHandler.onLoopJump += OnJumpLoop;
            Caster.MoveHandler.onStartFallJump += OnStartFall;
            Caster.MoveHandler.onCharacterLanded += OnFallEnd;
        }

        private void OnFallEnd(MoveHandler moveHandler)
        {
            IsJump = false;
            Caster.AnimationHandler.SetIdle();
            IsCompleted = true;
            time = Time.time;
            feedback.Play(transform.position);
        }

        private void OnStartFall()
        {
            Caster.AnimationHandler.SetJump(3);
        }

        private void OnJumpLoop()
        {
            Caster.AnimationHandler.SetJump(1);
        }

        private void OnTriggerEvent(TrackEntry trackEntry, Spine.Event e)
        {
            var animation = trackEntry.Animation.Name;
            var eventKey = e.Data.Name;
            if (animation == startJump)
            {
                if (eventKey == "jump_tracking")
                {
                    Jump();
                }
            }
        }
        public override async UniTask End()
        {
            Caster.AnimationHandler.onEventTracking -= OnTriggerEvent;
            Caster.MoveHandler.onLoopJump -= OnJumpLoop;
            Caster.MoveHandler.onStartFallJump -= OnStartFall;
            Caster.MoveHandler.onCharacterLanded -= OnFallEnd; 
            IsJump = false;
            await base.End();
        }

        private void Jump()
        {
            time = Time.time;
            Caster.MoveHandler.Jump(Vector3.up, jumpForce);
        }
    }
}