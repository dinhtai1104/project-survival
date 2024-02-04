using DG.Tweening;
using Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Sound { 
    public class Controller : MonoBehaviour
    {
        private static bool sfxEnable, musicEnable;
        public static bool SfxEnable
        {
            get { return DataManager.Save.General.IsOnSFX; }

            set
            {
                sfxEnable = value;
                onSoundChange?.Invoke(sfxEnable);
                PlayerPrefs.SetInt("Sound", value ? 1 : 0);
            }
        }
        public static bool MusicEnable
        {
            get { return DataManager.Save.General.IsOnMusic; }

            set
            {
                musicEnable = value;
                if (musicEnable)
                {
                    Instance.ContinueMusic();
                }
                else
                {
                    Instance.PauseMusic();
                }
                PlayerPrefs.SetInt("Music", value ? 1 : 0);
            }
        }
        public delegate void OnSoundChange(bool state);
        public static OnSoundChange onSoundChange;
        public static Controller Instance;
        [SerializeField]
        private AudioSource sfxPlayer, musicPlayer;

        public SoundData soundData;
        private float masterMusicVolumn = 1;

        private float lastVolumnMusic = 1f;
        private void Start()
        {
            if (Instance == null)
            {
                sfxEnable = PlayerPrefs.GetInt("Sound", 1) == 1;
                musicEnable = PlayerPrefs.GetInt("Music", 1) == 1;
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Addressables.LoadAssetAsync<SoundData>("Sound Data").Completed += op =>
                {
                    soundData = op.Result;
                };
                masterMusicVolumn = musicPlayer.volume;
            }
            else
            {
                Destroy(gameObject);
            }
        }
       
        public bool IsReady()
        {
            return soundData != null;
        }
        public void PlayOneShot(Vector3 worldPosition,AudioClip clip, float vol = 1)
        {
            if (SfxEnable && clip != null)
            {
                Game.Pool.GameObjectSpawner.Instance.Get("ObjectSoundHandler", obj =>
                 {
                     ObjectSoundHandler objectSoundHandler = obj.GetComponent<ObjectSoundHandler>();
                     objectSoundHandler.willAutoDeactive = true;
                     objectSoundHandler.PlayOneShot(worldPosition, clip, vol);
                 });
            }
        }

        public async void PlayOneShot(string address, float vol = 1)
        {
            var au = await ResourcesLoader.Instance.LoadAsync<AudioClip>(address);
            PlayOneShot(au, vol);
        }

        public void PlayOneShot(AudioClip clip, float vol = 1)
        {
            if (SfxEnable && clip!=null)
            {
                sfxPlayer.PlayOneShot(clip, vol);
            }
        }
        public void PlayMusic(AudioClip clip, float vol = 1)
        {
            if (MusicEnable)
            {
                if (musicPlayer.clip == clip) return;
                lastVolumnMusic = vol;
                Game.Asset.AssetLoader.Release(musicPlayer.clip);
                musicPlayer.clip = clip;
                musicPlayer.volume = masterMusicVolumn*vol;
                musicPlayer.loop = true;
                musicPlayer.Play();
            }
        }
        public AudioSource GetMusicPlayer()
        {
            return musicPlayer;
        }
        public void StopMusic()
        {
            Game.Asset.AssetLoader.Release(musicPlayer.clip);

            musicPlayer.clip = null;
            musicPlayer.Stop();
        }
        public void PauseMusic()
        {
            if (musicPlayer.isPlaying)
            {
                musicPlayer.DOFade(0, 1f).OnComplete(() =>
                {
                    musicPlayer.Pause();
                });
            }
        }
        public void ContinueMusic()
        {
            musicPlayer.DOFade(lastVolumnMusic, 1f).OnComplete(() =>
            {
                musicPlayer.UnPause();
            });
        }
        public void PlayClickSFX()
        {
            if (soundData.clickSFXs.Length == 0) return;
            PlayOneShot(soundData.clickSFXs[Random.Range(0,soundData.clickSFXs.Length)]);
        }

        public void Play(AudioSource source, AudioClip clip, bool loop = false)
        {
            source.clip = clip;
            source.loop = loop;

            if (!SfxEnable) return;
            source.Play();
        }
        public void Stop(AudioSource source)
        {
            source.Stop();
        }

        public void UpdateMusic()
        {
            if (MusicEnable == false)
            {
                musicPlayer.Stop();
            }
            else
            {
                musicPlayer.Play();
            }
        }
    }
}