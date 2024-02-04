using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ExpHandler
{
    public int MaxLevel = 100;
    [SerializeField] private long _totalExpAmount;
    [SerializeField] private List<long> expRequire = new List<long>();

    public ExpHandler(long totalExpAmount, List<long> expRequire)
    {
        MaxLevel = expRequire.Count;
        this.expRequire = expRequire;
        _totalExpAmount = totalExpAmount;
        _currentLevel = CalculateLevel(totalExpAmount);
        OnExpChanged?.Invoke(TotalExpAmount);
    }

    [SerializeField] private int _currentLevel = 1;

    public int NextLevel => Mathf.Clamp(_currentLevel + 1, 1, MaxLevel);

    public int CurrentLevel
    {
        get => _currentLevel;
        set
        {
            _currentLevel = value;
        }
    }

    /// <summary>
    /// Notify when level was changed. Params: fromLevel, toLevel
    /// </summary>
    public event Action<int, int> OnLevelChanged;
    public event Action<long> OnExpChanged;
    public long TotalExpAmount
    {
        set
        {
            _totalExpAmount = value;
            if (_totalExpAmount < 0) _totalExpAmount = 0;
        }
        get => _totalExpAmount;
    }
    public float LevelProgress
    {
        get
        {
            var currentLvlExp = LevelToExp(_currentLevel);
            var nextLvlExp = LevelToExp(NextLevel);
            float exp = TotalExpAmount - currentLvlExp;
            float total = nextLvlExp - currentLvlExp;
            float percentage = exp / total;
            return percentage;
        }
    }

    public long LevelToExp(int level)
    {
        if (level < 1 || level > MaxLevel) return 0;
        return expRequire[level - 1];
        //return DataManager.Base.ExpRequire.Dictionary[level];
    }

    public void Add(long amount, bool invokeEvent = true)
    {
        TotalExpAmount += amount;
        int newLevel = CalculateLevel(TotalExpAmount);
        OnExpChanged?.Invoke(TotalExpAmount);
        if (_currentLevel == newLevel) return;
        var lastLevel = _currentLevel;
        _currentLevel = newLevel;

        // Notify about all changes
        if (invokeEvent)
        {
            if (lastLevel != newLevel)
            {
                OnLevelChanged?.Invoke(lastLevel, newLevel);
            }
            //InvokeLevelChangeEvent(lastLevel, newLevel);
        }

    }

    public void Substract(long amount, bool invokeEvent = true)
    {
        TotalExpAmount -= amount;
        int newLevel = CalculateLevel(TotalExpAmount);
        OnExpChanged?.Invoke(TotalExpAmount);

        if (_currentLevel == newLevel) return;

        int _lastLevel = _currentLevel;
        _currentLevel = newLevel;
        // Notify about all changes
        if (invokeEvent)
        {
            InvokeLevelChangeEvent(_lastLevel, newLevel);
        }

    }

    private void InvokeLevelChangeEvent(int from, int to)
    {
        int current = from;

        if (to >= from)
        {
            for (int level = from + 1; level <= to; level++)
            {
                OnLevelChanged?.Invoke(current, level);
                current = level;
            }
        }
        else
        {
            for (int level = from - 1; level >= to; level--)
            {
                OnLevelChanged?.Invoke(current, level);
                current = level;
            }
        }
    }

    public int CalculateLevel(long currentExp)
    {
        if (CurrentLevel >= MaxLevel) return MaxLevel;
        //Debug.Log(currentExp);
        var currentLevel = 0;
        var expData = expRequire;
        for(int i = 0; i < expData.Count - 1; i++)
        {
            var levelData = expData[i];
            var nextLevelData = expData[i + 1];

            if (currentExp >= levelData && currentExp < nextLevelData)
            {
                currentLevel = i + 1;
                break;
            }
        }
        if (currentExp >= expData.Last()) currentLevel = MaxLevel;
        return Mathf.Min(currentLevel, MaxLevel);
    }
}