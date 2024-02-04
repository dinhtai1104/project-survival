using Game.GameActor;
using UnityEngine;

public interface IImpact
{
    BulletBase Base { get; set; }
    void SetUp(BulletBase bulletBase);
    void Impact(ITarget target);
    void ForceImpact();
    void OnTriggerEnter2D(Collider2D collider);
}
