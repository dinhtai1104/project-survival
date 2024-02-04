using UnityEngine;

namespace Game.GameActor
{
    public interface IAttackHandler
    {
        void Active();
        bool IsValid(ECharacterType type);
        void OnAttackEnded();
        void SetUp(Character character);
        void Stop();
        void Trigger();
        void Trigger(Vector2 direction, ITarget trackingTarget);
        void TriggerSupportWeapons();
    }
}