using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScoreManager
{
    private int _score;
    private int _record;

    public int Score => _score;
    public int Record => _record;

    public Action<int> changeScoreEvent;
    public Action<int> changeRecordEvent;
    public Action scoreToNextLevelEvent;

    public void Init()
    {
        _score = 0;
        _record = 0;
        changeScoreEvent?.Invoke(_score);
        changeRecordEvent?.Invoke(_record);
    }

    public void LoadRecordData(int value)
    {
        _record = value;
        changeRecordEvent?.Invoke(_record);
    }

    public void AddScore(int value)
    {
        _score += value;
        changeScoreEvent?.Invoke(_score);

        if (_record < _score)
        {
            _record = _score;
            changeRecordEvent?.Invoke(_record);
        }
    }
}
