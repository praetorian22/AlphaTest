using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelManager
{
    private int _level;
    public int startLevel;

    public int Level => _level;

    public Action<int> levelChangeEvent;

    public void Init()
    {
        _level = startLevel;
        levelChangeEvent.Invoke(_level);
    }

    public void LevelUP()
    {
        _level++;
        levelChangeEvent.Invoke(_level);
    }
}
