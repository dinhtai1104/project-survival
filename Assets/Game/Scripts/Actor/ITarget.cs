using UnityEngine;

namespace Game.GameActor
{
    public interface ITarget
    {
        Transform GetTransform();
        Transform GetMidTransform();
        Vector3 GetMidPos();
        Vector3 GetPosition();
        bool GetHit(DamageSource damageSource, IDamageDealer dealer);
        float GetHealthPoint();
        bool IsDead();
        bool IsThreat();
        bool CanFocusOn();
        void HighLight(bool active);
        ECharacterType GetCharacterType();

    }
}