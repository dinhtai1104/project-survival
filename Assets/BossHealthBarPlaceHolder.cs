using Game.GameActor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthBarPlaceHolder : MonoBehaviour
{
    private void OnEnable()
    {
        Messenger.AddListener<HealthBarBase, ActorBase>(EventKey.OnHealthBarSpawn, OnSpawn);
    }
    private void OnDisable()
    {
        Messenger.RemoveListener<HealthBarBase, ActorBase>(EventKey.OnHealthBarSpawn, OnSpawn);


    }
    private void OnSpawn(HealthBarBase arg1, ActorBase arg2)
    {
        if (arg2.GetCharacterType() == ECharacterType.Boss)
        {
            arg1.transform.localScale = Vector3.one;
            arg1.transform.localPosition=Vector3.zero;
            arg1.transform.SetParent(transform,false);
        }
    }
}
