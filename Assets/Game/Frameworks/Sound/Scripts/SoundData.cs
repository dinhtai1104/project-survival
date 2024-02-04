using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Sound
{
    [CreateAssetMenu(menuName = "SoundData/Data")]
    public class SoundData : ScriptableObject
    {
        public AssetReferenceT<AudioClip> menuTheme, gameTheme, bossTheme,bonusStageTheme;
        public AssetReferenceT<AudioClip>[]  gameThemes, winSFXs, loseSFXs;
        public AudioClip purchaseSFX,startSFX;
        public AudioClip[]  coinSfxs, clickSFXs,playerWinSFX,playerLoseSFX,collectSFXs;

        
        public AudioClip GetCoinSFX()
        {
            return coinSfxs[Random.Range(0, coinSfxs.Length)];
        }
        public void PlayWinSFX()
        {
            Game.Asset.AssetLoader.Load<AudioClip>(winSFXs.Random(), result =>
            {
                Controller.Instance.PlayOneShot(result, 1);
            });
        }
        public void PlayLoseSFX()
        {
            Game.Asset.AssetLoader.Load<AudioClip>(loseSFXs.Random(), result =>
            {
                Controller.Instance.PlayOneShot(result, 1);
            });
        }
        public void PlayMenuTheme() 
        {
            Game.Asset.AssetLoader.Load<AudioClip>(menuTheme, result =>
            {
                Controller.Instance.PlayMusic(result, 0.6f);
            });
        }
        public void PlayFountainTheme()
        {
            Game.Asset.AssetLoader.Load<AudioClip>(bonusStageTheme, result =>
            {
                Controller.Instance.PlayMusic(result, 0.6f);
            });
        }
        public void PlayGameTheme()
        {
            Game.Asset.AssetLoader.Load<AudioClip>(gameThemes.Random(), result =>
            {
                Controller.Instance.PlayMusic(result, 0.3f);

            });
        }
        public void PlayBossTheme()
        {
            Game.Asset.AssetLoader.Load<AudioClip>(bossTheme, result =>
            {
                Controller.Instance.PlayMusic(result, 0.4f);
            });
        }

        public void PlayStartSFX()
        {
            Controller.Instance.PlayOneShot(startSFX,1f);
        }
    }
   
}