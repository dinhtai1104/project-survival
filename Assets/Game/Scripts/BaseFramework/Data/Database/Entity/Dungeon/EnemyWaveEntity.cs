using BansheeGz.BGDatabase;

[System.Serializable]
public class EnemyWaveEntity
{
    public int Wave;
    public string Enemy;
    public int EnemyLevel;
    public ESpawnPoint SpawnPoint;
    public float Delay;
    public DungeonRoomEntity RoomLinked;
    
    public EnemyWaveEntity(BGEntity e)
    {
        Wave = e.Get<int>("Wave");
        Enemy = e.Get<string>("Enemy");
        EnemyLevel = e.Get<int>("EnemyLevel");
        System.Enum.TryParse(e.Get<string>("SpawnPoint"), out SpawnPoint);
        Delay = e.Get<float>("Delay");
    }

    public override string ToString()
    {
        return Wave+" "+Enemy+" "+EnemyLevel+" "+SpawnPoint+" "+Delay;
    }
}