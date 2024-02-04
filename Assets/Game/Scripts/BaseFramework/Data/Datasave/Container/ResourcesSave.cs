using Assets.Game.Scripts.BaseFramework.Architecture;
using CodeStage.AntiCheat.ObscuredTypes;
using Foundation.Game.Time;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResourcesSave : BaseDatasave
{
    [ShowInInspector]
    public Dictionary<EResource, double> Resources;
    public DateTime LastTimeChargeEnergy;


    public ResourcesSave()
    {
        Resources = new Dictionary<EResource, double>();
        Resources.Add(EResource.Energy, 0);
        Resources[EResource.Energy] = 20;
        LastTimeChargeEnergy = UnbiasedTime.UtcNow;
    }

    public ResourcesSave(string key) : base(key)
    {
        Resources = new Dictionary<EResource, double>();
        Resources.Add(EResource.Energy, 0);
        Resources[EResource.Energy] = 20;
        LastTimeChargeEnergy = UnbiasedTime.UtcNow;
    }

    public void IncreaseResources(params ResourceData[] data)
    {
        foreach (var dt in data)
        {
            IncreaseResource(dt.Resource, dt.Value, false);
        }
        Save();
    }


    [Button]
    public void IncreaseResource(EResource Resource, double value, bool save = true)
    {
        if (!Resources.ContainsKey(Resource))
        {
            Resources.Add(Resource, 0);
        }
        Resources[Resource] += value;

        if (Resource == EResource.Energy)
        {
        }
        if (save)
        {
            Save();
        }
        Messenger.Broadcast(EventKey.UpdateResource, Resource);
        if (FirebaseAnalysticController.Instance != null)
        {
            FirebaseAnalysticController.Instance.AddTrackingResourceEarn(Resource, value);
        }
    }
    public void IncreaseResource(ResourceData data, bool save = true)
    {
        IncreaseResource(data.Resource, data.Value, save);
    }
    public void DecreaseResource(EResource Resource, double value, bool save = true)
    {
        if (!Resources.ContainsKey(Resource)) return;
        Resources[Resource] -= value;
        if (Resources[Resource] <= 0)
        {
            Resources[Resource] = 0;
        }
        if (save)
        {
            Save();
        }
        Messenger.Broadcast(EventKey.UpdateResource, Resource);
    }

    public void DecreaseResources(params ResourceData[] data)
    {
        foreach (var dt in data)
        {
            DecreaseResource(dt.Resource, dt.Value, false);
        }
        Save();
    }

    public void DecreaseResource(ResourceData data, bool save = true)
    {
        DecreaseResource(data.Resource, data.Value, save);
    }
    public override void Fix()
    {
        CalculateEnergyOffline();
        var energy = GetResource(EResource.Energy);
        if (energy >= Architecture.Get<EnergyService>().GetCapacity())
        {
            //Resources[EResource.Energy] = (long)EnergyController.Instance.Capacity.Value;
            //Messenger.Broadcast(EventKey.UpdateResource, EResource.Energy);
            LastTimeChargeEnergy = UnbiasedTime.UtcNow;
            Save();
        }
    }
    private void CalculateEnergyOffline()
    {
        var energyS = GetResource(EResource.Energy);
        if (energyS >= Architecture.Get<EnergyService>().GetCapacity())
        {
            return;
        }
        var timeNow = UnbiasedTime.UtcNow;
        var timePassed = (long)(timeNow - LastTimeChargeEnergy).TotalSeconds;
        var energy = timePassed / (new ValueConfigSearch("Time_Recharge_Energy").IntValue);
        Debug.Log("Add " + energy + " Energy");
        if (energy > 0)
        {
            IncreaseResource(EResource.Energy, energy);
            LastTimeChargeEnergy = LastTimeChargeEnergy.AddSeconds(energy * (new ValueConfigSearch("Time_Recharge_Energy").IntValue));
        }
        Save();
    }
    public double GetResource(EResource type)
    {
        if (Resources.ContainsKey(type) == false)
        {
            return 0;
        }
        return Resources[type];
    }

    public bool HasResource(ResourceData cost)
    {
        return GetResource(cost.Resource) >= cost.Value;
    }

    public void IncreaseResource(List<LootParams> lootData)
    {
        foreach (var item in lootData)
        {
            var rs = item.Data as ResourceData;
            IncreaseResource(rs);
        }
    }

    public bool HasResource(EResource pigCoin, int target)
    {
        return GetResource(pigCoin) >= target;
    }
}
