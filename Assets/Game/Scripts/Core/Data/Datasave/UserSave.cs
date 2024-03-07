using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.Core.Data.Datasave
{
    [System.Serializable]
    public class UserSave : BaseDatasave
    {
        public string Id;
        public string PlayerName;
        public long Experience;

        public int SessionGame = 0;
        public double OnlineTime; // Second;

        public int IAP_Count = 0;
        public int IAA_Count = 0;

        public List<IAPSave> IAPBought = new List<IAPSave>();
        public UserSave(string key) : base(key)
        {
            PlayerName = "Player" + UnityEngine.Random.Range(1, 9999999);
            OnlineTime = 0;
        }

        public UserSave()
        {

        }

        public void BuyIAP(string product)
        {
            IAPBought.Add(IAPSave.Buy(product));
            IAP_Count++;
            Save();
        }
        public void WatchIAA()
        {
            IAA_Count++;
            Save();
        }

        public void SetId(string id)
        {
            this.Id = id;
            Save();
        }

        public void PlaySession()
        {
            SessionGame++;
            Save();
        }

        public override void Fix()
        {
        }
    }
}
