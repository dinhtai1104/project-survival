using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSoundHandler : MonoBehaviour, IObjectSoundHandler
{
    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private bool playOnAwake = true;
    public bool willAutoDeactive = false;

    private void OnEnable()
    {
        if (playOnAwake && Sound.Controller.SfxEnable)
        {
            Play();
        }
    }
    public void Play()
    {
        audioSource.Play();
    }
    public void Stop()
    {
        audioSource.Stop();
    }
    void Deactive()
    {
        gameObject.SetActive(false);
    }
    public void PlayOneShot(Vector3 position, AudioClip audioClip, float v)
    {
        if (Sound.Controller.SfxEnable)
        {
            transform.position = position;
            audioSource.PlayOneShot(audioClip, v);
            if (willAutoDeactive) Invoke(nameof(Deactive), audioClip.length);
        }
    }
    public void PlayOneShot(AudioClip audioClip, float v)
    {
        if (Sound.Controller.SfxEnable && audioClip != null)
        {
            audioSource.PlayOneShot(audioClip, v);
            if (willAutoDeactive) Invoke(nameof(Deactive), audioClip.length);

        }
    }
    public void PlayLoop(AudioClip clip, float vol = 1)
    {
        audioSource.clip = clip;
        audioSource.loop = true;

        audioSource.volume = vol;

        audioSource.Play();
    }
    public void Play(AudioClip audioClip)
    {
        if (Sound.Controller.SfxEnable)
        {
            audioSource.PlayOneShot(audioClip, audioSource.volume);
            if (willAutoDeactive) Invoke(nameof(Deactive), audioClip.length);

        }
    }
}
