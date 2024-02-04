using Game.Effect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class HealEffect : EffectAbstract
{
    public static float LAST_PLAYSFX_TIME = 0,SFX_SPACING=0.5f;
    bool isPlaying = false;
    [SerializeField]
    private TMPro.TextMeshPro healText;
    [SerializeField]
    private ParticleSystem ps;
    [SerializeField]
    private ParticleSystem[] subPs;
    [SerializeField]
    private AudioClip [] clip;
    [SerializeField]
    private float soundVol = 1;

    [SerializeField]
    private bool limitSFX=false;
    public override bool IsUsing()
    {
        return isPlaying && ps.isPlaying;
    }


 
    public override EffectAbstract Active(Vector3 pos,string text)
    {
        isPlaying = true;
        transform.localScale = Vector3.one;
        transform.position = pos;
        this.healText.SetText(text);
        gameObject.SetActive(true);
        if (ps != null)
        {
            ps.Play();
        }
        if (clip != null && clip.Length > 0)
        {
            Sound.Controller.Instance.PlayOneShot(clip[Random.Range(0, clip.Length)], soundVol);
        }
        Invoke(nameof(Clear), 1.5f);
        return this;
    }

    void Clear()
    {
        isPlaying = false;
    }

    public override void Stop()
    {
        foreach (var ps in subPs)
        {
            var main = ps.main;
            main.startSizeMultiplier = 1;
        }

        ps.Stop();
    }
}
