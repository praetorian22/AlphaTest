using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private int _errors;
    [SerializeField] private float _speedKoef;
    [SerializeField] private int _balls;
    [SerializeField] private int _maxAlphaCount;
    [SerializeField] private float _timeLevel;

    public int Errors => _errors;
    public float TimeLevel => _timeLevel;
    public int Balls => _balls;
    public int MaxAlphaCount => _maxAlphaCount;
    public float SpeedKoef => _speedKoef;
}
