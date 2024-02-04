using Game.GameActor.Buff;

namespace Game.GameActor.Passive
{
    public class RocketCatPassive : AbstractBuff
    {
        public MoveAddOn jetpack;

        public override void Play()
        {
            Caster.MoveHandler.AddAddOn(jetpack);
        }
    }
}