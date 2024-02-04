using Game.GameActor;

public interface IPassive
{
    void Initialize(ActorBase actor);
    void OnUpdate();
    void Play();
    void Remove();
}