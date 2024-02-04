using System.Collections.Generic;

namespace Game.GameActor
{
    [System.Serializable]
    public class StatBonusHandler
    {
        public int bonusAttack = 0;
        public int bonusHealth = 0;
        public float bonusFireRate = 0;
        public float bonusBulletVelocity = 0;
        public float bonusSpeed = 0;
        public float bonusReloadSpeed = 0;
        public float damageMultiply = 1;


        public StatBonusHandler()
        {

        }
        public void Clear()
        {
            bonusReloadSpeed = 0;
            bonusBulletVelocity = 0;
            bonusSpeed = 0;
            bonusFireRate = 0;
            bonusAttack = 0;
            bonusHealth = 0;
            damageMultiply = 1;
        }

    }
}