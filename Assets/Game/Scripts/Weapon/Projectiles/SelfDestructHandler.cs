using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Bullet
{
    public class SelfDestructHandler : MonoBehaviour
    {
        public BulletBase Base { get { if (bulletBase == null) bulletBase = GetComponent<BulletBase>(); return bulletBase; } }

        private BulletBase bulletBase;

        public ValueConfigSearch Time;
        public ValueConfigSearch TimeUntilExplode;

        private float timeUntilExplode;

        float timer;
        bool activeEffect = false;
        public ParticleSystem effect;
        private void OnEnable()
        {
            activeEffect = false;
            timer = 0;
            timeUntilExplode = Time.FloatValue - TimeUntilExplode.FloatValue;
        }

        private void Update()
        {
            if (timer >= Time.FloatValue)
            {
                SelfDestruct();
            }
            else
            {
                timer += GameTime.Controller.DeltaTime();
            }

            if (!activeEffect && timer>=timeUntilExplode)
            {
                //effect.gameObject.SetActive(true);
                effect.Play();
                activeEffect = true;
            }
        }

        void SelfDestruct()
        {
            //effect.gameObject.SetActive(false);

            timer = 999;
            foreach (var impactHandler in Base.ImpactHandlers)
            {
                impactHandler.ForceImpact();
            }
        }
        private void OnDisable()
        {

        }
    }
}