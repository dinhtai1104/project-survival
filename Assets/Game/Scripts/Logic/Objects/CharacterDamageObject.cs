using UnityEngine;

[RequireComponent(typeof(InvokeHitByTrigger))]
public class CharacterDamageObject : CharacterObjectBase, IDamage
{
    public virtual Stat DmgStat { get; set;  }
    public InvokeHitByTrigger _hit;

    protected virtual void OnValidate()
    {
        _hit = GetComponent<InvokeHitByTrigger>();
    }
}