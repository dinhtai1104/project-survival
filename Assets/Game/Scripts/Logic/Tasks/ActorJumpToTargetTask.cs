using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.GameActor;
using System;
using UnityEngine;

public class ActorJumpToTargetTask : SkillTask
{
    private float _duration => durationJump.FloatValue;

    private float _height => heightJump.FloatValue;

    public ValueConfigSearch durationJump;
    public ValueConfigSearch heightJump;

    [SerializeField] private string _animationInHighest;
    [SerializeField] private string _animationFalling;
    [SerializeField] private AnimationCurve _jumpCurve;

    public LayerMask layerDetectGround;
    public bool isControlByOther = false;
    private Vector3 dest;

    public async override UniTask Begin()
    {
        await base.Begin();
        if (isControlByOther)
        {
            var player = GameController.Instance.GetMainActor();
            Vector3 dest = player.GetPosition();
            Caster.SetFacing(player);
        }
        Vector3 dir = Vector3.Normalize(dest - Caster.GetPosition());
        ((Character)Caster).SetFacing(dir.x);

        var pos = Physics2D.Raycast(dest, Vector3.down, Mathf.Infinity, layerDetectGround);

        dest = pos.point;
        Caster.AnimationHandler.SetAnimation(_animationInHighest, true);
        float lastPosY = Caster.GetPosition().y;

        Caster.MoveHandler.Jump(Vector3.zero, 0);
        Caster.transform.DOJump(dest, Caster.GetMidTransform().position.y + 7f, 1, _duration).OnComplete(() =>
        {
            IsCompleted = true;
        }).OnUpdate(() =>
        {
            float currentPosY = Caster.GetPosition().y;
            if (currentPosY < lastPosY)
            {
                Caster.AnimationHandler.SetAnimation(_animationFalling, true);
            }
            lastPosY = currentPosY;
        }).SetEase(_jumpCurve);
    }

    public Vector2 SetVelocityToJump(Vector2 goToJumpTo, float timeToJump)
    {
        var toTarget = goToJumpTo - (Vector2)this.transform.position;
        var _velocity = (toTarget - (Mathf.Pow(timeToJump, 2) * 0.5f * Physics2D.gravity * Caster.GetRigidbody().gravityScale)) / timeToJump;

        return _velocity;
    }

    public void SetPos(Vector3 pos)
    {
        dest = pos;
    }
}