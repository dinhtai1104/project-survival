using Assets.Game.Scripts.Enums;
using Engine;
using Framework;
using Manager;
using SceneManger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gameplay
{
    public class PlayerGameplayData
    {
        public PlayerData PlayerData;
        public ECharacter Character => PlayerData.CurrentCharacter;
        public OutfitData Outfit;


        public static PlayerGameplayData CreateTest()
        {
            var data = new PlayerGameplayData();
            data.PlayerData = (BaseSceneManager.Instance as GameSceneManager).PlayerData;
            data.Outfit = new OutfitData(0, data.PlayerData.PlayerStats);

            var outfit = data.Outfit;

            var newEquipment = new EquipableItem();
            return data;
        }
    }
}
