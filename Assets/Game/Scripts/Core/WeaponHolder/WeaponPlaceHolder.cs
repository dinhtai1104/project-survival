using Engine.Weapon;
using ExtensionKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Gameplay
{
    public class WeaponPlaceHolder : MonoBehaviour
    {
        [SerializeField] private List<Transform> m_WeaponHolders;
        private List<WeaponActor> m_WeaponActors;

        public void SetupWeapon(List<WeaponActor> weapons)
        {
            this.m_WeaponActors = weapons;
            for (int i = 0; i < weapons.Count; i++)
            {
                weapons[i].SetPlaceHolder(m_WeaponHolders[i]);
            }
        }

        public List<WeaponActor> UnpackWeapon()
        {
            if (m_WeaponActors.IsNotNull())
            {
                for (int i = 0; i < m_WeaponActors.Count; i++)
                {
                    m_WeaponActors[i].transform.SetParent(null, false);
                }
            }
            return m_WeaponActors;
        }
    }
}
