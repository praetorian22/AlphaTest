using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HealthManager 
{
    private int _health;
    private int _maxHealth;
    public int Health => _health;

    public Action gameOverEvent;
    public Action<int> healthChangeEvent;

    public void Init(int value)
    {
        _maxHealth = value;        
    }

    public void SetHealth(int value)
    {
        _health = value;
        healthChangeEvent?.Invoke(_health);
    }

    public void DecHealth(int value)
    {
        _health -= value;
        healthChangeEvent?.Invoke(_health);
        if (_health <= 0)
        {
            gameOverEvent?.Invoke();
        }
    }
    /*
    public void DecHealth(Square square)
    {
        _health -= square.dataSquare.Damage;
        healthChangeEvent?.Invoke(_health);
        if (_health <= 0)
        {
            gameOverEvent?.Invoke();
        }
    }
    */
}
