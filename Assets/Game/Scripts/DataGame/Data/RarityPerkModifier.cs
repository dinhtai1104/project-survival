﻿using Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Gameplay.Data
{
    [System.Serializable]
    public class RarityPerkModifier
    {
        public ERarityPerk PerkType;
        public ModifierData Modifier;

        public RarityPerkModifier()
        {
        }

        public RarityPerkModifier(ERarityPerk perkType, ModifierData modifier)
        {
            PerkType = perkType;
            Modifier = modifier;
        }
    }
}
