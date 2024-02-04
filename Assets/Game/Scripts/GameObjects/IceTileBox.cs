using com.mec;
using DG.Tweening;
using Game.GameActor;
using Game.Pool;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum EIceTileBox
{
    Broken,
    Fixed
}

public class IceTileBox : TileInstanceBase
{
    public EIceTileBox eIce = EIceTileBox.Broken;
    public SpriteRenderer spriteRenderer;
    private Collider2D collider2D;
    [ShowIf("eIce", EIceTileBox.Broken)] public float timeDelay = 1f;
    [ShowIf("eIce", EIceTileBox.Broken)] public float timeDelay_Collider = 1f;
    [ShowIf("eIce", EIceTileBox.Broken)] public string VFX_Ice_Broken = "VFX_Ice_Broken";
    [ShowIf("eIce", EIceTileBox.Broken)] public List<Sprite> frames = new List<Sprite>();
    private bool isBroken = false;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (eIce == EIceTileBox.Fixed) return;
        if (isBroken) return;
        var target = collision.transform.GetComponentInParent<ActorBase>();
        if (target == null) return;

        if (!target.Tagger.HasTag(ETag.Player))
        {
            return;
        }
        isBroken = true;
        Timing.RunCoroutine(_DelayBroken(), gameObject);
        Timing.RunCoroutine(_DelayCloseCollider(), gameObject);
    }

    protected override void OnDisable()
    {
        Timing.KillCoroutines(gameObject);
        DOTween.Kill(gameObject);
        base.OnDisable();
    }

    private IEnumerator<float> _DelayCloseCollider()
    {
        yield return Timing.WaitForSeconds(timeDelay_Collider);
        GetComponent<Collider2D>().enabled = false;

    }

    private IEnumerator<float> _DelayBroken()
    {
        spriteRenderer.transform.DOShakePosition(1f, UnityEngine.Random.Range(0.05f, 0.1f)).SetId(gameObject);
        float timeBtwSprite = (timeDelay - 0.1f) / frames.Count;
        for (int i = 0; i < frames.Count; i += 1)
        {
            spriteRenderer.sprite = frames[i];
            yield return Timing.WaitForSeconds(timeBtwSprite);
        }
        GameObjectSpawner.Instance.Get(VFX_Ice_Broken, (t) =>
        {
            t.GetComponent<Game.Effect.EffectAbstract>().Active(gameObject.transform.position);
        });
        yield return Timing.WaitForSeconds(0.1f);
        gameObject.SetActive(false);
    }
}