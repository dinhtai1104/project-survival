using Engine;
using Engine.Weapon;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class WeaponHolderController : MonoBehaviour
    {
        [SerializeField] private List<WeaponPlaceHolder> m_PlaceHolders;
        private Actor m_Actor;

        public void Init(Actor actor)
        {
            this.m_Actor = actor;
        }

        public void SetupWeapon(List<WeaponActor> weapons)
        {
            foreach (var holder in m_PlaceHolders)
            {
                holder.UnpackWeapon();
            }

            m_PlaceHolders[weapons.Count - 1].SetupWeapon(weapons);
        }
    }
}
