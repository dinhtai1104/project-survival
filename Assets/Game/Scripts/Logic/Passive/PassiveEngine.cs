using Game.GameActor;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class PassiveEngine : MonoBehaviour, IPassiveEngine
{
    private ActorBase caster;
    [ShowInInspector]
    private List<IPassive> passives = new List<IPassive>();

    public void ApplyPassive(IPassive passive)
    {
        passives.Add(passive);
        passive.Initialize(caster);
        passive.Play();
    }

    public void Initialize(ActorBase actor)
    {
        this.caster = actor;
    }
    public void RemovePassives()
    {
        for (int i = passives.Count - 1; i >= 0; i--)
        {
            passives[i].Remove();
            passives.RemoveAt(i);
        }
    }

    public void Ticks()
    {
        for (int i = 0; i < passives.Count;i++)
        {
            if (passives[i] != null)
            {
                passives[i].OnUpdate();
            }
        }
    }
}