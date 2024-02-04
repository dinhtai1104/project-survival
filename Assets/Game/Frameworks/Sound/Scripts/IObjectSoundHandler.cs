using UnityEngine;

public interface IObjectSoundHandler
{
    void Play();
    void Play(AudioClip audioClip);
    void PlayLoop(AudioClip clip, float vol = 1);
    void PlayOneShot(AudioClip audioClip, float v);
    void PlayOneShot(Vector3 position, AudioClip audioClip, float v);
    void Stop();
}