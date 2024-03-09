using Core;
using Engine;
namespace Assets.Game.Scripts.Events
{
    public class AttackEventArgs : BaseEventArgs<AttackEventArgs>
    {
        public Engine.Actor attack;
        public Engine.Actor defender;
        public HitResult hitResult;

        public AttackEventArgs(Engine.Actor attack, Engine.Actor defense, HitResult hitResult)
        {
            this.attack = attack;
            this.defender = defense;
            this.hitResult = hitResult;
        }
    }
}
