using UnityEngine;

namespace Game.GameActor
{
    public interface IMoveHandler
    {
        Character Character { get; }
        float JumpForce { get; }
        bool Locked { get; set; }
        float MoveSpeed { get; }
        float SpeedMultiply { get; set; }

        void AddAddOn(MoveAddOn addOn);
        void AddForce(Vector2 force);
        void AddJetpack(float fuel);
        void ClearBoostVelocity();
        void Descend();
        void HoldJump();
        void Jump();
        void Jump(Vector2 direction, float power);
        void Move(Vector2 moveDirection, float ratio);
        void MoveToPosition(Vector2 position, float followSpeed);
        void ReleaseJump();
        void SetUp(Character character);
        void Stop();
    }
}