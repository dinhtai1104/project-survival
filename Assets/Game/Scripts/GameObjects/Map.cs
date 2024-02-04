using com.assets.loader.addressables;
using com.assets.loader.core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map : MonoBehaviour
{
    public ERoomType roomType=ERoomType.Normal;
    public Collider2D boundary;
    public GroupNpcSpawn groupNpcSpawn;
    public Transform playerSpawnPoint;
    public Transform portalPoint;

#if UNITY_EDITOR
    private void OnValidate()
    {
        try
        {
            groupNpcSpawn = GetComponentInChildren<GroupNpcSpawn>();
            playerSpawnPoint = transform.Find("PlayerSpawnPoint");
            for (int i = 0; i < groupNpcSpawn.transform.childCount; i++)
            {
                if (!groupNpcSpawn.transform.GetChild(i).gameObject.activeSelf) continue;
                var point = groupNpcSpawn.transform.GetChild(i).GetComponent<NpcSpawnPoint>();
                point.SpawnPoint = (ESpawnPoint)i;
            }
        }
        catch { Logger.LogError("ERROR VALIDATING MAP:" + gameObject.name); };
    }
#endif
    private void OnEnable()
    {
    }


}
