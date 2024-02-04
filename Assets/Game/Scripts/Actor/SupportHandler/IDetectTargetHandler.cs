using Game.GameActor;

public interface IDetectTargetHandler
{
    ITarget CurrentTarget { get; }

    void BatchFixedUpdate();
    void BatchUpdate();
    ITarget GetTarget(int index);
    void Search();
}