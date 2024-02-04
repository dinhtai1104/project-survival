using BansheeGz.BGDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Game.Scripts.PiggyBank.Data
{
    [System.Serializable]
    public class PiggyBankEntity
    {
        public int Id;
        public int Target;
        private EResource Type;
        private int Value;
        private int ValueAd;
        private int ValuePurchase;
        private int TimeValue;
        public string ProductId;

        public ResourceData BaseValue;
        public ResourceData AdValue;
        public ResourceData PurchaseValue;
        public TimeSpan TimeReset;

        public PiggyBankEntity(BGEntity e)
        {
            Id = e.Get<int>("Id");
            Target = e.Get<int>("Target");
            Enum.TryParse(e.Get<string>("Type"), out Type);
            Value = e.Get<int>("Value");
            ValueAd = e.Get<int>("ValueAd");
            ValuePurchase = e.Get<int>("ValuePurchase");
            TimeValue = e.Get<int>("TimeReset");
            ProductId = e.Get<string>("ProductId");

            BaseValue = new ResourceData(Type, Value);
            AdValue = new ResourceData(Type, ValueAd);
            PurchaseValue = new ResourceData(Type, ValuePurchase);

            TimeReset = TimeSpan.FromSeconds(TimeValue);
        }
    }
}
