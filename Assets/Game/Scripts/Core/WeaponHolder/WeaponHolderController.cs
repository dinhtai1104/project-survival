using Assets.Game.Scripts.Core.Data.Database.Equipment.Weapon;
using Assets.Game.Scripts.Events;
using Core;
using Engine;
using Engine.Weapon;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    public class WeaponHolderController : MonoBehaviour
    {
        [SerializeField] private List<WeaponPlaceHolder> m_PlaceHolders;
        private PlayerActor m_Actor;
        private List<WeaponActor> m_ListWeaponCurrent = new List<WeaponActor>();

        public List<WeaponActor> ListWeaponCurrent { get => m_ListWeaponCurrent; set => m_ListWeaponCurrent = value; }

        public void Init(ActorBase actor)
        {
            this.m_Actor = actor as PlayerActor;
        }

        public void SetupWeapon(List<WeaponActor> weapons)
        {
            this.m_ListWeaponCurrent = weapons;
            foreach (var holder in m_PlaceHolders)
            {
                holder.UnpackWeapon();
            }

            m_PlaceHolders[weapons.Count - 1].SetupWeapon(weapons);
        }

        //Listen WeaponChange
        private void OnEnable()
        {
            GameArchitecture.GetService<IEventMgrService>().Subscribe<WeaponBuyEventArgs>(BuyWeaponEventHandler);
        }
        private void OnDisable()
        {
            GameArchitecture.GetService<IEventMgrService>().Unsubscribe<WeaponBuyEventArgs>(BuyWeaponEventHandler);
        }

        private void BuyWeaponEventHandler(object sender, IEventArgs e)
        {
            var evt = e as WeaponBuyEventArgs;
            if (evt.m_Owner != this.m_Actor) return;

            // add weapon
            var weaponEntity = evt.m_WeaponEntity;
        }

        public void AddWeapon(WeaponActor weapon)
        {
            m_ListWeaponCurrent.Add(weapon);
            SetupWeapon(m_ListWeaponCurrent);
        }

        public bool IsMax()
        {
            return m_ListWeaponCurrent.Count >= ConstantValue.Weapon_MaxSlot;
        }
    }
}
