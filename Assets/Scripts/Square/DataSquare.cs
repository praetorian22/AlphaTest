using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSquare
{
    private List<string> _alpha = new List<string>();
    private int _id;
    private int _damage;
    private Color32 _color;
    private bool _initOK;    
    private int _balls;    

    public List<string> Alpha => _alpha;
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

    public void AddDataSquare(char alpha, Color32 color)
    {
        _alpha.Add(alpha.ToString());
        _color = color;        
    }
    public void AddDataSquare(string str, Color32 color)
    {
        foreach (char c in str)
        {
            _alpha.Add(c.ToString());
        }        
        _color = color;
    }

    public bool DeleteAlpha(string alpha)
    {
        if (alpha != "" && _alpha.Count > 0 && _alpha[0] == alpha)
        {
            _alpha.RemoveAt(0);
        }
        if (_alpha.Count > 0) return false;
        else return true;
    }
}
