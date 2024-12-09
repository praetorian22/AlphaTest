using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button _start;
    [SerializeField] private Button _lang;
    [SerializeField] private Button _levelChange;
    [SerializeField] private Button _menu;
    [SerializeField] private Button _resume;
    [SerializeField] private Button _restart;
    [SerializeField] private Button _exit;
    [SerializeField] private Button _restart2;
    [SerializeField] private Button _exit2;
    [SerializeField] private Button _closeProgram;

    [SerializeField] private TMP_Text _health;
    [SerializeField] private TMP_Text _score;
    [SerializeField] private TMP_Text _record;
    [SerializeField] private TMP_Text _level;
    private int _levelSetter;
    private bool _langSetter;

    public Action pressStartGameButtonEvent;
    public Action pressMenuGameButtonEvent;
    public Action pressExitGameButtonEvent;
    public Action pressResumeGameButtonEvent;
    public Action pressRestartGameButtonEvent;
    public Action<int> setLevelEvent;
    public Action<bool> setLangEvent;

    private void Start()
    {
        _levelSetter = 1;
        _langSetter = true;
        ChangeLevel();
        ChangeLang();
        _start.onClick.AddListener(() => pressStartGameButtonEvent?.Invoke());
        _levelChange.onClick.AddListener(() => ChangeLevel());
        _lang.onClick.AddListener(() => ChangeLang());
        _menu.onClick.AddListener(() => pressMenuGameButtonEvent?.Invoke());
        _resume.onClick.AddListener(() => pressResumeGameButtonEvent?.Invoke());
        _restart.onClick.AddListener(() => pressRestartGameButtonEvent?.Invoke());
        _restart2.onClick.AddListener(() => pressRestartGameButtonEvent?.Invoke());
        _exit.onClick.AddListener(() => pressExitGameButtonEvent?.Invoke());
        _exit2.onClick.AddListener(() => pressExitGameButtonEvent?.Invoke());
        _closeProgram.onClick.AddListener(() => Application.Quit());
    }

    private void ChangeLevel()
    {
        _levelSetter += 10;
        if (_levelSetter > 21) _levelSetter = 1;
        if (_levelSetter == 1) _levelChange.GetComponentInChildren<TMP_Text>().text = "À≈ √Œ";
        if (_levelSetter == 11) _levelChange.GetComponentInChildren<TMP_Text>().text = "ÕŒ–Ã¿";
        if (_levelSetter == 21) _levelChange.GetComponentInChildren<TMP_Text>().text = "—ÀŒ∆ÕŒ";
        setLevelEvent?.Invoke(_levelSetter);
    }

    private void ChangeLang()
    {
        _langSetter = !_langSetter;
        if (_langSetter) _lang.GetComponentInChildren<TMP_Text>().text = "¿Õ√À»…— »…";
        else _lang.GetComponentInChildren<TMP_Text>().text = "–”—— »…";
        setLangEvent?.Invoke(_langSetter);
    }

    public void UpdateHealth(int value)
    {
        _health.text = value.ToString();
    }
    public void UpdateScore(int value)
    {
        _score.text = value.ToString();
    }
    public void UpdateRecord(int value)
    {
        _record.text = value.ToString();
    }
    public void UpdateLevel(int value)
    {
        _level.text = value.ToString();
    }
}
