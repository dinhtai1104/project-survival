using Cysharp.Threading.Tasks;
using Game.GameActor;
using System;

public class ActorWaitFallGround : SkillTask
{
    public override async UniTask Begin()
    {
        await base.Begin();
        Caster.MoveHandler.onCharacterLanded += OnLand;
    }
    public override async UniTask End()
    {
        await base.End();
        Caster.MoveHandler.onCharacterLanded -= OnLand;
    }

    private void OnLand(MoveHandler moveHandler)
    {
        IsCompleted = true;
    }
}