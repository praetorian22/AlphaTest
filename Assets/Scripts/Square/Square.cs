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
    public int ID => _id;
    public void Init()
    {
        dataSquare.Init(_id, _damage, _balls);        
    }
    
    private void OnEnable()
    {
        _indexAlpha = 0;
        foreach (var item in _listTextAlpha)
        {
            Destroy(item.gameObject);
        }
        _listTextAlpha.Clear();
        foreach (string s in dataSquare.Alpha)
        {
            GameObject textAlpha = Instantiate(_textAlphaPrefab, _charAlphaPanel.transform);
            textAlpha.GetComponent<TMP_Text>().text = s;
            textAlpha.GetComponent<TMP_Text>().color = dataSquare.Color;
            _listTextAlpha.Add(textAlpha);
        }
        _simulationSquare.StartMoveSquare();
    }
    
    public Vector3 DisableAlpha()
    {
        if (_listTextAlpha.Count > 0)
        {
            GameObject text = _listTextAlpha[_indexAlpha];
            text.GetComponent<TMP_Text>().color = _colorDisable;
            _indexAlpha++;
            return text.transform.position;
        }
        return Vector3.zero;
    }

    public void UpdateSpeed(float koef)
    {
        if (koef > 0) _simulationSquare.speed = _simulationSquare.SpeedDef * koef;
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
