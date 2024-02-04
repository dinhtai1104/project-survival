using Cysharp.Threading.Tasks;
using Game.GameActor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyShooter : Character
{
    public override ECharacterType GetCharacterType()
    {
        return ECharacterType.Object;
    }
    protected override async UniTask SetUp()
    {
        await base.SetUp();
        Tagger.AddTag(ETag.Object);


    }

    public override bool GetHit(DamageSource damageSource, IDamageDealer dealer)
    {
        return false;
    }
    public override void Dead()
    {
    }


    protected override void OnEnable()
    {
        base.OnEnable();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();

    }
    protected override void OnDisable()
    {
        base.OnDisable();

    }
    public override void OnVisible(bool isVisible)
    {

    }

}
