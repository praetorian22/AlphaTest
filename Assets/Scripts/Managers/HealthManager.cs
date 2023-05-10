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

    public void SetMaxHealth()
    {
        _health = _maxHealth;
        healthChangeEvent?.Invoke(_health);
    }

    public void DecHealth(Square square)
    {
        _health -= square.dataSquare.Damage;
        healthChangeEvent?.Invoke(_health);
        if (_health <= 0)
        {
            gameOverEvent?.Invoke();
        }
    }
}
