using UnityEngine;

namespace Game.GameActor
{
    public class EnemyBoss : EnemyBase
    {
        public override ECharacterType GetCharacterType()
        {
            return ECharacterType.Boss;
        }
        protected override void PlayGetHitSFX()
        {
            if (Time.time - effectTime > 0.2f)
            {
                if (soundData != null && UnityEngine.Random.Range(0f, 1f) > 0.5f)
                {
                    this.SoundHandler.PlayOneShot(soundData.getHitSFXs.Random(), 1f);
                    effectTime = Time.time;
                }
            }
        }
        protected override void PlayDeadSFX()
        {
            this.SoundHandler.PlayOneShot(soundData.dieSFXs[Random.Range(0, soundData.dieSFXs.Length)], 1f);

        }
        public override void StartBehaviours()
        {
            base.StartBehaviours();
            SoundHandler.PlayOneShot(soundData.startSFXs.Random(),1);
        }
        float time = 10;
        protected override void Update()
        {
            base.Update();
            if (time > 0)
            {
                time -= GameTime.Controller.DeltaTime();
            }
            else
            {
                time = UnityEngine.Random.Range(9, 15);
                try
                {
                    SoundHandler.PlayOneShot(soundData.growSFXs.Random(), 1);
                }
                catch (System.Exception e)
                {

                }
            }
        }
    }
}