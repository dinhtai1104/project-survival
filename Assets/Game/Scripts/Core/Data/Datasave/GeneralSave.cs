using Foundation.Game.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts.Core.Data.Datasave
{
    [System.Serializable]
    public class GeneralSave : BaseDatasave
    {
        public bool IsOnSFX = true;
        public bool IsOnMusic = true;
        public bool IsOnVibrate = true;
        public bool IsHDR = true;
        public DateTime LastTimeOut;
        public int PlaySession = 0;
        public bool IsGameTutFinished = false;
        public double TimeOnline = 0;
        public DateTime DateFirstTime;

        public GeneralSave()
        {
        }

        public GeneralSave(string key) : base(key)
        {
            DateFirstTime = LastTimeOut = UnbiasedTime.UtcNow;
            IsHDR = PlayerPrefs.GetInt("HDR", 0) == 1;
        }

        public override void Fix()
        {
        }

        public void SetLastTimeOut()
        {
            LastTimeOut = UnbiasedTime.UtcNow;
            Save();
        }
    }
}
