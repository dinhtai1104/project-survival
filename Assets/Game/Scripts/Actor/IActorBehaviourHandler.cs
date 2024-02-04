namespace Game.GameActor
{
    public interface IActorBehaviourHandler
    {
        void Destroy();
        void SetUp(ActorBase actor);
        void StartBehaviours();
        void StopBehaviours();
    }
}