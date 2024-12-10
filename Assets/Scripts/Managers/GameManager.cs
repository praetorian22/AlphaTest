using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class GameManager : MonoBehaviour
{
    private LevelManager _levelManager = new LevelManager();       
    private ScoreManager _scoreManager = new ScoreManager();
    private HealthManager _healthManager = new HealthManager();

    private string _alphaAll;
    private bool _langEng;

    private List<string> _listOfWordsRus = new List<string>();
    private List<string> _listOfWordsEng = new List<string>();    
    private Dictionary<int, List<string>> _dictCountAlphaLists = new Dictionary<int, List<string>>();    
    private Dictionary<string, GameObject> _dictAlphaPrefabs = new Dictionary<string, GameObject>();
    
    private float koefSpeed = 1f;

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
    private float _timeErrorInput = 0f;

    
    public IGameState State { get; set; }
    public IGameState OldState { get; set; }
    public Dictionary<Type, IGameState> stateMap;
    bool iniOK;

    public Action<int> trueAlphaInputEvent;    

    private void UpSpeedSimulation()
    {
        if (_levelManager.Level < 5) koefSpeed = 1f;
        else
        {
            if (_levelManager.Level < 10) koefSpeed = 1.2f;
            else
            {
                if (_levelManager.Level < 15) koefSpeed = 1.5f;
                else
                {
                    if (_levelManager.Level < 20) koefSpeed = 2f;
                    else
                    {
                        if (_levelManager.Level < 25) koefSpeed = 2.5f;
                        else
                        {
                            if (_levelManager.Level < 35) koefSpeed = 3f;
                            else
                            {
                                if (_levelManager.Level < 40) koefSpeed = 3.5f;
                                else
                                {
                                    if (_levelManager.Level < 50) koefSpeed = 5f;
                                }
                            }
                        }

                    }
                }

            }
        }
    }
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
        _scoreManager.scoreToNextLevelEvent += _levelManager.LevelUP;
        _scoreManager.scoreToNextLevelEvent += UpSpeedSimulation;
        _levelManager.levelChangeEvent += uiManager.UpdateLevel;
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
        _scoreManager.scoreToNextLevelEvent -= _levelManager.LevelUP;
        _scoreManager.scoreToNextLevelEvent -= UpSpeedSimulation;
        _levelManager.levelChangeEvent -= uiManager.UpdateLevel;
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
        _healthManager.Init(_startHealth); 
    }
    private GameObject CreateAlphaDead(string alphaS, Color32 color)
    {
        if (_dictAlphaPrefabs.ContainsKey(alphaS))
        {
            GameObject alpha = Instantiate(_dictAlphaPrefabs[alphaS]);
            var renderers = alpha.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                renderer.material.color = color;
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
        _levelManager.Init();
        _scoreManager.Init();
        saveLoadManager.LoadData();
        _healthManager.SetMaxHealth();
        _gameCoroutine = StartCoroutine(GameCoroutine());
        _inputCoroutine = StartCoroutine(InputControlCoroutine());
        UpSpeedSimulation();
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
                Vector2 positionRandomX = Vector2.zero;                
                Square square = controllerSquare.TakeNextSquareInPool(_prefabSquare.ID, new Vector3(positionRandomX.x, positionRandomX.y, _parentForActiveObject.position.z));
                square.gameObject.transform.parent = _parentForActiveObject;
                if (_levelManager.Level < 5)
                {
                    square.dataSquare.AddDataSquare(_alphaAll[UnityEngine.Random.Range(0, _alphaAll.Length)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                    controllerSquare.SetPosition(square, _levelManager.Level);
                }
                else
                {
                    if (_levelManager.Level < 10)
                    {
                        int r = UnityEngine.Random.Range(0, 2);
                        if (r == 0)
                        {
                            square.dataSquare.AddDataSquare(_dictCountAlphaLists[2][UnityEngine.Random.Range(0, _dictCountAlphaLists[2].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                            controllerSquare.SetPosition(square, _levelManager.Level);
                        }
                        else
                        {
                            square.dataSquare.AddDataSquare(_alphaAll[UnityEngine.Random.Range(0, _alphaAll.Length)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                            controllerSquare.SetPosition(square, _levelManager.Level);
                        }
                    }
                    else
                    {
                        if (_levelManager.Level < 15)
                        {
                            int r = UnityEngine.Random.Range(0, 3);
                            if (r == 0)
                            {
                                square.dataSquare.AddDataSquare(_dictCountAlphaLists[2][UnityEngine.Random.Range(0, _dictCountAlphaLists[2].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                controllerSquare.SetPosition(square, 5);
                            }
                            if (r == 1)
                            {
                                square.dataSquare.AddDataSquare(_alphaAll[UnityEngine.Random.Range(0, _alphaAll.Length)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                controllerSquare.SetPosition(square, 5);
                            }
                            if (r == 2)
                            {
                                square.dataSquare.AddDataSquare(_dictCountAlphaLists[3][UnityEngine.Random.Range(0, _dictCountAlphaLists[3].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                controllerSquare.SetPosition(square, _levelManager.Level);
                            }
                        }
                        else
                        {
                            if (_levelManager.Level < 20)
                            {
                                int r = UnityEngine.Random.Range(0, 4);
                                if (r == 0)
                                {
                                    square.dataSquare.AddDataSquare(_dictCountAlphaLists[2][UnityEngine.Random.Range(0, _dictCountAlphaLists[2].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                    controllerSquare.SetPosition(square, 5);
                                }
                                if (r == 1)
                                {
                                    square.dataSquare.AddDataSquare(_alphaAll[UnityEngine.Random.Range(0, _alphaAll.Length)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                    controllerSquare.SetPosition(square, 5);
                                }
                                if (r == 2)
                                {
                                    square.dataSquare.AddDataSquare(_dictCountAlphaLists[3][UnityEngine.Random.Range(0, _dictCountAlphaLists[3].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                    controllerSquare.SetPosition(square, 10);
                                }
                                if (r == 3)
                                {
                                    square.dataSquare.AddDataSquare(_dictCountAlphaLists[4][UnityEngine.Random.Range(0, _dictCountAlphaLists[4].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                    controllerSquare.SetPosition(square, _levelManager.Level);
                                }
                            }
                            else
                            {
                                if (_levelManager.Level < 25)
                                {
                                    int r = UnityEngine.Random.Range(0, 5);
                                    controllerSquare.SetPosition(square, 5);
                                    if (r == 0) square.dataSquare.AddDataSquare(_dictCountAlphaLists[2][UnityEngine.Random.Range(0, _dictCountAlphaLists[2].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                    if (r == 1) square.dataSquare.AddDataSquare(_alphaAll[UnityEngine.Random.Range(0, _alphaAll.Length)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                    if (r == 2) square.dataSquare.AddDataSquare(_dictCountAlphaLists[3][UnityEngine.Random.Range(0, _dictCountAlphaLists[3].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                    if (r == 3)
                                    {
                                        square.dataSquare.AddDataSquare(_dictCountAlphaLists[4][UnityEngine.Random.Range(0, _dictCountAlphaLists[4].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                        controllerSquare.SetPosition(square, 15);
                                    }
                                    if (r == 4)
                                    {
                                        square.dataSquare.AddDataSquare(_dictCountAlphaLists[5][UnityEngine.Random.Range(0, _dictCountAlphaLists[5].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                        controllerSquare.SetPosition(square, _levelManager.Level);
                                    }
                                }
                                else
                                {
                                    if (_levelManager.Level < 30)
                                    {
                                        int r = UnityEngine.Random.Range(0, 6);
                                        controllerSquare.SetPosition(square, 5);
                                        if (r == 0) square.dataSquare.AddDataSquare(_dictCountAlphaLists[2][UnityEngine.Random.Range(0, _dictCountAlphaLists[2].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                        if (r == 1) square.dataSquare.AddDataSquare(_alphaAll[UnityEngine.Random.Range(0, _alphaAll.Length)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                        if (r == 2) square.dataSquare.AddDataSquare(_dictCountAlphaLists[3][UnityEngine.Random.Range(0, _dictCountAlphaLists[3].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                        if (r == 3)
                                        {
                                            square.dataSquare.AddDataSquare(_dictCountAlphaLists[4][UnityEngine.Random.Range(0, _dictCountAlphaLists[4].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                            controllerSquare.SetPosition(square, 15);
                                        }
                                        if (r == 4)
                                        {
                                            square.dataSquare.AddDataSquare(_dictCountAlphaLists[5][UnityEngine.Random.Range(0, _dictCountAlphaLists[5].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                            controllerSquare.SetPosition(square, 25);
                                        }
                                        if (r == 5)
                                        {
                                            square.dataSquare.AddDataSquare(_dictCountAlphaLists[6][UnityEngine.Random.Range(0, _dictCountAlphaLists[6].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                            controllerSquare.SetPosition(square, _levelManager.Level);
                                        }
                                    }
                                    else
                                    {
                                        if (_levelManager.Level < 35)
                                        {
                                            int r = UnityEngine.Random.Range(0, 7);
                                            controllerSquare.SetPosition(square, 5);
                                            if (r == 0) square.dataSquare.AddDataSquare(_dictCountAlphaLists[2][UnityEngine.Random.Range(0, _dictCountAlphaLists[2].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                            if (r == 1) square.dataSquare.AddDataSquare(_alphaAll[UnityEngine.Random.Range(0, _alphaAll.Length)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                            if (r == 2) square.dataSquare.AddDataSquare(_dictCountAlphaLists[3][UnityEngine.Random.Range(0, _dictCountAlphaLists[3].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                            if (r == 3)
                                            {
                                                square.dataSquare.AddDataSquare(_dictCountAlphaLists[4][UnityEngine.Random.Range(0, _dictCountAlphaLists[4].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                controllerSquare.SetPosition(square, 15);
                                            }
                                            if (r == 4)
                                            {
                                                square.dataSquare.AddDataSquare(_dictCountAlphaLists[5][UnityEngine.Random.Range(0, _dictCountAlphaLists[5].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                controllerSquare.SetPosition(square, 25);
                                            }
                                            if (r == 5)
                                            {
                                                square.dataSquare.AddDataSquare(_dictCountAlphaLists[6][UnityEngine.Random.Range(0, _dictCountAlphaLists[6].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                controllerSquare.SetPosition(square, 30);
                                            }
                                            if (r == 6)
                                            {
                                                square.dataSquare.AddDataSquare(_dictCountAlphaLists[7][UnityEngine.Random.Range(0, _dictCountAlphaLists[7].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                controllerSquare.SetPosition(square, _levelManager.Level);
                                            }
                                        }
                                        else
                                        {
                                            if (_levelManager.Level < 40)
                                            {
                                                int r = UnityEngine.Random.Range(0, 8);
                                                controllerSquare.SetPosition(square, 5);
                                                if (r == 0) square.dataSquare.AddDataSquare(_dictCountAlphaLists[2][UnityEngine.Random.Range(0, _dictCountAlphaLists[2].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                if (r == 1) square.dataSquare.AddDataSquare(_alphaAll[UnityEngine.Random.Range(0, _alphaAll.Length)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                if (r == 2) square.dataSquare.AddDataSquare(_dictCountAlphaLists[3][UnityEngine.Random.Range(0, _dictCountAlphaLists[3].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                if (r == 3)
                                                {
                                                    square.dataSquare.AddDataSquare(_dictCountAlphaLists[4][UnityEngine.Random.Range(0, _dictCountAlphaLists[4].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                    controllerSquare.SetPosition(square, 15);
                                                }
                                                if (r == 4)
                                                {
                                                    square.dataSquare.AddDataSquare(_dictCountAlphaLists[5][UnityEngine.Random.Range(0, _dictCountAlphaLists[5].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                    controllerSquare.SetPosition(square, 20);
                                                }
                                                if (r == 5)
                                                {
                                                    square.dataSquare.AddDataSquare(_dictCountAlphaLists[6][UnityEngine.Random.Range(0, _dictCountAlphaLists[6].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                    controllerSquare.SetPosition(square, 25);
                                                }
                                                if (r == 6)
                                                {
                                                    square.dataSquare.AddDataSquare(_dictCountAlphaLists[7][UnityEngine.Random.Range(0, _dictCountAlphaLists[7].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                    controllerSquare.SetPosition(square, 30);
                                                }
                                                if (r == 7)
                                                {
                                                    square.dataSquare.AddDataSquare(_dictCountAlphaLists[8][UnityEngine.Random.Range(0, _dictCountAlphaLists[8].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                }
                                            }
                                            else
                                            {
                                                if (_levelManager.Level < 45)
                                                {
                                                    int r = UnityEngine.Random.Range(0, 9);
                                                    controllerSquare.SetPosition(square, 5);
                                                    if (r == 0) square.dataSquare.AddDataSquare(_dictCountAlphaLists[2][UnityEngine.Random.Range(0, _dictCountAlphaLists[2].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                    if (r == 1) square.dataSquare.AddDataSquare(_alphaAll[UnityEngine.Random.Range(0, _alphaAll.Length)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                    if (r == 2) square.dataSquare.AddDataSquare(_dictCountAlphaLists[3][UnityEngine.Random.Range(0, _dictCountAlphaLists[3].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                    if (r == 3)
                                                    {
                                                        square.dataSquare.AddDataSquare(_dictCountAlphaLists[4][UnityEngine.Random.Range(0, _dictCountAlphaLists[4].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                        controllerSquare.SetPosition(square, 20);
                                                    }
                                                    if (r == 4)
                                                    {
                                                        square.dataSquare.AddDataSquare(_dictCountAlphaLists[5][UnityEngine.Random.Range(0, _dictCountAlphaLists[5].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                        controllerSquare.SetPosition(square, 25);
                                                    }
                                                    if (r == 5)
                                                    {
                                                        square.dataSquare.AddDataSquare(_dictCountAlphaLists[6][UnityEngine.Random.Range(0, _dictCountAlphaLists[6].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                        controllerSquare.SetPosition(square, 30);
                                                    }
                                                    if (r == 6)
                                                    {
                                                        square.dataSquare.AddDataSquare(_dictCountAlphaLists[7][UnityEngine.Random.Range(0, _dictCountAlphaLists[7].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                        controllerSquare.SetPosition(square, 35);
                                                    }
                                                    if (r == 7)
                                                    {
                                                        square.dataSquare.AddDataSquare(_dictCountAlphaLists[8][UnityEngine.Random.Range(0, _dictCountAlphaLists[8].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                        controllerSquare.SetPosition(square, 40);
                                                    }
                                                    if (r == 8)
                                                    {
                                                        square.dataSquare.AddDataSquare(_dictCountAlphaLists[9][UnityEngine.Random.Range(0, _dictCountAlphaLists[9].Count)], _colors[UnityEngine.Random.Range(0, _colors.Count)]);
                                                        controllerSquare.SetPosition(square, _levelManager.Level);
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
                square.UpdateSpeed(_levelManager.Level);
                square.gameObject.SetActive(true);                
            }
            yield return new WaitForSeconds(_defaultTimeForRespawnSquade / koefSpeed);
        }
    }

    private IEnumerator InputControlCoroutine()
    {
        while (true)
        {
            if (_timeErrorInput >= 0) _timeErrorInput -= Time.deltaTime;
            if (State != GetState<PauseState>())
            {
                if (Input.anyKey)
                {
                    Square square = null;
                    string alpha = Input.inputString.ToLower();
                    if (controllerSquare.CheckAlpha(alpha, out square))
                    {
                        if (square != null)
                        {
                            if (square.dataSquare.DeleteAlpha(alpha))
                            {
                                Vector3 position = square.DisableAlpha();
                                GameObject deadAlpha = CreateAlphaDead(alpha, square.dataSquare.Color);
                                if (deadAlpha != null) deadAlpha.transform.position = position;
                                controllerSquare.ReturnToPool(square);
                                trueAlphaInputEvent?.Invoke(square.dataSquare.Balls);
                            } 
                            else
                            {
                                Vector3 position = square.DisableAlpha();
                                GameObject deadAlpha = CreateAlphaDead(alpha, square.dataSquare.Color);
                                if (deadAlpha != null) deadAlpha.transform.position = position;
                                trueAlphaInputEvent?.Invoke(square.dataSquare.Balls);
                            }
                        }
                    }
                    else
                    {
                        if (_timeErrorInput < 0)
                        {
                            DecHealth(1);
                            _timeErrorInput = timeErrorInputDefault;
                        }
                    }
                }                              
            }
            yield return null;
        }
    }    
}
