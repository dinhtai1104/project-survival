using Assets.Game.Scripts.BaseFramework.Architecture;
using Assets.Game.Scripts.PiggyBank.Data;
using System;
using UnityEngine;

namespace Assets.Game.Scripts._Services
{
    [Serializable]
    public class PiggyBankServices : Service
    {
        public delegate void PiggyBankClaim(EPiggyBank type);
        public delegate void PiggyBankActive(int piggyBank);
        public event PiggyBankClaim OnPiggyBankClaimEvent;
        public event PiggyBankActive OnPiggyBankActiveEvent;

        [SerializeField] private PiggyBanksSave save;
        [SerializeField] private PiggyBankTable table;

        public PiggyBanksSave Save { get => save; set => save = value; }
        public PiggyBankTable Table { get => table; set => table = value; }

        public int PiggyCurrent => Save.PiggyCurrent;
        public DateTime TimeEnd => Save.Saves[PiggyCurrent].TimeEnd;
        public override void OnInit()
        {
            base.OnInit();
            Save = DataManager.Save.PiggyBank;
            Table = DataManager.Base.PiggyBank;
        }
        public void Active(int Id, bool IsReset = false, bool init = false)
        {
            OnPiggyBankActiveEvent?.Invoke(Id);
            Save.Active(Id, IsReset, init);   
        }

        public void ClaimPiggy(EPiggyBank type)
        {
            OnPiggyBankClaimEvent?.Invoke(type);
            Save.ClaimPiggy(type);
            switch (type)
            {
                case EPiggyBank.FREE:
                    break;
                case EPiggyBank.AD:
                    break;
                case EPiggyBank.PURCHASE:
                    break;
            }
        }
        public bool IsClaim(int id, EPiggyBank type)
        {
            return Save.Saves[id].IsClaim(type);
        }
        public bool IsClaimCurrent(EPiggyBank type)
        {
            return IsClaim(PiggyCurrent, type);
        }

        public bool CanClaim()
        {
            return Save.CanClaim();
        }

        public PiggyBankEntity GetTable(int piggyId)
        {
            return table.Get(piggyId);
        }
    }
}
