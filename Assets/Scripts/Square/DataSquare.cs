using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSquare
{
    private string _alpha;
    private int _id;
    private int _damage;
    private Color32 _color;
    private bool _initOK;    
    private int _balls;

    public string Alpha => _alpha;
    public int ID => _id;
    public int Damage => _damage;    
    public Color32 Color => _color;
    public Vector2 tempSpeed;
    public int Balls => _balls;

    public void Init (int id, int damage, int balls)
    {
        if (!_initOK)
        {
            _id = id;
            _damage = damage;
            _balls = balls;
            _initOK = true;
        }        
    }

    public void SetDataSquare(char alpha, Color32 color)
    {
        _alpha = alpha.ToString();
        _color = color;        
    }
}
