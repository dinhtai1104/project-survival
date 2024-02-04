using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tasks
{
    public class TaskRunner : MonoBehaviour, IObserver
    {
        [SerializeField] private bool _autoStart;

        [SerializeField] private Task _startingTask;

        [SerializeField] private Task _currentTask;

        [SerializeField] private bool _isRunning;
        private IStopHandler[] _stopHandlers;

        private Task[] _tasks;

        public bool IsRunning
        {
            get { return _isRunning; }
        }

        public Task CurrentTask
        {
            get { return _currentTask; }
        }

        public Task[] Tasks
        {
            get { return _tasks; }
        }

        public Task StartingTask { get => _startingTask; set => _startingTask = value; }

        public delegate void CompleteDelegate();

        public CompleteDelegate OnComplete;

        private void OnEnable()
        {
            if (_tasks != null)
            {
                foreach (var task in _tasks)
                {
                    task.UnityEnable();
                }
            }

            if (_autoStart && _startingTask != null)
                RunTask();
        }

        private void OnDisable()
        {
            if (_isRunning && gameObject != null)
                StopTask();

            if (_tasks != null)
            {
                foreach (var task in _tasks)
                {
                    task.UnityDisable();
                }
            }
        }

        public void Awake()
        {
            _tasks = GetComponentsInChildren<Task>();
            _stopHandlers = GetComponentsInChildren<IStopHandler>();
            if (_tasks.Length > 0)
            {
                foreach (var task in _tasks)
                {
                    task.SetupTaskRelationship();
                    task.UnityAwake();
                }

                _startingTask = _tasks[0];
            }
        }

        private void Start()
        {
            if (_tasks != null)
            {
                foreach (var task in _tasks)
                {
                    task.UnityStart();
                }
            }
        }

        public void RunTask()
        {
            if (_isRunning || _startingTask == null)
                return;

            foreach (var task in _tasks)
            {
                task.IsCompleted = false;
                task.IsRunning = false;
            }

            _currentTask = _startingTask;
            _isRunning = true;

            foreach (var task in _currentTask.ParallelTasks)
            {
                if (task.IsIgnore) continue;
                task.Begin();
            }

            TaskRunnerManager.Instance.Subscribe(this);
        }

        public void StopTask()
        {
            if (!_isRunning)
                return;

            _isRunning = false;

            _currentTask = null;
            foreach (var task in _tasks)
            {
                if (task.IsRunning)
                    task.End();
                task.IsCompleted = false;
                task.IsRunning = false;
                task.ForceInterruptTask = false;
            }
            if (_stopHandlers != null)
            {
                foreach (var stop in _stopHandlers)
                {
                    stop.OnStop();
                }
            }
            OnComplete?.Invoke();

            if (TaskRunnerManager.Instance != null)
                TaskRunnerManager.Instance.Unsubscribe(this);
        }

        public void Notify()
        {
            if (_isRunning)
            {
                // Current task complete
                if (_currentTask.IsCompleted)
                {
                    // Then stop parallel tasks
                    for (int i = 0; i < _currentTask.ParallelTasks.Length; ++i)
                    {
                        var task = _currentTask.ParallelTasks[i];
                        if (task.IsIgnore) continue;
                        task.End();
                    }

                    // Prevent current task being null when gameobject was destroyed in End method
                    if (_currentTask == null)
                    {
                        StopTask();
                        return;
                    }

                    // If there is next task then run it
                    if (_currentTask.NextTask != null)
                    {
                        _currentTask = _currentTask.NextTask;


                        foreach (var task in _currentTask.ParallelTasks)
                        {
                            task.Begin();
                        }
                    }
                    // If there is no next task then stop
                    else
                    {
                        StopTask();
                    }
                }
                else
                {
                    if (_currentTask != null)
                    {
                        foreach (var task in _currentTask.ParallelTasks)
                        {
                            if (task.IsIgnore) continue;
                            task.Run();
                            if (task.ForceInterruptTask)
                            {
                                StopTask();
                            }
                        }
                    }
                }
            }
        }
    }
}