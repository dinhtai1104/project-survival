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
        private List<WeaponEntity> m_WeaponInQueue = new List<WeaponEntity>();
        private List<WeaponEntity> m_WeaponDeleteInQueue = new List<WeaponEntity>();

        public void Init(Actor actor)
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
            Architecture.Get<EventMgr>().Subscribe<WeaponBuyEventArgs>(BuyWeaponEventHandler);
        }
        private void OnDisable()
        {
            Architecture.Get<EventMgr>().Unsubscribe<WeaponBuyEventArgs>(BuyWeaponEventHandler);
        }

        private void BuyWeaponEventHandler(object sender, IEventArgs e)
        {
            var evt = e as WeaponBuyEventArgs;
            if (evt.m_Owner != this.m_Actor) return;

            // add weapon
            var weaponEntity = evt.m_WeaponEntity;
            m_WeaponInQueue.Add(weaponEntity);
        }
    }
}
