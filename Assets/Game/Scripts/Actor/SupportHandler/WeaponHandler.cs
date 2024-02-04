using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GameActor
{
    public class WeaponHandler : MonoBehaviour
    {
        public delegate void OnWeaponChanged(WeaponHandler weaponHandler,WeaponBase weapon);
        public OnWeaponChanged onWeaponChanged;
        ActorBase Actor;


        [SerializeField]
        //default weapons
        private List<WeaponBase> weapons = new List<WeaponBase>();
        private List<WeaponData> weaponDatas = new List<WeaponData>();
        [SerializeField]
        private Transform [] defaultAttackPoint;

        public Transform DefaultAttackPoint => defaultAttackPoint[currentWeaponIndex];

        private Dictionary<WeaponBase, Transform> shootPointDict=new Dictionary<WeaponBase, Transform>();
        // instance of selected weapon
        private List<WeaponBase> weaponInstance=new List<WeaponBase>();
        // instance of selected weapon
        private List<WeaponBase> supportWeaponInstance = new List<WeaponBase>();

        int currentWeaponIndex;
        public List<WeaponBase> SupportWeapons { get => supportWeaponInstance; }

        public WeaponBase CurrentWeapon { get => weaponInstance.Count > 0 ? weaponInstance[currentWeaponIndex] : null; }

        [SerializeField]
        private float supportWeaponOffset=1.5f;
        public async UniTask SetUp(ActorBase actor)
        {
            this.Actor = actor;
            shootPointDict.Clear();
            for (int i = 0; i < weapons.Count; i++)
            {
                WeaponBase weapon = await weapons[i].SetUp((Character)Actor);
                weaponInstance.Add(weapon);

                if ((weapon.GetType()==typeof(GunBase) || weapon.GetType().BaseType == typeof(GunBase)) && ((GunBase)weapon).weaponObj != null)
                {
                    ((GunBase)weapon).weaponObj.Flip(Actor.GetFacingDirection());
                }
               

                shootPointDict.Add(weapon, weapon.GetShootPoint() ?? defaultAttackPoint[i]);
                if (i < weaponDatas.Count)
                {
                    await weapon.SetItemEquipment(weaponDatas[i].Item);
                }
            }
            SetWeapon(0);
        }
        public void ClearPool()
        {
            weapons.Clear();
            weaponInstance.Clear();
            shootPointDict.Clear();
        }
        public void LoadWeapon(WeaponBase weapon, WeaponData weaponData)
        {
            this.weapons.Add(weapon);
            this.weaponDatas.Add(weaponData);
        }
        public void SetWeapon(int index)
        {
            currentWeaponIndex = index;

            onWeaponChanged?.Invoke(this,CurrentWeapon);
        }

        public async UniTask SetSupportWeapon(WeaponBase weapon)
        {

            var instance=await weapon.SetUp((Character)Actor);

            supportWeaponInstance.Add(instance);
            shootPointDict.Add(instance, instance.GetShootPoint() ?? defaultAttackPoint[0]);
            
            for(int i = 0; i < supportWeaponInstance.Count; i++)
            {
                GunBase supportWeapon = supportWeaponInstance[i] as GunBase;
                supportWeapon.weaponObj.SetActive(gameObject.activeInHierarchy);
                supportWeapon.weaponObj.DefaultPosition= GameUtility.GameUtility.RotationToVector((i-supportWeaponInstance.Count/2)*30+90)*supportWeaponOffset;
                //Logger.Log(i + " + " + supportWeapon.weaponObj.DefaultPosition);
            }


        }

        //get shoot point of a weapon
        public Transform GetAttackPoint(WeaponBase weapon)
        {

            if (weapon==null ||!shootPointDict.ContainsKey(weapon)) return DefaultAttackPoint;
            return shootPointDict[weapon];
        }

        public Transform GetCurrentAttackPoint()
        {
            if (currentWeaponIndex >= weaponInstance.Count && defaultAttackPoint != null && defaultAttackPoint.Length > 0) return defaultAttackPoint[0];
            else if(weaponInstance.Count==0 && (defaultAttackPoint==null || (defaultAttackPoint != null && defaultAttackPoint.Length == 0))) return transform;
            return GetAttackPoint(weaponInstance[currentWeaponIndex]);
        }


        //destroy all weapon instance
        public void Destroy()
        {
            foreach(WeaponBase weapon in weaponInstance)
            {
                weapon.Destroy();
            }
            //weapons.Clear();
            weaponInstance.Clear();
            shootPointDict.Clear();
        }

        public void Stop()
        {
            foreach (WeaponBase weapon in weaponInstance)
            {
                weapon.Release();
            }
        }
        private void OnDisable()
        {
            foreach(var weapon in supportWeaponInstance)
            {
                if ((weapon.GetType() != typeof(GunBase) && weapon.GetType().BaseType != typeof(GunBase)) || ((GunBase)weapon).weaponObj==null) break;
                ((GunBase)weapon).weaponObj.SetActive(false);
            }
            foreach (var weapon in weaponInstance)
            {
                if ((weapon.GetType() != typeof(GunBase) && weapon.GetType().BaseType != typeof(GunBase)) || ((GunBase)weapon).weaponObj == null) break;
                ((GunBase)weapon).weaponObj.SetActive(false);
            }
        }
        private void OnEnable()
        {
            foreach (var weapon in supportWeaponInstance)
            {
                if ((weapon.GetType() != typeof(GunBase) && weapon.GetType().BaseType != typeof(GunBase)) || ((GunBase)weapon).weaponObj == null) break;
                ((GunBase)weapon).weaponObj.SetActive(true);
            }
            foreach (var weapon in weaponInstance)
            {
                if ((weapon.GetType() != typeof(GunBase) && weapon.GetType().BaseType != typeof(GunBase)) || ((GunBase)weapon).weaponObj == null) break;
                ((GunBase)weapon).weaponObj.SetActive(true);
            }
        }
    }
}
[System.Serializable]
public class WeaponData
{
    public WeaponBase Weapon;
    public EquipableItem Item;
}