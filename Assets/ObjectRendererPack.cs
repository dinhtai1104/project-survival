using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRendererPack : MonoBehaviour
{
    public SpritePack[] spritePacks;

    [System.Serializable]
    public struct SpritePack
    {
        public Sprite[] sprites;
    }

    private new SpriteRenderer renderer;
    private void OnEnable()
    {
        renderer = GetComponent<SpriteRenderer>();
        GameController.onStageReady += OnStageInit;


    }

    private void OnStageInit(int mode, int dungeonId, int stageId, EDungeonEvent eventType)
    {
        //portalSR.sprite = portalSprites[dungeonId];
        renderer.sprite = spritePacks[mode].sprites[eventType==EDungeonEvent.None?dungeonId:(int)eventType];
    }



    private void OnDisable()
    {
        GameController.onStageReady -= OnStageInit;
    } 
    private void OnDestroy()
    {
        GameController.onStageReady -= OnStageInit;
    }
}
