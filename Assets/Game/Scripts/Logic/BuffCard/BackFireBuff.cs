using Game.GameActor.Buff;

namespace Game.BuffCard
{
    public class BackFireBuff : AbstractBuff
    {
        public ShootPatternBase shootPattern;
        private void OnEnable()
        {

        }
        private void OnDisable()
        {
        }


        public override void Play()
        {
            ((GunBase)Caster.WeaponHandler.CurrentWeapon).AddShootPattern(shootPattern);
        }
    }
}