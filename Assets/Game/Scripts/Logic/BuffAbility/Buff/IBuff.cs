using Game.GameActor;

public interface IBuff
{
    EBuff BuffKey { get; }
    BuffAtrData BuffData { get; }
    ActorBase Caster { get; }
    bool IsPause { set; get; }
    void Initialize(ActorBase Caster, BuffEntity entity, int stageStart);
    void BeforePlay();
    void Play();
    void OnUpdate(float dt);
    void Exit();
}