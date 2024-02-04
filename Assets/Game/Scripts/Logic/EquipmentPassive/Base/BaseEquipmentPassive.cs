using Game.GameActor;
using Game.GameActor.Buff;
using UnityEngine;

public abstract class BaseEquipmentPassive : MonoBehaviour, IPassive
{
    protected ActorBase Caster;
    protected EquipableItem itemEquipment;
    protected EquipmentEntity equipmentEntity;
    public EEquipment EquipmentType => itemEquipment.EquipmentType;
    public ERarity Rarity => itemEquipment?.EquipmentRarity ?? ERarity.Common;
    public virtual void SetEquipment(EquipableItem itemEquipment)
    {
        this.itemEquipment = itemEquipment;
    }

    public virtual void Initialize(ActorBase actor)
    {
        Caster = actor;
    }

    public virtual void OnUpdate()
    {
    }

    public virtual void Remove()
    {
    }

    public virtual void Play()
    {
    }
}