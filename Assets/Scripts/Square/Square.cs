using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Square : MonoBehaviour
{
    private List<GameObject> _listTextAlpha = new List<GameObject>();
    private int _indexAlpha;
    [SerializeField] private int _id;
    [SerializeField] private int _damage;
    [SerializeField] private int _balls;
    [SerializeField] private SimulationSquare _simulationSquare;
    [SerializeField] private GameObject _charAlphaPanel;
    [SerializeField] private GameObject _textAlphaPrefab;
    [SerializeField] private Color32 _colorDisable;


    public DataSquare dataSquare = new DataSquare();
    public Action<Square> damageEvent;

    private void Start()
    {
        dataSquare.Init(_id, _damage, _balls);
        _simulationSquare.StartMoveSquare();
    }

    private void OnEnable()
    {
        _indexAlpha = 0;
        foreach (string s in dataSquare.Alpha)
        {
            GameObject textAlpha = Instantiate(_textAlphaPrefab, _charAlphaPanel.transform);
            textAlpha.GetComponent<TMP_Text>().text = s;
            textAlpha.GetComponent<TMP_Text>().color = dataSquare.Color;
            _listTextAlpha.Add(textAlpha);
        }               
    }
    
    public void DisableAlpha()
    {
        if (_listTextAlpha.Count > 0)
        {
            _listTextAlpha[_indexAlpha].GetComponent<TMP_Text>().color = _colorDisable;
            _indexAlpha++;
        }
    }

    public void UpdateSpeed(int level)
    {
        int koef = Mathf.CeilToInt(level / 10);
        if (koef > 0) _simulationSquare.speed = _simulationSquare.speed * koef;
        dataSquare.tempSpeed = _simulationSquare.speed;
    }

    public void StopMove()
    {
        _simulationSquare.speed = Vector2.zero;
    }

    public void ResumeMove()
    {
        _simulationSquare.speed = dataSquare.tempSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        damageEvent?.Invoke(this);
    }
}
