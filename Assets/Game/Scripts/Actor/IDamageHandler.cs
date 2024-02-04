namespace Game.GameActor
{
    public interface IDamageHandler
    {
        bool GetHit(IDamageDealer damageDealer, DamageSource damageSource);
    }
}