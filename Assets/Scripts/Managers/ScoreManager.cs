using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScoreManager
{
    private int _score;
    private List<int> _records;
    private int _recordInLevel;
    private int _numberLevel;

    public int Score => _score;
    public List<int> Records => _records;

    public Action<int> changeScoreEvent;
    public Action<int> changeRecordEvent;
    public Action scoreToNextLevelEvent;

    public void Init(int numberLevel)
    {
        _score = 0;
        _numberLevel = numberLevel;
        _recordInLevel = _records[numberLevel];
        changeScoreEvent?.Invoke(_score);
        changeRecordEvent?.Invoke(_recordInLevel);
    }

    public void LoadRecordData(List<int> values)
    {
        _records = new List<int>(values);
    }

    public void AddScore(int value)
    {
        _score += value;
        changeScoreEvent?.Invoke(_score);

        if (_recordInLevel < _score)
        {
            _recordInLevel = _score;
            _records[_numberLevel] = _recordInLevel;
            changeRecordEvent?.Invoke(_recordInLevel);
        }
    }
}
