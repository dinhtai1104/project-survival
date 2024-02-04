using Game.GameActor;

public interface IStateEnterCallback
{
    void SetActor(ActorBase actor);
    public void Action();
}