using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicObject : MonoBehaviour
{
    Transform _transform;
    Rigidbody2D rb;
    public UnityEngine.Events.UnityEvent onImpact;
    private void OnEnable()
    {
        _transform = transform;
        rb = GetComponent<Rigidbody2D>();
    }
    Vector3 scale = Vector3.one;
    private void FixedUpdate()
    {
        Vector3 velocity = Vector3.ClampMagnitude(rb.velocity/(-Physics2D.gravity.y*rb.gravityScale),5);

        scale.y = 1+Mathf.Abs(velocity.y)/3f;
        scale.x = 1- Mathf.Abs(velocity.y)/3f;

        _transform.localScale = Vector3.Lerp(_transform.localScale,scale,0.4f);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude > 1)
        {
            onImpact?.Invoke();
            Game.Pool.GameObjectSpawner.Instance.Get("VFX_ImpactGround", res =>
            {
                res.GetComponent<Game.Effect.EffectAbstract>().Active(transform.position);
            });
        }
    }
}
