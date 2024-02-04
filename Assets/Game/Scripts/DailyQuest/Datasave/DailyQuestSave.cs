using System;

[System.Serializable]
public class DailyQuestSave
{
    public int Id;
    public EMissionDaily Mission;
    public int Progress;
    public bool Received;

    public void Receive()
    {
        Received = true;
        DataManager.Save.DailyQuest.Save();
    }

    public void Save()
    {
        DataManager.Save.DailyQuest.Save();
    }
}