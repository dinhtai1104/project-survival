using com.mec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Logic.Objects
{
    public class AutoMoveToLocalPosition : MonoBehaviour, IMove
    {
        public Stat Speed { set; get; }
        public bool IsActive { set; get; }

        public Vector3 LocalPositionTarget { set; get; }

        public Vector3 GetDirection()
        {
            return transform.right;
        }

        public virtual void SetMove(Vector2 move)
        {

        }
        public void Move()
        {
            Timing.KillCoroutines(gameObject);
            Timing.RunCoroutine(_Moving(), gameObject);
        }

        private IEnumerator<float> _Moving()
        {
            IsActive = true;
            var dir = LocalPositionTarget - transform.localPosition;
            while (true)
            {
                if (GameUtility.GameUtility.GetRange(transform.localPosition, LocalPositionTarget) < 0.2f)
                {
                    transform.localPosition = LocalPositionTarget;
                    IsActive = false;
                    break;
                }
                transform.localPosition += Speed.Value * Time.deltaTime * dir.normalized;
                yield return Time.deltaTime;
            }
            IsActive = false;
            Timing.KillCoroutines(gameObject);
        }

        public void Move(Stat speed, Vector2 direction)
        {
            Timing.KillCoroutines(gameObject);
        }

        public void OnUpdate()
        {
        }

        public void SetDirection(Vector3 direction)
        {
        }

        public void SetPosition(Vector3 position)
        {
        }

        public void Stop()
        {
        }

        public void TrackTarget(float levelTracking, Transform target)
        {
        }
    }
}
