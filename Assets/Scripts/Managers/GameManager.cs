using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    private LevelManager _levelManager = new LevelManager();       
    private ScoreManager _scoreManager = new ScoreManager();
    private HealthManager _healthManager = new HealthManager();

    private string _alphaAll;
    private bool _langEng;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private int _sizePool;
    [SerializeField] private int _startHealth;
    [SerializeField] private List<Color32> _colors;
    [SerializeField] private Transform _parentForPool;
    [SerializeField] private Transform _parentForActiveObject;
    [SerializeField] private Square _prefabSquare;
    [SerializeField] private float _defaultTimeForRespawnSquade;
    
    public SceneManager sceneManager;
    public ControllerSquare controllerSquare = new ControllerSquare();
    public SaveLoadManager saveLoadManager = new SaveLoadManager();

    private Coroutine _gameCoroutine;
    private Coroutine _inputCoroutine;

    public IGameState State { get; set; }
    public IGameState OldState { get; set; }
    public Dictionary<Type, IGameState> stateMap;
    bool iniOK;

    public Action<int> trueAlphaInputEvent;    

    private void OnEnable()
    {
        uiManager.pressStartGameButtonEvent += () => ChangeState(GetState<StartGameState>());
        uiManager.pressMenuGameButtonEvent += () => ChangeState(GetState<PauseState>());
        uiManager.pressResumeGameButtonEvent += () => ChangeState(GetState<GameState>());
        uiManager.pressExitGameButtonEvent += () => ChangeState(GetState<ExitGameState>());
        uiManager.pressRestartGameButtonEvent += () => ChangeState(GetState<RestartGameState>());
        uiManager.setLangEvent += ChangeLang;
        uiManager.setLevelEvent += (int level) => _levelManager.startLevel = level;
        _healthManager.healthChangeEvent += uiManager.UpdateHealth;
        _scoreManager.changeScoreEvent += uiManager.UpdateScore;
        _scoreManager.changeRecordEvent += uiManager.UpdateRecord;
        trueAlphaInputEvent += _scoreManager.AddScore;
        _healthManager.gameOverEvent += GameOver;
        saveLoadManager.loadDataEvent += _scoreManager.LoadRecordData;
    }
    private void OnDisable()
    {
        uiManager.pressStartGameButtonEvent -= () => ChangeState(GetState<StartGameState>());
        uiManager.pressMenuGameButtonEvent -= () => ChangeState(GetState<PauseState>());
        uiManager.pressResumeGameButtonEvent -= () => ChangeState(GetState<GameState>());
        uiManager.pressExitGameButtonEvent -= () => ChangeState(GetState<ExitGameState>());
        uiManager.pressRestartGameButtonEvent -= () => ChangeState(GetState<RestartGameState>());
        uiManager.setLangEvent -= ChangeLang;
        uiManager.setLevelEvent -= (int level) => _levelManager.startLevel = level;
        _healthManager.healthChangeEvent -= uiManager.UpdateHealth;
        _scoreManager.changeScoreEvent -= uiManager.UpdateScore;
        _scoreManager.changeRecordEvent -= uiManager.UpdateRecord;
        trueAlphaInputEvent -= _scoreManager.AddScore;
        _healthManager.gameOverEvent -= GameOver;
        saveLoadManager.loadDataEvent -= _scoreManager.LoadRecordData;
    }
    
    private void Start()
    {
        Initializing();
    }

    public void ChangeLang(bool langEng)
    {
        _langEng = langEng;
        if (_langEng) _alphaAll = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        else _alphaAll = "¿¡¬√ƒ≈®∆«»… ÀÃÕŒœ–—“”‘’÷◊ÿŸ⁄€‹›ﬁﬂ";
    }

    private List<Square> InstantiateSquareForPool()
    {
        List<Square> squares = new List<Square>();
        for (int i = 0; i < _sizePool; i++)
        {
            GameObject gameObject = Instantiate(_prefabSquare.gameObject, _parentForPool);
            gameObject.SetActive(false);
            Square square = gameObject.GetComponent<Square>();
            squares.Add(square);
            square.damageEvent += _healthManager.DecHealth; // ·ÂÁ ÓÚÔËÒÍË
        }
        return squares;
    }

    private void InitState()
    {
        stateMap = new Dictionary<Type, IGameState>();
        stateMap[typeof(MainMenuState)] = new MainMenuState();
        stateMap[typeof(GameState)] = new GameState();
        stateMap[typeof(StartGameState)] = new StartGameState();
        stateMap[typeof(PauseState)] = new PauseState();
        stateMap[typeof(GameOverState)] = new GameOverState();
        stateMap[typeof(ExitGameState)] = new ExitGameState();
        stateMap[typeof(RestartGameState)] = new RestartGameState();
    }
    public void ChangeState(IGameState newState)
    {
        if (State != null)
        {
            State.Exit(this);            
        }
        State = newState;        
        State.Enter(this);
    }
    public IGameState GetState<T>() where T : IGameState
    {
        var type = typeof(T);
        return stateMap[type];
    }

    public void Initializing()
    {
        if (!iniOK)
        {
            iniOK = true;
            InitState();
        }        
        ChangeState(GetState<MainMenuState>());
        _scoreManager.Init();
        controllerSquare.Init(InstantiateSquareForPool(), _parentForPool);
        saveLoadManager.LoadData();
        _healthManager.Init(_startHealth);
    }

    public void StartGame()
    {
        _levelManager.Init();
        _healthManager.SetMaxHealth();
        _gameCoroutine = StartCoroutine(GameCoroutine());
        _inputCoroutine = StartCoroutine(InputControlCoroutine());
    }
    private void OnDestroy()
    {
        saveLoadManager.SaveData(_scoreManager.Record);
    }

    public void GameOver()
    {
        ChangeState(GetState<GameOverState>());
        StopCoroutine(_gameCoroutine);
        StopCoroutine(_inputCoroutine);
        controllerSquare.ReturnToPoolAllSquare();
        saveLoadManager.SaveData(_scoreManager.Record);
    }

    private IEnumerator GameCoroutine()
    {
        while (true)
        {
            if (State != GetState<PauseState>())
            {
                Vector2 positionRandomX = Camera.main.ViewportToWorldPoint(new Vector2(UnityEngine.Random.Range(0.1f, 0.9f), 1));
                Square square = controllerSquare.TakeNextSquareInPool(_prefabSquare.dataSquare.ID, new Vector3(positionRandomX.x, positionRandomX.y, _parentForActiveObject.position.z));
                square.gameObject.transform.parent = _parentForActiveObject;                
                square.dataSquare.SetDataSquare(_alphaAll[UnityEngine.Random.Range(0, _alphaAll.Length)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                square.UpdateSpeed(_levelManager.Level);
                square.gameObject.SetActive(true);                
            }
            yield return new WaitForSeconds(_defaultTimeForRespawnSquade / _levelManager.Level);
        }
    }

    private IEnumerator InputControlCoroutine()
    {
        while (true)
        {
            if (State != GetState<PauseState>())
            {
                if (Input.anyKey)
                {
                    Square square = null;
                    if (controllerSquare.CheckAlpha(Input.inputString.ToUpper(), out square))
                    {
                        if (square != null)
                        {
                            controllerSquare.ReturnToPool(square);
                            trueAlphaInputEvent?.Invoke(square.dataSquare.Balls);
                        }
                    }
                }                              
            }
            yield return null;
        }
    }    
}
