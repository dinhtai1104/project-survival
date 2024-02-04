using Assets.Game.Scripts.Logic.Objects;
using com.mec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Logic.Tasks.Boss_10_1
{
    public class Enemy10001BulletCircle : CharacterObjectBase
    {
        private List<BulletSimpleDamageObject> listBullet = new List<BulletSimpleDamageObject>();
        public void AddBullet(Vector3 localTarget, BulletSimpleDamageObject bullet)
        {
            listBullet.Add(bullet);
            (bullet.Movement as AutoMoveToLocalPosition).LocalPositionTarget = localTarget;
            bullet.Play();
            bullet.onBeforeDestroy += (c) =>
            {
                listBullet.Remove(bullet);
            };
        }

        protected override void OnDisable()
        {
            listBullet.Clear();
            base.OnDisable();
        }

        protected override IEnumerator<float> _OnUpdate()
        {
            while (true)
            {
                SkillEngine.Ticks();
                foreach (var i in listUpdateParallel)
                {
                    i.OnUpdate();
                }

                if (listBullet.Count == 0)
                {
                    yield return Timing.WaitForSeconds(1f);
                    if (listBullet.Count == 0)
                    {
                        break;
                    }
                }
                yield return Timing.DeltaTime;
            }

            PoolManager.Instance.Despawn(gameObject);
        }
    }
}
