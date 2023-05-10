using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Square : MonoBehaviour
{
    [SerializeField] private int _id;
    [SerializeField] private int _damage;
    [SerializeField] private int _balls;
    [SerializeField] private SimulationSquare _simulationSquare;
    [SerializeField] private TMP_Text _charAlpha;


    public DataSquare dataSquare = new DataSquare();
    public Action<Square> damageEvent;

    private void Start()
    {
        dataSquare.Init(_id, _damage, _balls);
        _simulationSquare.StartMoveSquare();
    }

    private void OnEnable()
    {
        _charAlpha.text = dataSquare.Alpha;
        _charAlpha.color = dataSquare.Color;        
    }

    public void UpdateSpeed(int level)
    {
        _simulationSquare.speed = _simulationSquare.speed * level;
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
