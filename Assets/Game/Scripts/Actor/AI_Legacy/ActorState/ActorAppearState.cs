using Game.Fsm;
using Game.Handler;
using TypeReferences;
using UnityEngine;

public class ActorAppearState : BaseState
{
    [SerializeField] private string _spawnAnim;
    [SerializeField] private string _animation;
    [SerializeField, ClassImplements(typeof(IState))]
    private ClassTypeReference _nextState;
    private bool update = false;
    public override void Enter()
    {
        base.Enter();
        update = true;
        Actor.IsActived = false;
        Actor.AnimationHandler.SetAnimation(_spawnAnim, false);
        Actor.AnimationHandler.AddAnimation(0, _animation, false);
        Actor.AnimationHandler.onCompleteTracking += AnimationHandler_onCompleteTracking;
    }
    public override void Exit()
    {
        update = false;
        Actor.AnimationHandler.onCompleteTracking -= AnimationHandler_onCompleteTracking;
        base.Exit();
    }

    private void Update()
    {
        if (update)
        {
            Actor.Machine.Ticks();
        }
    }

    private void AnimationHandler_onCompleteTracking(Spine.TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == _spawnAnim)
        {
            Actor.Machine.ChangeState(_nextState);
            Actor.IsActived = true;
        }
    }
}