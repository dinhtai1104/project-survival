using System.Collections.Generic;

/// <summary>
/// Dungeon wave has many wave enemy, each wave has many enemies
/// </summary>
[System.Serializable]
public class DungeonWaveEntity
{
    public int WaveId;
    public List<EnemyWaveEntity> EnemiesWave;
    public float DelayToWave;
    public DungeonRoomEntity RoomLink;
    public DungeonWaveEntity(int WaveId)
    {
        this.WaveId = WaveId;
        EnemiesWave =new List<EnemyWaveEntity>();
    }
    public void AddEnemy(EnemyWaveEntity enemy)
    {
        EnemiesWave.Add(enemy);
        enemy.RoomLinked = RoomLink;
    }
}