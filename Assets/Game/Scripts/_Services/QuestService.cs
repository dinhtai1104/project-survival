using Assets.Game.Scripts.BaseFramework.Architecture;
using Game.GameActor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Scripts._Services
{
    public class QuestService : Service
    {
        [SerializeField] private DailyQuestTable table;
        [SerializeField] private DailyQuestSaves save;
        public int Medal => save.Medal;

        public override void OnInit()
        {
            base.OnInit();
            table = DataManager.Base.DailyQuest;
            save = DataManager.Save.DailyQuest;

            AD.Controller.Instance.OnRewardEvent += Instance_OnRewardEvent;
        }

        public override void OnStart()
        {
            base.OnStart();
            IncreaseProgress(EMissionDaily.Login);
        }
        public override void OnDispose()
        {
            base.OnDispose();
            AD.Controller.Instance.OnRewardEvent -= Instance_OnRewardEvent;
        }

        private void Instance_OnRewardEvent(bool obj)
        {
            if (obj)
            {
                save.IncreaseProgress(EMissionDaily.WatchAds);
            }
        }

        public bool CanReceive(int Id)
        {
            return save.CanReceive(Id);
        }

        public bool CanReceiveMilestone(int Id)
        {
            return save.CanReceiveMilestone(Id);
        }
        public void ReceiveMilestone(int Id)
        {
            save.ReceiveMilestone(Id);
        }

        public bool IsReceiveMilestone(int Id)
        {
            return save.IsReceiveMilestone(Id);
        }

        public bool IsReceiveMission(int Id)
        {
            return save.IsReceiveMission(Id);
        }

        public void ReceiveMission(int Id)
        {
            save.ReceiveMission(Id);
        }

        public DailyQuestSave FindMission(EMissionDaily eMission)
        {
            return save.FindMission(eMission);
        }

        public void IncreaseProgress(EMissionDaily eMission)
        {
            save.IncreaseProgress(eMission);
        }

        public bool CanReceive()
        {
            return save.CanReceive();
        }

        public DailyQuestSave GetSave(int key)
        {
            return save.Saves[key];
        }
    }
}
