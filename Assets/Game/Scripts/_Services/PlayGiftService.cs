using Assets.Game.Scripts.BaseFramework.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts._Services
{
    [Serializable]
    public class PlayGiftService : Service
    {
        public delegate void PlayGiftServiceClaim(int Index);
        public event PlayGiftServiceClaim OnPlayGiftServiceClaim;

        [SerializeField] private PlayGiftTable table;
        [SerializeField] private PlayGiftSaves save;

        public bool IsClaimedPlay(int Session)
        {
            return save.IsClaimedPlay(Session);
        }
        public bool CanClaimedPlay(int Session)
        {
            return save.CanClaimedPlay(Session);
        }

        public void Claim(int INDEX)
        {
            save.Claim(INDEX);
            OnPlayGiftServiceClaim?.Invoke(INDEX);
        }

        public void Purchase()
        {
            save.Purchase();
        }
    }
}
