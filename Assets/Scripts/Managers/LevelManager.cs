using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelManager
{
    private Level _levelSelected;
    [SerializeField] private List<Level> _levels = new List<Level>();

    public Level LevelSelected => _levelSelected;

    public Action<int> levelChangeEvent;

    public void SelectLevel(Level level)
    {
        _levelSelected = level;
    }    
}
