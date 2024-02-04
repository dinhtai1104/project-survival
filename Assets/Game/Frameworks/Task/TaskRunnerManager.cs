using System.Collections.Generic;
using UnityEngine;

namespace Game.Tasks
{
    public class TaskRunnerManager : LiveSingleton<TaskRunnerManager>, IObservable<TaskRunner>
    {
        private List<TaskRunner> _allTaskRunners = new List<TaskRunner>();
        public void Subscribe(TaskRunner observer)
        {
            if (_allTaskRunners.Contains(observer)) return;
            _allTaskRunners.Add(observer);
        }

        public void Unsubscribe(TaskRunner observer)
        {
            if (!_allTaskRunners.Contains(observer)) return;
            _allTaskRunners.Remove(observer); 
        }

        private void Update()
        {
            for(int i = 0; i < _allTaskRunners.Count; i++)
            {
                var runner = _allTaskRunners[i];
                if (runner != null)
                {
                    runner.Notify();
                }
            }
        }
    }
}