using System;
using UnityEngine;

public abstract class UIGeneralBaseIcon : MonoBehaviour
{
    protected UIInventorySlot parentSlot;
    public virtual Sprite Sprite => null;
    public UIInventorySlot ParentSlot { get => parentSlot; set => parentSlot = value; }

    public virtual void Clear() { }
    public virtual void OnUpdate() { }

    public void SetSlot(UIInventorySlot uIInventorySlot)
    {
        ParentSlot = uIInventorySlot;
    }
    public virtual void SetSizeImage(float size = 1)
    {
    }
    public abstract void SetData(ILootData lootData);
}