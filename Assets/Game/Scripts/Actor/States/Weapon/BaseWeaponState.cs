using Engine;
using Engine.Weapon;
using ExtensionKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Actor.States.Weapon
{
    public class BaseWeaponState : BaseState
    {
        public WeaponActor Weapon => Actor as WeaponActor;
        public Engine.Actor Owner => Weapon.Owner;
    }
    
    public class BaseWeaponState<TNextState> : BaseWeaponState where TNextState : BaseState
    {
        private Vector3 left = new Vector3(-1, 1, 1);
        private Vector3 right = new Vector3(1, 1, 1);

        public void NextState()
        {
            Weapon.Fsm.ChangeState(typeof(TNextState));
        }
        public override void Execute()
        {
            base.Execute();
            var target = Weapon.TargetFinder.CurrentTarget;
            Vector2 dir = Vector2.zero;
            if (target != null)
            {
                dir = target.CenterPosition - Weapon.Trans.position;
                
            }
            else
            {
                // Follow Joystick
                dir = Owner.Movement.CurrentDirection;
            }

            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Weapon.Trans.eulerAngles = new Vector3(0, 0, angle);
            Weapon.Movement.SetDirection(dir);

            if (angle > 90 || angle < -90)
            {
                Weapon.Trans.LocalScaleY(-1);
            }
            else
            {
                Weapon.Trans.LocalScaleY(1);
            }
        }
    }
}
