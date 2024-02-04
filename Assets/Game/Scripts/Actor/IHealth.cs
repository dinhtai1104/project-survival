using CodeStage.AntiCheat.ObscuredTypes;

namespace Game.GameActor
{
    public interface IHealth
    {
        void SetHealth(int health);
        void SetArmor(int armor);
        ObscuredInt GetHealth();
        ObscuredInt GetArmor();
        ObscuredInt GetMaxHP();

        void HealthDepleted();
        void ArmorDepleted();

    }
}