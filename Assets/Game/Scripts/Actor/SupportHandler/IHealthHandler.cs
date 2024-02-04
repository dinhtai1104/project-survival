using CodeStage.AntiCheat.ObscuredTypes;

namespace Game.GameActor
{
    public interface IHealthHandler
    {
        ActorBase Actor { get; set; }

        void AddArmor(int point);
        void AddHealth(float point);
        void ArmorDepleted();
        ObscuredFloat GetArmor();
        ObscuredFloat GetHealth();
        ObscuredFloat GetMaxHP();
        ObscuredFloat GetPercentHealth();
        void HealthDepleted();
        void Reset(float health, float armor);
        void ResetArmor();
        void RestoreHealth();
        void SetActor(ActorBase actor);
        void SetArmor(int armor);
        void SetHealth(float health);
        void SetMaxHealth(float newValue);
    }
}