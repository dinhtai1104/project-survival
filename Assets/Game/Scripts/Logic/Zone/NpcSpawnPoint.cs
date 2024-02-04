using UnityEngine;

public class NpcSpawnPoint : MonoBehaviour
{
    [SerializeField] private ESpawnPoint spawnPoint;
    public ESpawnPoint SpawnPoint { get { return spawnPoint; } set { gameObject.name = value.ToString(); spawnPoint = value; } }
    public Vector3 Position => transform.position;
}