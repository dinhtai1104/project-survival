using UnityEngine;

namespace Game.Bullet
{
    public class BounceHandler : MonoBehaviour
    {
        public BulletBase Base { get { if (bulletBase == null) bulletBase = GetComponent<BulletBase>(); return bulletBase; } }

        private BulletBase bulletBase;
        public LayerMask mask;
        private void OnEnable()
        {
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (Base.currentBounce < Base.MaxBounce)
            {
                if (collision.CompareTag("Ground"))
                {
                    Impact(collision);
                }
            }
        }
        void Impact(Collider2D collider)
        {
            Vector3 direction = Base.MoveHandler.GetDirection();
#if UNITY_EDITOR
            Debug.DrawLine(transform.position, transform.position + direction * 2, Color.red, 1);
#endif
            var hit = Physics2D.Raycast(transform.position, direction, 2, mask);
            if (hit.collider != null)
            {
                Base.MoveHandler.SetDirection(Vector3.Reflect(direction, hit.normal));
                
            }
        }
    }
    }