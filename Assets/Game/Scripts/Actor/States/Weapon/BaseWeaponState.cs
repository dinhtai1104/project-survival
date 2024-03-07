using Engine;
using Engine.Weapon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Actor.States.Weapon
{
    public class BaseWeaponState : BaseState
    {
        public WeaponActor Weapon => Actor as WeaponActor;
        public Engine.Actor Owner => Weapon.Owner;
    }
    
    public class BaseWeaponState<TNextState> : BaseWeaponState where TNextState : BaseState
    {

        public void NextState()
        {
            Weapon.Fsm.ChangeState(typeof(TNextState));
        }
        public override void Execute()
        {
            base.Execute();
            var target = Weapon.TargetFinder.CurrentTarget;
            if (target != null)
            {
                var dir = target.CenterPosition - Weapon.CenterPosition;
                Weapon.transform.LookAt2D(dir);
                Weapon.Movement.SetDirection(dir);
            }
            else
            {
                // Follow Joystick
                var directionMove = Owner.Movement.CurrentDirection;
                Weapon.transform.LookAt2D(directionMove);
                Weapon.Movement.SetDirection(directionMove);
            }
        }
    }
}
