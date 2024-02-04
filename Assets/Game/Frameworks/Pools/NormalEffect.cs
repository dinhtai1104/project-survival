using Game.Effect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NormalEffect : EffectAbstract
{
    public static float LAST_PLAYSFX_TIME = 0,SFX_SPACING=0.15f;
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
        if (ps == null) return false;
        return ps.isPlaying;
    }
    void PlaySFX()
    {
        if (clip != null && clip.Length > 0 && (!limitSFX || (limitSFX && Time.time - LAST_PLAYSFX_TIME > SFX_SPACING)))
        {
            Sound.Controller.Instance.PlayOneShot(clip[Random.Range(0, clip.Length)], soundVol);

            if (limitSFX)
            {
                LAST_PLAYSFX_TIME = Time.time;
            }
        }
    }
    public override void Active()
    {
        base.Active();
        if (ps != null)
        {
            ps.Play();
        }
        PlaySFX();

    }

    public override void Active(Transform parent)
    {
        base.Active(parent);
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        Active();
    }

    public override EffectAbstract Active(Vector3 pos, float size)
    {
        transform.localScale = Vector3.one * size;
        transform.position = pos;
        gameObject.SetActive(true);

        if (ps != null)
        {
            ps.Play();
        }
        PlaySFX();

        return this;
    }
    public override EffectAbstract Active(Vector3 pos)
    {
        transform.localScale = Vector3.one;
        transform.position = pos;
        gameObject.SetActive(true);
        if (ps != null)
        {
            ps.Play();
        }
        PlaySFX();

        return this;
    }
    public override EffectAbstract Active(Vector3 pos,Vector3 direction)
    {
        transform.localScale = Vector3.one;
        transform.position = pos;
        transform.right = direction;
        gameObject.SetActive(true);
        if (ps != null)
        {
            ps.Play();
        }
        PlaySFX();

        return this;
    }

    public override EffectAbstract Active(MeshRenderer renderer,float scale=1)
    {
        transform.localScale = Vector3.one;
        gameObject.SetActive(true);

        foreach(var ps in subPs)
        {
            var shape = ps.shape;
            shape.meshRenderer = renderer;
            var main = ps.main;
            main.startSizeMultiplier = scale;
        }

        if (ps != null)
        {
            ps.Play();
        }
        PlaySFX();

        return this;
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
