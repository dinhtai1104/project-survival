using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
[CreateAssetMenu (menuName ="GameMode")]
public class GameModeData : ScriptableObject
{
    public GameMode gameMode = GameMode.Normal;
    public string title;
    public AssetReferenceGameObject gameController;

    public WorldBackGround[] worldBackGrounds;

    public AssetReference GetBackGround(int world,ERoomType type)
    {
        return worldBackGrounds[world].Random(type);
    }
    public Color GetWorldColor(int world)
    {
        return worldBackGrounds[world].worldColor;
    }
}

[System.Serializable]
public class WorldBackGround
{
    public string id;
    public Color worldColor;
    public TypeBackGround[] backGrounds;

    public AssetReference Random(ERoomType type)
    {
        return backGrounds[(int)type].Random();
    }
}


[System.Serializable]
public class TypeBackGround
{
    public AssetReference[] backGroundRefs;
    public AssetReference Random()
    {
        return backGroundRefs[UnityEngine.Random.Range(0, backGroundRefs.Length)];
    }
}
