using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GameActor
{
    public interface IWeaponHandler
    {
        WeaponBase CurrentWeapon { get; }
        Transform DefaultAttackPoint { get; }
        List<WeaponBase> SupportWeapons { get; }

        void Destroy();
        Transform GetAttackPoint(WeaponBase weapon);
        Transform GetCurrentAttackPoint();
        void LoadWeapon(WeaponBase weapon, WeaponData weaponData);
        UniTask SetSupportWeapon(WeaponBase weapon);
        UniTask SetUp(ActorBase actor);
        void SetWeapon(int index);
        void Stop();
    }
}