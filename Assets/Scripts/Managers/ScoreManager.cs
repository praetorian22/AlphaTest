using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScoreManager
{
    private int _score;
    private int _scoreNextLevel;
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
        _scoreNextLevel = 500;
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

        if (_score > _scoreNextLevel)
        {
            scoreToNextLevelEvent?.Invoke();
            if (_scoreNextLevel < 5000) _scoreNextLevel += 500;
            if (_scoreNextLevel < 20000) _scoreNextLevel += 1000;
            if (_scoreNextLevel < 50000) _scoreNextLevel += 5000;
        }

        if (_record < _score)
        {
            _record = _score;
            changeRecordEvent?.Invoke(_record);
        }
    }
}
