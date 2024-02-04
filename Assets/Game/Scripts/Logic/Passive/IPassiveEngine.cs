using Game.GameActor;

public interface IPassiveEngine
{
    void Initialize(ActorBase actor);
    void Ticks();
    void ApplyPassive(IPassive passive);
    void RemovePassives();
} 