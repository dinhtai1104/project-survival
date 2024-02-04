using Game.GameActor;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Bullet
{
    public class RicochetHandler : MonoBehaviour
    {
        public BulletBase Base { get { if (bulletBase == null) bulletBase = GetComponent<BulletBase>(); return bulletBase; } }

        private BulletBase bulletBase;


        private Sensor sensor;
        List<ITarget> lastTargets = new List<ITarget>();
        private void OnEnable()
        {
            sensor = GetComponent<Sensor>();
            lastTargets.Clear();
        }
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (Base.currentRicochet < Base.MaxRicochet)
            {
                Impact(collider.GetComponentInParent<ITarget>());
            }
        }
        void Impact(ITarget target)
        {
            if ((Object)target == Base.Caster) return;
            if (target == null)
            {
                Base.canRicochet = false;
                return;
            }
            Vector3 direction = Base.MoveHandler.GetDirection();
            lastTargets.Add(target);

            ITarget nextTarget = sensor.Search((Character)Base.Caster,transform.position,lastTargets);
            //Logger.Log("NEXT:" + (nextTarget == null ? "NULL" : nextTarget.GetTransform().gameObject.name));
            if (nextTarget == null)
            {
                Base.canRicochet = false;

            }
            else
            {
                Base.canRicochet = true;

#if UNITY_EDITOR
                Debug.DrawRay(transform.position, (nextTarget.GetMidTransform().position - transform.position).normalized * 3, Color.red, 1);
#endif
                Base.MoveHandler.SetDirection((nextTarget.GetMidTransform().position-transform.position).normalized);
              //  Base.currentRicochet++;
            }
           
        }
    }
}