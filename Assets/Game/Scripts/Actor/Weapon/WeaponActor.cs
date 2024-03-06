using UnityEngine;
namespace Engine.Weapon
{
    public class WeaponActor : Actor
    {
        [SerializeField] private Transform m_TriggerPoint;
        private Actor m_Owner;
        private Transform m_PlaceHolderWeapon;
        public Transform PlaceHolderWeapon => m_PlaceHolderWeapon;
        public Actor Owner => m_Owner;
        public Transform TriggerPoint => m_TriggerPoint;

        public void InitOwner(Actor owner)
        {
            m_Owner = owner;
        }

        public void SetPlaceHolder(Transform placeHolder)
        {
            m_PlaceHolderWeapon = placeHolder;

            Trans.SetParent(placeHolder, false);
            Trans.localPosition = Vector3.zero;
        }
    }
}
