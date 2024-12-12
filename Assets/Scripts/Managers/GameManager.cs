using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class GameManager : MonoBehaviour
{
    private float _timeLevel;
    private int _ballTargetInLevel;

    private LevelManager _levelManager = new LevelManager();       
    private ScoreManager _scoreManager = new ScoreManager();
    private HealthManager _healthManager = new HealthManager();

    private string _alphaAll;
    private bool _langEng;

    private List<string> _listOfWordsRus = new List<string>();
    private List<string> _listOfWordsEng = new List<string>();    
    private Dictionary<int, List<string>> _dictCountAlphaLists = new Dictionary<int, List<string>>();    
    private Dictionary<string, GameObject> _dictAlphaPrefabs = new Dictionary<string, GameObject>();
    
    
    [SerializeField] private UIManager uiManager;
    [SerializeField] private int _sizePool;
    [SerializeField] private int _startHealth;
    [SerializeField] private List<Color32> _colors;
    [SerializeField] private Transform _parentForPool;
    [SerializeField] private Transform _parentForActiveObject;
    [SerializeField] private Square _prefabSquare;
    [SerializeField] private float _defaultTimeForRespawnSquade;
    [SerializeField] private List<string> _alphaForDictionary = new List<string>();
    [SerializeField] private List<GameObject> _prefabsForDictionary = new List<GameObject>();

    [SerializeField] private TextAsset fileRus;
    [SerializeField] private TextAsset fileEng;
    [SerializeField] private float timeErrorInputDefault;

    public SceneManager sceneManager;
    public ControllerSquare controllerSquare = new ControllerSquare();
    public SaveLoadManager saveLoadManager = new SaveLoadManager();

    private Coroutine _gameCoroutine;
    private Coroutine _inputCoroutine;
    private Coroutine _timerCoroutine;
    
    public IGameState State { get; set; }
    public IGameState OldState { get; set; }
    public Dictionary<Type, IGameState> stateMap;
    bool iniOK;

    public Action<int> trueAlphaInputEvent;
    public Action<float> changeTimeInLevelEvent;
        
    private void OnEnable()
    {
        uiManager.pressStartGameButtonEvent += () => ChangeState(GetState<StartGameState>());
        uiManager.pressMenuGameButtonEvent += () => ChangeState(GetState<PauseState>());
        uiManager.pressResumeGameButtonEvent += () => ChangeState(GetState<GameState>());
        uiManager.pressExitGameButtonEvent += () => ChangeState(GetState<ExitGameState>());
        uiManager.pressRestartGameButtonEvent += () => ChangeState(GetState<RestartGameState>());
        uiManager.pressBackToMapLevelButtonEvent += () => ChangeState(GetState<LevelsMapState>());
        uiManager.pressBackToMenuButtonEvent += () => ChangeState(GetState<MainMenuState>());
        uiManager.pressBeginButtonEvent += () => ChangeState(GetState<LevelsMapState>());
        uiManager.setLangEvent += ChangeLang;
        _healthManager.healthChangeEvent += uiManager.UpdateHealth;
        _scoreManager.changeScoreEvent += uiManager.UpdateScore;
        _scoreManager.changeRecordEvent += uiManager.UpdateRecord;
        trueAlphaInputEvent += _scoreManager.AddScore;
        _healthManager.gameOverEvent += GameOver;
        saveLoadManager.loadDataEvent += _scoreManager.LoadRecordData;           
        _levelManager.levelChangeEvent += uiManager.UpdateLevel;
        changeTimeInLevelEvent += uiManager.UpdateTimerInLevel;
        uiManager.pressLevelSelect += _levelManager.SelectLevel;
        uiManager.pressLevelSelect += (Level level) => ChangeState(GetState<SelectLevelState>());
        uiManager.needRecordInLevelEvent += _scoreManager.GetRecordInLevel;
    }
    private void OnDisable()
    {
        uiManager.pressStartGameButtonEvent -= () => ChangeState(GetState<StartGameState>());
        uiManager.pressMenuGameButtonEvent -= () => ChangeState(GetState<PauseState>());
        uiManager.pressResumeGameButtonEvent -= () => ChangeState(GetState<GameState>());
        uiManager.pressExitGameButtonEvent -= () => ChangeState(GetState<ExitGameState>());
        uiManager.pressRestartGameButtonEvent -= () => ChangeState(GetState<RestartGameState>());
        uiManager.pressBackToMapLevelButtonEvent -= () => ChangeState(GetState<LevelsMapState>());
        uiManager.pressBackToMenuButtonEvent -= () => ChangeState(GetState<MainMenuState>());
        uiManager.pressBeginButtonEvent -= () => ChangeState(GetState<LevelsMapState>());
        uiManager.setLangEvent -= ChangeLang;
        _healthManager.healthChangeEvent -= uiManager.UpdateHealth;
        _scoreManager.changeScoreEvent -= uiManager.UpdateScore;
        _scoreManager.changeRecordEvent -= uiManager.UpdateRecord;
        trueAlphaInputEvent -= _scoreManager.AddScore;
        _healthManager.gameOverEvent -= GameOver;
        saveLoadManager.loadDataEvent -= _scoreManager.LoadRecordData;        
        _levelManager.levelChangeEvent -= uiManager.UpdateLevel;
        changeTimeInLevelEvent -= uiManager.UpdateTimerInLevel;
        uiManager.pressLevelSelect -= _levelManager.SelectLevel;
        uiManager.pressLevelSelect -= (Level level) => ChangeState(GetState<SelectLevelState>());
        uiManager.needRecordInLevelEvent -= _scoreManager.GetRecordInLevel;
    }
    
    private void Start()
    {
        Initializing();
    }

    public void ChangeLang(bool langEng)
    {
        _langEng = langEng;

        for (int i = 0; i < 12; i++)
        {
            _dictCountAlphaLists[i].Clear();
        }

        if (_langEng)
        {
            _alphaAll = "abcdefghijklmnopqrstuvwxyz";
            
            foreach (string word in _listOfWordsEng)
            {
                if (word.Length > 0 && word.Substring(0, 1) != "-" && word.Length < 10) _dictCountAlphaLists[word.Length].Add(word.ToLower());
            }
        }
        else
        {
            _alphaAll = "àáâãäå¸æçèéêëìíîïðñòóôõö÷øùúûüýþÿ";
            
            foreach (string word in _listOfWordsRus)
            {
                if (word.Length > 0 && word.Substring(0, 1) != "-" && word.Length < 10) _dictCountAlphaLists[word.Length].Add(word.ToLower());
            }
        }
    }

    private List<Square> InstantiateSquareForPool()
    {
        List<Square> squares = new List<Square>();
        for (int i = 0; i < _sizePool; i++)
        {
            GameObject gameObject = Instantiate(_prefabSquare.gameObject, _parentForPool);
            gameObject.SetActive(false);
            Square square = gameObject.GetComponent<Square>();
            square.Init();
            squares.Add(square);
            square.damageEvent += _healthManager.DecHealth; // áåç îòïèñêè
        }
        return squares;
    }

    private void DecHealth(int value)
    {
        _healthManager.DecHealth(value);
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
        stateMap[typeof(LevelsMapState)] = new LevelsMapState();
        stateMap[typeof(SelectLevelState)] = new SelectLevelState();
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
        controllerSquare.Init(InstantiateSquareForPool(), _parentForPool);        
        saveLoadManager.LoadData();
    }
    private GameObject CreateAlphaDead(string alphaS, Color32 color)
    {
        if (_dictAlphaPrefabs.ContainsKey(alphaS))
        {
            GameObject alpha = Instantiate(_dictAlphaPrefabs[alphaS]);
            var renderers = alpha.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                if (renderer.gameObject.tag != "alphaMain") renderer.material.color = color;
            }
            return alpha;
        } 
        return null;
    }
    private void Awake()
    {
        var contentRus = fileRus.text;
        var allWordsRus = contentRus.Split("\n");
        _listOfWordsRus = new List<string>(allWordsRus);

        var contentEng = fileEng.text;
        var allWordsEng = contentEng.Split("\n");
        _listOfWordsEng = new List<string>(allWordsEng);

        for (int i = 0; i < 12; i++)
        {
            _dictCountAlphaLists.Add(i, new List<string>());
        }
        for (int i = 0; i < _alphaForDictionary.Count; i++)
        {
            _dictAlphaPrefabs.Add(_alphaForDictionary[i], _prefabsForDictionary[i]);
        }        
    }
    public void StartGame()
    {
        _scoreManager.Init(_levelManager.LevelSelected.Number);        
        _healthManager.SetHealth(_levelManager.LevelSelected.Errors);
        _timeLevel = _levelManager.LevelSelected.TimeLevel;
        changeTimeInLevelEvent?.Invoke(_timeLevel);
        _ballTargetInLevel = _levelManager.LevelSelected.Balls;
        uiManager.SetBallsInLevel(_levelManager.LevelSelected.Balls);
        if (_gameCoroutine != null) StopCoroutine(_gameCoroutine);
        if (_inputCoroutine != null) StopCoroutine(_inputCoroutine);
        if (_timerCoroutine != null) StopCoroutine(_timerCoroutine);
        _gameCoroutine = StartCoroutine(GameCoroutine());
        _inputCoroutine = StartCoroutine(InputControlCoroutine());  
        _timerCoroutine = StartCoroutine(TimerCoroutine());
    }
    private void OnDestroy()
    {
        saveLoadManager.SaveData(_scoreManager.Records);
    }

    public void GameOver()
    {
        ChangeState(GetState<GameOverState>());
        bool win;
        int ballNeed = _levelManager.LevelSelected.Balls;
        int ballPlayerRecord = _scoreManager.GetRecordInLevel(_levelManager.LevelSelected.Number);
        if (ballPlayerRecord >= ballNeed) win = true;
        else win = false;
        uiManager.EndPanelSet(win, _levelManager.LevelSelected);
        if (_gameCoroutine != null) StopCoroutine(_gameCoroutine);
        if (_inputCoroutine != null) StopCoroutine(_inputCoroutine);
        if (_timerCoroutine != null) StopCoroutine(_timerCoroutine);
        controllerSquare.ReturnToPoolAllSquare();
        saveLoadManager.SaveData(_scoreManager.Records);
    }
    private IEnumerator TimerCoroutine()
    {
        while(_timeLevel > 0)
        {
            yield return new WaitForSeconds(0.1f);
            _timeLevel -= 0.1f;
            changeTimeInLevelEvent?.Invoke(_timeLevel);
        }
        GameOver();
    }
    private IEnumerator GameCoroutine()
    {
        while (true)
        {
            if (State != GetState<PauseState>())
            {
                Vector2 positionRandomX = Vector2.zero;                
                Square square = controllerSquare.TakeNextSquareInPool(_prefabSquare.ID, new Vector3(positionRandomX.x, positionRandomX.y, _parentForActiveObject.position.z));
                square.gameObject.transform.parent = _parentForActiveObject;
                
                if (_levelManager.LevelSelected.MaxAlphaCount == 1)
                {
                    square.dataSquare.AddDataSquare(_alphaAll[UnityEngine.Random.Range(0, _alphaAll.Length)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                    controllerSquare.SetPosition(square, 1);
                }
                else
                {
                    if (_levelManager.LevelSelected.MaxAlphaCount == 2)
                    {
                        int r = UnityEngine.Random.Range(0, 2);
                        if (r == 0)
                        {
                            square.dataSquare.AddDataSquare(_dictCountAlphaLists[2][UnityEngine.Random.Range(0, _dictCountAlphaLists[2].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                            controllerSquare.SetPosition(square, 2);
                        }
                        else
                        {
                            square.dataSquare.AddDataSquare(_alphaAll[UnityEngine.Random.Range(0, _alphaAll.Length)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                            controllerSquare.SetPosition(square, 1);
                        }
                    }
                    else
                    {
                        if (_levelManager.LevelSelected.MaxAlphaCount == 3)
                        {
                            int r = UnityEngine.Random.Range(0, 3);
                            if (r == 0)
                            {
                                square.dataSquare.AddDataSquare(_dictCountAlphaLists[2][UnityEngine.Random.Range(0, _dictCountAlphaLists[2].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                controllerSquare.SetPosition(square, 2);
                            }
                            if (r == 1)
                            {
                                square.dataSquare.AddDataSquare(_alphaAll[UnityEngine.Random.Range(0, _alphaAll.Length)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                controllerSquare.SetPosition(square, 1);
                            }
                            if (r == 2)
                            {
                                square.dataSquare.AddDataSquare(_dictCountAlphaLists[3][UnityEngine.Random.Range(0, _dictCountAlphaLists[3].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                controllerSquare.SetPosition(square, 3);
                            }
                        }
                        else
                        {
                            if (_levelManager.LevelSelected.MaxAlphaCount == 4)
                            {
                                int r = UnityEngine.Random.Range(0, 4);
                                if (r == 0)
                                {
                                    square.dataSquare.AddDataSquare(_dictCountAlphaLists[2][UnityEngine.Random.Range(0, _dictCountAlphaLists[2].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                    controllerSquare.SetPosition(square, 2);
                                }
                                if (r == 1)
                                {
                                    square.dataSquare.AddDataSquare(_alphaAll[UnityEngine.Random.Range(0, _alphaAll.Length)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                    controllerSquare.SetPosition(square, 1);
                                }
                                if (r == 2)
                                {
                                    square.dataSquare.AddDataSquare(_dictCountAlphaLists[3][UnityEngine.Random.Range(0, _dictCountAlphaLists[3].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                    controllerSquare.SetPosition(square, 3);
                                }
                                if (r == 3)
                                {
                                    square.dataSquare.AddDataSquare(_dictCountAlphaLists[4][UnityEngine.Random.Range(0, _dictCountAlphaLists[4].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                    controllerSquare.SetPosition(square, 4);
                                }
                            }
                            else
                            {
                                if (_levelManager.LevelSelected.MaxAlphaCount == 5)
                                {
                                    int r = UnityEngine.Random.Range(0, 5);
                                    if (r == 0)
                                    {
                                        square.dataSquare.AddDataSquare(_dictCountAlphaLists[2][UnityEngine.Random.Range(0, _dictCountAlphaLists[2].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                        controllerSquare.SetPosition(square, 2);
                                    }
                                    if (r == 1)
                                    {
                                        square.dataSquare.AddDataSquare(_alphaAll[UnityEngine.Random.Range(0, _alphaAll.Length)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                        controllerSquare.SetPosition(square, 1);
                                    }
                                    if (r == 2)
                                    {
                                        square.dataSquare.AddDataSquare(_dictCountAlphaLists[3][UnityEngine.Random.Range(0, _dictCountAlphaLists[3].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                        controllerSquare.SetPosition(square, 3);
                                    }
                                    if (r == 3)
                                    {
                                        square.dataSquare.AddDataSquare(_dictCountAlphaLists[4][UnityEngine.Random.Range(0, _dictCountAlphaLists[4].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                        controllerSquare.SetPosition(square, 4);
                                    }
                                    if (r == 4)
                                    {
                                        square.dataSquare.AddDataSquare(_dictCountAlphaLists[5][UnityEngine.Random.Range(0, _dictCountAlphaLists[5].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                        controllerSquare.SetPosition(square, 5);
                                    }
                                }
                                else
                                {
                                    if (_levelManager.LevelSelected.MaxAlphaCount == 6)
                                    {
                                        int r = UnityEngine.Random.Range(0, 6);
                                        if (r == 0)
                                        {
                                            square.dataSquare.AddDataSquare(_dictCountAlphaLists[2][UnityEngine.Random.Range(0, _dictCountAlphaLists[2].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                            controllerSquare.SetPosition(square, 2);
                                        }
                                        if (r == 1)
                                        {
                                            square.dataSquare.AddDataSquare(_alphaAll[UnityEngine.Random.Range(0, _alphaAll.Length)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                            controllerSquare.SetPosition(square, 1);
                                        }
                                        if (r == 2)
                                        {
                                            square.dataSquare.AddDataSquare(_dictCountAlphaLists[3][UnityEngine.Random.Range(0, _dictCountAlphaLists[3].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                            controllerSquare.SetPosition(square, 3);
                                        }
                                        if (r == 3)
                                        {
                                            square.dataSquare.AddDataSquare(_dictCountAlphaLists[4][UnityEngine.Random.Range(0, _dictCountAlphaLists[4].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                            controllerSquare.SetPosition(square, 4);
                                        }
                                        if (r == 4)
                                        {
                                            square.dataSquare.AddDataSquare(_dictCountAlphaLists[5][UnityEngine.Random.Range(0, _dictCountAlphaLists[5].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                            controllerSquare.SetPosition(square, 5);
                                        }
                                        if (r == 5)
                                        {
                                            square.dataSquare.AddDataSquare(_dictCountAlphaLists[6][UnityEngine.Random.Range(0, _dictCountAlphaLists[6].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                            controllerSquare.SetPosition(square, 6);
                                        }
                                    }
                                    else
                                    {
                                        if (_levelManager.LevelSelected.MaxAlphaCount == 7)
                                        {
                                            int r = UnityEngine.Random.Range(0, 7);
                                            if (r == 0)
                                            {
                                                square.dataSquare.AddDataSquare(_dictCountAlphaLists[2][UnityEngine.Random.Range(0, _dictCountAlphaLists[2].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                controllerSquare.SetPosition(square, 2);
                                            }
                                            if (r == 1)
                                            {
                                                square.dataSquare.AddDataSquare(_alphaAll[UnityEngine.Random.Range(0, _alphaAll.Length)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                controllerSquare.SetPosition(square, 1);
                                            }
                                            if (r == 2)
                                            {
                                                square.dataSquare.AddDataSquare(_dictCountAlphaLists[3][UnityEngine.Random.Range(0, _dictCountAlphaLists[3].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                controllerSquare.SetPosition(square, 3);
                                            }
                                            if (r == 3)
                                            {
                                                square.dataSquare.AddDataSquare(_dictCountAlphaLists[4][UnityEngine.Random.Range(0, _dictCountAlphaLists[4].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                controllerSquare.SetPosition(square, 4);
                                            }
                                            if (r == 4)
                                            {
                                                square.dataSquare.AddDataSquare(_dictCountAlphaLists[5][UnityEngine.Random.Range(0, _dictCountAlphaLists[5].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                controllerSquare.SetPosition(square, 5);
                                            }
                                            if (r == 5)
                                            {
                                                square.dataSquare.AddDataSquare(_dictCountAlphaLists[6][UnityEngine.Random.Range(0, _dictCountAlphaLists[6].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                controllerSquare.SetPosition(square, 6);
                                            }
                                            if (r == 6)
                                            {
                                                square.dataSquare.AddDataSquare(_dictCountAlphaLists[7][UnityEngine.Random.Range(0, _dictCountAlphaLists[7].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                controllerSquare.SetPosition(square, 7);
                                            }
                                        }
                                        else
                                        {
                                            if (_levelManager.LevelSelected.MaxAlphaCount == 8)
                                            {
                                                int r = UnityEngine.Random.Range(0, 8);
                                                if (r == 0)
                                                {
                                                    square.dataSquare.AddDataSquare(_dictCountAlphaLists[2][UnityEngine.Random.Range(0, _dictCountAlphaLists[2].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                    controllerSquare.SetPosition(square, 2);
                                                }
                                                if (r == 1)
                                                {
                                                    square.dataSquare.AddDataSquare(_alphaAll[UnityEngine.Random.Range(0, _alphaAll.Length)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                    controllerSquare.SetPosition(square, 1);
                                                }
                                                if (r == 2)
                                                {
                                                    square.dataSquare.AddDataSquare(_dictCountAlphaLists[3][UnityEngine.Random.Range(0, _dictCountAlphaLists[3].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                    controllerSquare.SetPosition(square, 3);
                                                }
                                                if (r == 3)
                                                {
                                                    square.dataSquare.AddDataSquare(_dictCountAlphaLists[4][UnityEngine.Random.Range(0, _dictCountAlphaLists[4].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                    controllerSquare.SetPosition(square, 4);
                                                }
                                                if (r == 4)
                                                {
                                                    square.dataSquare.AddDataSquare(_dictCountAlphaLists[5][UnityEngine.Random.Range(0, _dictCountAlphaLists[5].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                    controllerSquare.SetPosition(square, 5);
                                                }
                                                if (r == 5)
                                                {
                                                    square.dataSquare.AddDataSquare(_dictCountAlphaLists[6][UnityEngine.Random.Range(0, _dictCountAlphaLists[6].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                    controllerSquare.SetPosition(square, 6);
                                                }
                                                if (r == 6)
                                                {
                                                    square.dataSquare.AddDataSquare(_dictCountAlphaLists[7][UnityEngine.Random.Range(0, _dictCountAlphaLists[7].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                    controllerSquare.SetPosition(square, 7);
                                                }
                                                if (r == 7)
                                                {
                                                    square.dataSquare.AddDataSquare(_dictCountAlphaLists[8][UnityEngine.Random.Range(0, _dictCountAlphaLists[8].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                    controllerSquare.SetPosition(square, 8);
                                                }
                                            }
                                            else
                                            {
                                                if (_levelManager.LevelSelected.MaxAlphaCount == 9)
                                                {
                                                    int r = UnityEngine.Random.Range(0, 9);
                                                    if (r == 0)
                                                    {
                                                        square.dataSquare.AddDataSquare(_dictCountAlphaLists[2][UnityEngine.Random.Range(0, _dictCountAlphaLists[2].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                        controllerSquare.SetPosition(square, 2);
                                                    }
                                                    if (r == 1)
                                                    {
                                                        square.dataSquare.AddDataSquare(_alphaAll[UnityEngine.Random.Range(0, _alphaAll.Length)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                        controllerSquare.SetPosition(square, 1);
                                                    }
                                                    if (r == 2)
                                                    {
                                                        square.dataSquare.AddDataSquare(_dictCountAlphaLists[3][UnityEngine.Random.Range(0, _dictCountAlphaLists[3].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                        controllerSquare.SetPosition(square, 3);
                                                    }
                                                    if (r == 3)
                                                    {
                                                        square.dataSquare.AddDataSquare(_dictCountAlphaLists[4][UnityEngine.Random.Range(0, _dictCountAlphaLists[4].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                        controllerSquare.SetPosition(square, 4);
                                                    }
                                                    if (r == 4)
                                                    {
                                                        square.dataSquare.AddDataSquare(_dictCountAlphaLists[5][UnityEngine.Random.Range(0, _dictCountAlphaLists[5].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                        controllerSquare.SetPosition(square, 5);
                                                    }
                                                    if (r == 5)
                                                    {
                                                        square.dataSquare.AddDataSquare(_dictCountAlphaLists[6][UnityEngine.Random.Range(0, _dictCountAlphaLists[6].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                        controllerSquare.SetPosition(square, 6);
                                                    }
                                                    if (r == 6)
                                                    {
                                                        square.dataSquare.AddDataSquare(_dictCountAlphaLists[7][UnityEngine.Random.Range(0, _dictCountAlphaLists[7].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                        controllerSquare.SetPosition(square, 7);
                                                    }
                                                    if (r == 7)
                                                    {
                                                        square.dataSquare.AddDataSquare(_dictCountAlphaLists[8][UnityEngine.Random.Range(0, _dictCountAlphaLists[8].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                        controllerSquare.SetPosition(square, 8);
                                                    }
                                                    if (r == 8)
                                                    {
                                                        square.dataSquare.AddDataSquare(_dictCountAlphaLists[9][UnityEngine.Random.Range(0, _dictCountAlphaLists[9].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                        controllerSquare.SetPosition(square, 9);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }

                }
                
                square.UpdateSpeed(_levelManager.LevelSelected.SpeedKoef);
                square.gameObject.SetActive(true);                
            }
            yield return new WaitForSeconds(_defaultTimeForRespawnSquade / _levelManager.LevelSelected.SpeedKoef);
        }
    }

    private IEnumerator InputControlCoroutine()
    {
        string lastAlpha = "";
        while (true)
        {
            if (State != GetState<PauseState>())
            {
                if (Input.anyKeyDown)
                {
                    Square square = null;
                    string alpha = Input.inputString.ToLower();
                    if (controllerSquare.CheckAlpha(alpha, out square))
                    {
                        lastAlpha = alpha;
                        if (square != null)
                        {
                            if (square.dataSquare.DeleteAlpha(alpha))
                            {
                                Vector3 position = square.DisableAlpha();
                                GameObject deadAlpha = CreateAlphaDead(alpha, square.dataSquare.Color);
                                if (deadAlpha != null) deadAlpha.transform.position = new Vector3 (position.x, position.y, position.z - 1f);
                                controllerSquare.ReturnToPool(square);
                                trueAlphaInputEvent?.Invoke(square.dataSquare.Balls);
                            }
                            else
                            {
                                Vector3 position = square.DisableAlpha();
                                GameObject deadAlpha = CreateAlphaDead(alpha, square.dataSquare.Color);
                                if (deadAlpha != null) deadAlpha.transform.position = new Vector3(position.x, position.y, position.z - 1f);
                                trueAlphaInputEvent?.Invoke(square.dataSquare.Balls);
                            }
                        }
                    }
                    else
                    {
                        if (lastAlpha != alpha)
                        {
                            DecHealth(1);
                            lastAlpha = alpha;
                        }                        
                    }
                                        
                }                              
            }
            yield return null;
        }
    }    
}
