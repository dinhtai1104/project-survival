using System;
using System.Collections.Generic;
using System.Linq;
using Engine;
using UnityEngine;

namespace Engine.State.Common
{
    public class ActorKeepDistanceState : BaseActorState
    {
        [SerializeField] private string _runAnimation = "move";
        [SerializeField] private string _idleAnimation = "idle";

        [SerializeField, Range(0f, 10f)] private float _distance = 3f;

        [SerializeField, Range(0f, 10f)] private float _cooldownDuration = 3f;

        [SerializeField, Range(0f, 10f)] private float _minDuration = 2f;

        [SerializeField, Range(0f, 10f)] private float _changeDirCooldown = 2f;

        private float _cooldownTimer;
        private float _durationTimer;
        private float _changeDirTimer;
        private Actor _target;
        private Vector3 _currentDir;

        public float Distance
        {
            get { return _distance; }
        }

        public bool IsCooldowning { set; get; }

        public float MinDuration
        {
            get { return _minDuration; }
        }

        public Actor CurrentThreat
        {
            set { _target = value; }
            get { return _target; }
        }

        public override void Enter()
        {
            base.Enter();
            _distance = Actor.Stats.GetValue(StatKey.AttackRange, 3f);
            _durationTimer = 0f;
            _cooldownTimer = 0f;
            _changeDirTimer = 0f;
            Actor.SkillCaster.InterruptCurrentSkill();
            UpdateDirection();
        }

        public override void Execute()
        {
            base.Execute();
            _durationTimer += Time.deltaTime;

            if (_durationTimer >= _minDuration)
            {
                Actor.Fsm.BackToDefaultState();
                return;
            }

            if (_target != null)
            {
                Actor.Animation.EnsurePlay(0, _runAnimation, true);

                _changeDirTimer += Time.deltaTime;

                if (_changeDirTimer >= _changeDirCooldown)
                {
                    _changeDirTimer = 0f;
                    UpdateDirection();
                }

                Actor.Movement.MoveDirection(_currentDir);
            }
        }

        public override void Exit()
        {
            base.Exit();
            Actor.Animation.EnsurePlay(0, _idleAnimation, true);
            Actor.Movement.IsMoving = false;
            IsCooldowning = true;
        }

        private void Update()
        {
            if (IsCooldowning)
            {
                _cooldownTimer += Time.deltaTime;

                if (_cooldownTimer >= _cooldownDuration)
                {
                    _cooldownTimer = 0f;
                    IsCooldowning = false;
                }
            }
        }

        private void UpdateDirection()
        {
            Vector3 casterPos = Actor.BotPosition;
            Vector3 dir = Vector3.Normalize(casterPos - _target.BotPosition);
            dir.z = 0f;
            Vector3 predictDest = casterPos + dir;

            Bound2D bounds = Actor.Movement.MovementBound;

            if (!bounds.Contains(predictDest))
            {
                //dir = -dir;

                predictDest.z = 0f;

                if (predictDest.y >= bounds.YHigh)
                {
                    dir = Vector3.Reflect(dir, Vector3.down);
                }
                else if (predictDest.y <= bounds.YLow)
                {
                    dir = Vector3.Reflect(dir, Vector3.up);
                }

                predictDest = Actor.BotPosition + dir;

                if (predictDest.x <= bounds.XLow)
                {
                    dir = Vector3.Reflect(dir, Vector3.right);
                }
                else if (predictDest.x >= bounds.XHigh)
                {
                    dir = Vector3.Reflect(dir, Vector3.left);
                }
            }

            dir.z = 0f;
            _currentDir = dir;
        }

        public Actor FindPotentialThreat()
        {
            IList<Actor> enemies = Actor.TargetFinder.Enemies;

            if (enemies.Count == 0)
                return null;

            float minSqrDist = float.MaxValue;
            Actor target = null;

            for (int i = 0; i < enemies.Count; ++i)
            {
                Actor enemyActor = enemies[i];

                if (enemyActor != null && enemyActor != Actor && enemyActor.gameObject.activeInHierarchy)
                {
                    float sqrDist = Vector3.SqrMagnitude(enemyActor.BotPosition - Actor.BotPosition);

                    if (!enemyActor.Status.HasStatusWithTag(Tags.Status_CrowdControl))
                    {
                        if (sqrDist < minSqrDist)
                        {
                            minSqrDist = sqrDist;
                            target = enemyActor;
                        }
                    }
                    else
                    {
                        if (sqrDist < minSqrDist)
                        {
                            minSqrDist = sqrDist;
                            target = enemyActor;
                        }
                    }
                }
            }


            if (target == null || !target.gameObject.activeInHierarchy)
                return null;

            return target;
        }
    }
}
