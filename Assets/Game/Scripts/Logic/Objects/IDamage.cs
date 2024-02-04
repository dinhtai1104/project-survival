using Game.GameActor;

public interface IDamage
{
    ActorBase Caster { get; set; }
    Stat DmgStat { get; set; }
}