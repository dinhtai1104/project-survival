using Assets.Game.Scripts.BaseFramework.Architecture;
using com.mec;
using Foundation.Game.Time;
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnergyService : LiveSingleton<EnergyService>, IService
{
    public Stat Capacity;
    public bool IsUpdate = false;
    public ResourcesSave resource;
    public ulong timeCooldown;
    public void OnInit()
    {
        Capacity = new Stat(20);
        Logger.Log("Service " + this.GetType() + " On Init", Color.blue);
        IsUpdate = false;
        Timing.KillCoroutines(gameObject);
    }

    public void OnStart() 
    {
        Logger.Log("Service " + this.GetType() + " On Start", Color.yellow);
        resource = DataManager.Save.Resources;
        IsUpdate = true;
        Timing.RunCoroutine(_Update(), Segment.RealtimeUpdate, gameObject);
    }
    public void OnDispose() 
    {
        IsUpdate = false;
        Logger.Log("Service " + this.GetType() + " On Dispose", Color.red);
        Timing.KillCoroutines(gameObject);
    }

    private IEnumerator<float> _Update()
    {
        yield return Timing.WaitForSeconds(2f);
        var res = DataManager.Save.Resources;

        while (true)
        {
            if (!IsUpdate)
            {
                yield return Timing.DeltaTime;
                continue;
            }

            var dateNow = UnbiasedTime.UtcNow;
            var lastDate = res.LastTimeChargeEnergy;
            timeCooldown = (ulong)(dateNow - lastDate).TotalSeconds;

            timeCooldown %= (ulong)(new ValueConfigSearch("Time_Recharge_Energy").IntValue);
            timeCooldown = (ulong)new ValueConfigSearch("Time_Recharge_Energy").IntValue - timeCooldown;

            if (res.GetResource(EResource.Energy) >= Capacity.Value)
            {
                res.LastTimeChargeEnergy = UnbiasedTime.UtcNow;
                timeCooldown = (ulong)(new ValueConfigSearch("Time_Recharge_Energy").IntValue);
                yield return Timing.DeltaTime;
                continue;
            }

            //UpdateTimeEnergy

            yield return Timing.WaitForSeconds(1f);
            timeCooldown--;
            if (timeCooldown <= 0)
            {
                timeCooldown = (ulong)(new ValueConfigSearch("Time_Recharge_Energy").IntValue);
                resource.LastTimeChargeEnergy = UnbiasedTime.UtcNow;
                resource.IncreaseResource(EResource.Energy, 1);
            }
        }
    }

    public string GetCooldownTime()
    {
        return $"{timeCooldown / 60:00}:{timeCooldown % 60:00}";
    }
    public void AddCapacityModifier(StatModifier modifier)
    {
        Capacity.AddModifier(modifier);
        Messenger.Broadcast(EventKey.UpdateResource, EResource.Energy);
    }
    public void RemoveAllModifier()
    {
        Capacity.ClearModifiers();
        Messenger.Broadcast(EventKey.UpdateResource, EResource.Energy);
    }
    public void RemoveAllModifier(object source)
    {
        Capacity.RemoveAllModifiersFromSource(source);
        Messenger.Broadcast(EventKey.UpdateResource, EResource.Energy);
    }

    public int GetCapacity()
    {
        return (int)Capacity.Value;
    }
}