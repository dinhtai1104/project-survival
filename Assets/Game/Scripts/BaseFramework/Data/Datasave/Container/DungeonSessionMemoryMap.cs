using System;
using System.Collections.Generic;

/// <summary>
/// Stage[i].Room = roomId[i]
/// </summary>
[System.Serializable]
public class DungeonSessionMemoryMap
{
    public List<int> roomId;
    public DungeonSessionMemoryMap()
    {
        roomId = new List<int>();
    }
    public void SetIdRooms(List<int> roomId)
    {
        this.roomId = roomId;
    }
    public void Clear()
    {
        roomId.Clear();
    }

    public int GetRoom(int currentStage)
    {
        return roomId[currentStage];
    }
}