using CodeStage.AntiCheat.ObscuredTypes;

namespace Game.GameActor
{
    [System.Serializable]
    public class CharacterHealthHandler : HealthHandler
    {
        public CharacterHealthHandler(int health, int armor) : base(health, armor)
        {
        }
    }
}