using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    public class MonsterWaveData
    {
        public int WaveId { get; }
        public List<int> MonsterId { get; }
        public float ClusterChance { get; }
        public int MaxCount { get; }
        public float SpawnChance { get; }
        public float SpawnTime { get; }

        public MonsterWaveData(int waveId, List<int> monsterId, float clusterChance, int maxCount, float spawnChance, float spawnTime)
        {
            WaveId = waveId;
            MonsterId = monsterId;
            ClusterChance = clusterChance;
            MaxCount = maxCount;
            SpawnChance = spawnChance;
            SpawnTime = spawnTime;
        }
    }
}
