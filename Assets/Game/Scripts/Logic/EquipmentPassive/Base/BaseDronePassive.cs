using Cysharp.Threading.Tasks;
using Game.GameActor;
using System;
using UnityEngine.AddressableAssets;

public class BaseDronePassive : BaseEquipmentPassive
{
    public AssetReferenceGameObject droneRef;
    private Drone drone;

    private void OnEnable()
    {
        Messenger.AddListener<ActorBase>(EventKey.ChangePlayer, OnChangePlayer);
    }

   

    private void OnDisable()
    {
        Messenger.RemoveListener<ActorBase>(EventKey.ChangePlayer, OnChangePlayer);
    }

    private void OnChangePlayer(ActorBase character)
    {
        if (character == Caster)
        {
            Logger.Log("SET ACTIVE DRONE");
            drone.SetActive(true);
            drone.StartBehaviours();
        }
    }

    public override void Initialize(ActorBase actor)
    {
        base.Initialize(actor);
    }
    public override void Play()
    {
        Logger.Log("PLAY " + ToString());
        base.Play();

        //string equipmentId = itemEquipment.Id;
        var rarity = itemEquipment.EquipmentRarity;
        PrepareDrone(droneRef.RuntimeKey.ToString(), rarity).Forget();

    }

   
    async UniTask PrepareDrone(string id,ERarity rarity)
    {
        //prepare drone
        drone = (await Game.Pool.GameObjectSpawner.Instance.GetAsync(id, Game.Pool.EPool.Pernament)).GetComponent<Drone>();
        await drone.SetUp(GetStatDrone( rarity));
        drone.SetActive(false);
        Messenger.Broadcast<Drone>(EventKey.DroneSpawn, drone);

    }
    public override void Remove()
    {
        Logger.Log("remove: " + ToString());
        base.Remove();
        drone.SetActive(false);
    }

    protected virtual IStatGroup GetStatDrone(ERarity rarity)
    {
        return null;
    }
}
