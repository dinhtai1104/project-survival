using BansheeGz.BGDatabase;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
/// <summary>
/// Room config, each room has many wave enemy
/// </summary>
[System.Serializable]
public class DungeonRoomEntity
{
    public int RoomId;
    public int WaveId;
    public string Map;
    public List<LootParams> Rewards;
    public List<LootParams> DropRates;
    public EDifficult Difficult;
    [ShowInInspector]
    public Dictionary<int, DungeonWaveEntity> Waves = new Dictionary<int, DungeonWaveEntity>();

    public DungeonStageEntity StageLinked;

    public void AddWave(DungeonWaveEntity wave)
    {
        if (!Waves.ContainsKey(wave.WaveId))
        {
            Waves.Add(wave.WaveId, wave);
        }
    }
    public void AddEnemyToWave(EnemyWaveEntity enemy, int WaveId)
    {
        Waves[WaveId].AddEnemy(enemy);
    }

    public bool HasWave(int waveId)
    {
        return Waves.ContainsKey(waveId);
    }

    public DungeonRoomEntity(BGEntity e)
    {
        // get base data of dungeon room
        RoomId = e.Get<int>("Room");
        Rewards = new List<LootParams>();
        DropRates = new List<LootParams>();
        WaveId = e.Get<int>("Wave");
        Map = e.Get<string>("Map");
        System.Enum.TryParse(e.Get<string>("Difficult"), out Difficult);

        var rewardData = e.Get<List<string>>("Reward");
        if (rewardData != null)
        {
            foreach (var data in rewardData)
            {
                var lootData = new LootParams(data);
                Rewards.Add(lootData);
            }
        }
        var dropData = e.Get<List<string>>("DropRate");
        if (dropData != null)
        {
            foreach (var data in dropData)
            {
                var dropRateData = new DropRateData(data);
                var lootData = new LootParams(dropRateData.LootType, dropRateData);
                DropRates.Add(lootData);
            }
        }
    }
}