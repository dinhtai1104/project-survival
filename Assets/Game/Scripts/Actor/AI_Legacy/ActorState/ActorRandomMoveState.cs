using Game.Fsm;
using UnityEngine;

namespace Game.AI.State
{
    public class ActorRandomMoveState : BaseState
    {
        [SerializeField] private string _fallAnim;
        [SerializeField] private float _timeRandomMove = 2f;
        [SerializeField] private float _timeRestWhenReach = 0.3f;
        private float _timeCtrRandomMove = 0;
        private float _timeCtrRestWhenReach = 0;
        private Vector2 _targetRandomDirectionMove = Vector2.right;
        private bool _isFinishRandomMove = false;
        public bool IsFinishedRandomMove => _isFinishRandomMove;

        public override void Enter()
        {
            base.Enter();
            _timeCtrRestWhenReach = 0;
            _timeCtrRandomMove = 0;
            _isFinishRandomMove = true;

            _targetRandomDirectionMove = Random.value > 0.5f ? Vector2.left : Vector2.right;
        }

        public override void Execute()
        {
            base.Execute();

            if (Actor.MoveHandler.isGrounded == false)
            {
                Actor.MoveHandler.Stop();
                if (Actor.AnimationHandler.GetAnimator().AnimationName != _fallAnim)
                {
                    Actor.AnimationHandler.SetAnimation(_fallAnim, true);
                }
                _timeCtrRestWhenReach = 0;
                _isFinishRandomMove = false;
                _targetRandomDirectionMove = Random.value > 0.5f ? Vector2.left : Vector2.right;
                return;
            }
            if (_isFinishRandomMove)
            {
                Actor.MoveHandler.Stop();
                _timeCtrRestWhenReach += Time.deltaTime;
                if (_timeCtrRestWhenReach >= _timeRestWhenReach)
                {
                    _timeCtrRestWhenReach = 0;
                    _isFinishRandomMove = false;
                    _targetRandomDirectionMove = Random.value > 0.5f ? Vector2.left : Vector2.right;
                }
            }
            else
            {
                _timeCtrRandomMove += Time.deltaTime;
                if (_timeCtrRandomMove >= _timeRandomMove)
                {
                    _timeCtrRandomMove = 0;
                    _isFinishRandomMove = true;
                }
                Actor.MoveHandler.Move(_targetRandomDirectionMove, 1);
            }
        }
    }
}