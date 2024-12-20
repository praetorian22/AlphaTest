using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button _begin;
    [SerializeField] private Button _lang;
    [SerializeField] private Button _levelChange;
    [SerializeField] private Button _menu;
    [SerializeField] private Button _resume;
    [SerializeField] private Button _restart;
    [SerializeField] private Button _exit;
    [SerializeField] private Button _restart2;
    [SerializeField] private Button _exit2;
    [SerializeField] private Button _closeProgram;
    [SerializeField] private Button _start;
    [SerializeField] private Button _goBackLevel;
    [SerializeField] private Button _goBackLevelMenu;
    [SerializeField] private List<Button> _selectLevelButtons = new List<Button>();

    [SerializeField] private TMP_Text _health;
    [SerializeField] private TMP_Text _score;
    [SerializeField] private TMP_Text _record;
    [SerializeField] private TMP_Text _level;
    [SerializeField] private TMP_Text _timeLevelInLevel;
    [SerializeField] private TMP_Text _ballsInLevel;

    [SerializeField] private TMP_Text _errorsLevel;
    [SerializeField] private TMP_Text _speedLevel;
    [SerializeField] private TMP_Text _ballsLevel;
    [SerializeField] private TMP_Text _maxAlphaLevel;
    [SerializeField] private TMP_Text _timeLevel;
    [SerializeField] private TMP_Text _recordPlayerLevel;

    [SerializeField] private TMP_Text _endPanelBallsNeed;
    [SerializeField] private TMP_Text _endPanelBallsRecord;
    [SerializeField] private TMP_Text _congratulation;

    private bool _langSetter;

    public Action pressStartGameButtonEvent;
    public Action pressMenuGameButtonEvent;
    public Action pressExitGameButtonEvent;
    public Action pressResumeGameButtonEvent;
    public Action pressRestartGameButtonEvent;
    public Action pressBeginButtonEvent;
    public Action pressBackToMapLevelButtonEvent;
    public Action pressBackToMenuButtonEvent;
    public Action<bool> setLangEvent;
    public Action<Level> pressLevelSelect;
    public Func<int,int> needRecordInLevelEvent;

    private void Start()
    {
        _langSetter = true;
        ChangeLang();
        _begin.onClick.AddListener(() => {
            pressBeginButtonEvent?.Invoke();
            ColorLevelMap();
        });
        _goBackLevel.onClick.AddListener(() => pressBackToMapLevelButtonEvent?.Invoke());
        _goBackLevelMenu.onClick.AddListener(() => pressBackToMenuButtonEvent?.Invoke());
        _start.onClick.AddListener(() => pressStartGameButtonEvent?.Invoke());
        _lang.onClick.AddListener(() => ChangeLang());
        _menu.onClick.AddListener(() => pressMenuGameButtonEvent?.Invoke());
        _resume.onClick.AddListener(() => pressResumeGameButtonEvent?.Invoke());
        _restart.onClick.AddListener(() => pressRestartGameButtonEvent?.Invoke());
        _restart2.onClick.AddListener(() => pressRestartGameButtonEvent?.Invoke());
        _exit.onClick.AddListener(() => pressExitGameButtonEvent?.Invoke());
        _exit2.onClick.AddListener(() => pressExitGameButtonEvent?.Invoke());
        _closeProgram.onClick.AddListener(() => Application.Quit());

        foreach (Button button in _selectLevelButtons)
        {
            Level level = button.GetComponent<Level>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                _errorsLevel.text = level.Errors.ToString();
                _speedLevel.text = level.SpeedKoef.ToString();
                _ballsLevel.text = level.Balls.ToString();
                _maxAlphaLevel.text = level.MaxAlphaCount.ToString();
                _timeLevel.text = level.TimeLevel.ToString();
                int record = needRecordInLevelEvent.Invoke(level.Number);
                _recordPlayerLevel.text = "(" + record.ToString() + ")";                
                pressLevelSelect?.Invoke(level);
            });
        }
    }
    
    public void ColorLevelMap()
    {
        foreach (Button button in _selectLevelButtons)
        {
            int level = button.gameObject.GetComponent<Level>().Number;
            int record = needRecordInLevelEvent.Invoke(level);
            int ballNeed = button.gameObject.GetComponent<Level>().Balls;
            if (record >= ballNeed)
                button.gameObject.GetComponent<Image>().color = new Color32(0, 255, 150, 255);
            else
                button.gameObject.GetComponent<Image>().color = Color.white;
        }
    }

    private void ChangeLang()
    {
        _langSetter = !_langSetter;
        if (_langSetter) _lang.GetComponentInChildren<TMP_Text>().text = "����������";
        else _lang.GetComponentInChildren<TMP_Text>().text = "�������";
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
    public void UpdateTimerInLevel(float value)
    {
        _timeLevelInLevel.text = value.ToString("0.0");
    }
    public void SetBallsInLevel(int value)
    {
        _ballsInLevel.text = value.ToString();
    }
    public void EndPanelSet(bool win, Level level)
    {
        int record = needRecordInLevelEvent.Invoke(level.Number);
        _endPanelBallsRecord.text = "(" + record.ToString() + ")";
        _endPanelBallsNeed.text = level.Balls.ToString();
        if (win) _congratulation.gameObject.SetActive(true);
        else _congratulation.gameObject.SetActive(false);
    }
}
