using Game.Fsm;
using System.Collections.Generic;
using UnityEngine;

public class ActorRandomFlyState : BaseState
{
    protected Vector2 _targetPos;
    private bool _isReachedPos;
    public Vector3 legitZoneSize = new Vector2(30, 20);
    protected Collider2D[] colliders = new Collider2D[1];

    public bool IsReachedPos => _isReachedPos;
    [SerializeField] protected LayerMask _layerMaskObstacles;
    [SerializeField] private float _delayWhenReached = 1f;
    private float _timeDelayCtr = 0;
    private Vector2 direction;

    public override void Enter()
    {
        base.Enter();
        _isReachedPos = false;
        _timeDelayCtr = 0;
        ScanNewPosition();
    }
    public override void Execute()
    {
        if (Actor.IsDead()) return;
        base.Execute();
        if (!_isReachedPos)
        {
            direction = (Vector3)_targetPos - Actor.GetMidTransform().position;
            if (direction.sqrMagnitude <= 0.3 * 0.3f)
            {
                _isReachedPos = true;
                _timeDelayCtr = Time.time;
                //Actor.MoveHandler.Stop();
                return;
            }
            //Actor.SetFacing(direction.x);
            Actor.MoveHandler.Move(direction.normalized, 1f);
        }
        else
        {
            if (Time.time - _timeDelayCtr >= _delayWhenReached)
            {
                _isReachedPos = false;
                ScanNewPosition();
            }
            else
            {
                Actor.MoveHandler.Move(direction.normalized, 0.3f);
            }
        }
    }
    public override void Exit()
    {
        base.Exit();
        Actor.MoveHandler.Stop();
    }

    protected virtual void ScanNewPosition()
    {
        var index = 0;
        do
        {
            index++;
            _targetPos.Set(Random.Range(-legitZoneSize.x / 2f, legitZoneSize.x / 2), Random.Range(-legitZoneSize.y / 2f, legitZoneSize.y / 2));
        } while ((_targetPos.y > 14 || Physics2D.OverlapCircleNonAlloc(_targetPos, 2f, colliders, _layerMaskObstacles) > 0) && index < 100);
    }
}