using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GroupNpcSpawn : MonoBehaviour
{
    [SerializeField] private List<NpcSpawnPoint> spawnPoints = new List<NpcSpawnPoint>();

    public NpcSpawnPoint GetSpawnPoint(ESpawnPoint spawnPoint)
    {
        return spawnPoints.Find(t => t.SpawnPoint == spawnPoint);
    }

    public NpcSpawnPoint GetSpawnPointNearest(Vector3 pos)
    {
        float maxDistance = 1000;
        NpcSpawnPoint res = spawnPoints[0];
        foreach (var point in spawnPoints)
        {
            if (Vector2.Distance(pos, point.transform.position) < maxDistance)
            {
                maxDistance = Vector2.Distance(pos, point.transform.position);
                res = point;
            }
        }
        return res;
    }

    private void OnValidate()
    {
        spawnPoints = GetComponentsInChildren<NpcSpawnPoint>().ToList();
    }
}