using Game.Effect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EffectPackHandler : EffectAbstract
{
    [SerializeField]
    private ParticleSystem[] psList;
    [SerializeField]
    private AudioClip[] clip;
    [SerializeField]
    private float soundVol = 1;
    public override bool IsUsing()
    {
        return psList[0].isPlaying;
    }
   
    public override EffectAbstract Active(Vector3 pos,Vector2 size)
    {
        transform.position = pos;
        gameObject.SetActive(true);
        foreach(ParticleSystem ps in psList)
        {
            ParticleSystem.ShapeModule shape = ps.shape;
            shape.scale = size;
            shape.position = new Vector3(0, size.y / 2f,0);
            ps.Play();
        }
        if (clip != null && clip.Length > 0)
        {
            Sound.Controller.Instance.PlayOneShot(clip[Random.Range(0, clip.Length)], soundVol);
        }
        return base.Active(size);
    }
    public override void Stop()
    {
    }

}
