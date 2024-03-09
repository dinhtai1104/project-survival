using Assets.Game.Scripts.Core.Data.Database.Equipment.Weapon;
using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Events
{
    public class WeaponBuyEventArgs : BaseEventArgs<WeaponBuyEventArgs>
    {
        public WeaponEntity m_WeaponEntity;
        public Engine.Actor m_Owner;

        public WeaponBuyEventArgs(WeaponEntity weaponEntity, Engine.Actor owner)
        {
            m_WeaponEntity = weaponEntity;
            m_Owner = owner;
        }
    }
}
