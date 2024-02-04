using System;

namespace Game.GameActor
{
    [Flags]
    public enum ECharacterType
    {
        None=0,
        Player=1,
        Enemy=1<<1,
        Object=1<<2,
        Drone=1<<3,
        Boss=1<<4,

    }
}