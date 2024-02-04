using Game.GameActor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawnListenner : MonoBehaviour
{

    private void OnDisable()
    {
        Messenger.RemoveListener<ActorBase,bool, int>(EventKey.ActorSpawn, OnSpawn);
    }

    private void OnDestroy()
    {
        Messenger.RemoveListener<ActorBase,bool, int>(EventKey.ActorSpawn, OnSpawn);
    }
    private void OnEnable()
    {
        Messenger.AddListener<ActorBase,bool, int>(EventKey.ActorSpawn, OnSpawn);
    }
    private void OnSpawn(ActorBase actor,bool active, int group)
    {
        if (actor.GetCharacterType() != ECharacterType.Boss) return;
#if UNITY_EDITOR
        if (DataManager.Save.General.GetInt($"BossIntro_{actor.gameObject.name}") == 1) return;
#endif
        UI.PanelManager.Create(string.Format(AddressableName.UIIntroBossPanel,actor.gameObject.name), panel =>
        {
            ((UIIntroBossPanel)panel).SetUp(actor.gameObject.name);
        });
        DataManager.Save.General.SetInt($"BossIntro_{actor.gameObject.name}", 1);

    }
}
