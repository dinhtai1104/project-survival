using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

/// <summary>
/// Dungeon map has many stage
/// </summary>
[System.Serializable]
public class DungeonEntity
{
    public int Dungeon;
    [ShowInInspector]
    public List<(DungeonStageEntity,int)> Stages;
    public bool[] buffStages;

    public DungeonEntity(int dungeon)
    {
        Dungeon = dungeon;
        Stages = new List<(DungeonStageEntity,int)>();
    }

    public void AddStage(DungeonStageEntity stageEntity, int buffCount)
    {
        Stages.Add((stageEntity,buffCount));
    }
    public DungeonStageEntity GetStage(int stage)
    {
        return Stages[stage].Item1;
    }
    public bool IsBuffStage(int stage)
    {
        return Stages[stage].Item2 > 0;
    }
    public int GetBuffStage(int stage)
    {
        return Stages[stage].Item2;
    }
}