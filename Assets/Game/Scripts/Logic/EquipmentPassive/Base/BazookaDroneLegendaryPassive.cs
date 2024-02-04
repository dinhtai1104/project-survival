using Game.GameActor;

public class BazookaDroneLegendaryPassive : BaseEquipmentPassive
{
    public override void Initialize(ActorBase actor)
    {
        base.Initialize(actor);
        Logger.Log("Initialize BazookaDroneLegendaryPassive");
    }
    public override void Play()
    {
        base.Play();

        string equipmentId = itemEquipment.Id;
        Logger.Log("Play BazookaDroneLegendaryPassive");

    }


}