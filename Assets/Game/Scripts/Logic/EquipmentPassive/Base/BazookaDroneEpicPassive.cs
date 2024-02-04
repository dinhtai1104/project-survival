using Game.GameActor;

public class BazookaDroneEpicPassive : BaseEquipmentPassive
{
    public override void Initialize(ActorBase actor)
    {
        base.Initialize(actor);
        Logger.Log("Initialize BazookaDroneEpicPassive");
    }
    public override void Play()
    {
        base.Play();

        string equipmentId = itemEquipment.Id;
        Logger.Log("Play BazookaDroneEpicPassive");

    }

}
