using Engine;
using Engine.Weapon;
using ExtensionKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Engine.State.Weapon
{
    public class BaseWeaponState : BaseState
    {
        public WeaponActor Weapon => Actor as WeaponActor;
        public Actor Owner => Weapon.Owner;
        public override void Enter()
        {
            base.Enter();
            Actor.IsActivated = true;
        }
    }
}

namespace Engine.State.Weapon
{
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
            
        }
    }
}
