using System.Collections.Generic;

public class EventTracking
{
    public string id;
}

public class ResourceTracking : EventTracking
{
    public int resourceId;
    public string source;
}

public class TutorialTracking : EventTracking
{
    public int stepId;
    public string stepName;
}
public class BattleEndTracking : EventTracking
{
    public string battleType;
    public string battleId;
    public int stage;
    public int result;
    public int playTime;
    public string heroUsed;
    public int remainingHp;
    public string battleData;
}
public class PurchaseClickTracking : EventTracking
{
    public string productId;
}
public class EarnResourceTracking : EventTracking
{

}
public class BuyResourceTracking : EventTracking
{

}
public class SpendResourceTracking : EventTracking
{
}
public class HeroEquipmentTracking : EventTracking
{
    public string action;
    public int heroId;
    public int equipmentId;
}