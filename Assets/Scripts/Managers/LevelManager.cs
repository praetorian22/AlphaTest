using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager
{
    private int _level;
    public int startLevel;

    public int Level => _level;

    public void Init()
    {
        _level = startLevel;
    }
}
