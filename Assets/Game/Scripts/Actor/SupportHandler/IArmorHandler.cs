using Game.GameActor;

public interface IArmorHandler
{
    void Break();
    void GetHit();
    void SetUp(Character character);
}