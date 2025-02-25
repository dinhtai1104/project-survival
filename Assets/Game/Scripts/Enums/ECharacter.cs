using System;
using System.Collections.Generic;

namespace Assets.Game.Scripts.Enums
{
    public enum ECharacter
    { 
        Normal,
    }

    public static class CharacterExtension
    {
        public static ECharacter[] AllCharacter = (ECharacter[])Enum.GetValues(typeof(ECharacter));
    }
}
