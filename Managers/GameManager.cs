using ReworkedWeapons;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{

    public static GameManager Instance;
    [Header("Dependencies")]
    [SerializeField] private SceneMgmt _sceneMgmt;
    [SerializeField] private SaveLoadManager _saveLoadManager;
    [SerializeField] private InputReaderAdvanced _input;
    [SerializeField] private GameObject _defaultBGForScene0;
    private UIManager _uiManager;
    [Header("Progress")]
    [SerializeField] private int _currentLevel = 0;
    public int CurrentLevel => _currentLevel;
    [Space(20)]
    [Header("Debug OnLevel References")]
    [SerializeField] private LevelController _activeLevelController;
    [SerializeField] private PlayerReferences _playerRef;
    private NewPlayerController _playerControls;
    public enum GameState
    {
        Playing,
        Paused,
        MainMenu,
        GameOver
    }
    [SerializeField] private GameState _state;
    [SerializeField] private float _targetTimeScale = 1f;
    [SerializeField] private float _ingameTimer = 0f;
    [SerializeField] private float _ingameUnscaledTimer = 0f;
    public GameState State => _state;
    public PlayerReferences PlayerRef => _playerRef;
    public NewPlayerController PlayerControls => _playerControls;
    public float IngameTimer => _ingameTimer;
    public float IngameUnscaledTimer => _ingameUnscaledTimer;

    [Header("Run stats transfer")]
    [SerializeField] private float _startingCurrentHP = 30.0f;
    [SerializeField] private float _startingMaxHP = 30.0f;
    [SerializeField] private int _startingCash = 0;
    [SerializeField] private List<IWeapon.GunID> _startingWeaponIDs = new List<IWeapon.GunID>();
    [SerializeField] private float _startingMaxEnergy = 30.0f;
    private RunPlayerDataHolder _currentRunData;
    public RunPlayerDataHolder CurrentRunData => _currentRunData;



    public static event Action<GameState> OnGameStateChanged;
    public static event Action<int> PlayMusicForLevel;
    public static event Action<int> OnNewLevelStarted;


    //public static event Action<PlayerReferences> OnPlayerChanged;
    //public static event Action<float, float> OnTimerTick;
    //public static event Action<PlayerReferences> LevelLoaded;







    private void Awake()
    {
        _defaultBGForScene0.SetActive(true);
        Init();
    }
    private void Init()
    {
        if (Instance != null)
        {
            Debug.Log("GM Instance already exists, destroying !!!");
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        _sceneMgmt.Init(this);
        GetLoadDataBySaveSlot();
    }

    private void GetLoadDataBySaveSlot()
    {
        SaveLoadData data = _saveLoadManager.GetLoadDataBySlot(0);
        Debug.Log("GM recieved LoadData: " + data + " TEST - finish implementing");
    }

    private void OnEnable()
    {
            // references
        UIManager.OnUIManagerSpawned += SetUIManagerReference;
        PlayerReferences.NewPlayerReference += SetPlayerReference;
        LevelController.OnLevelLoaded += SetNewActiveLevelController;

        LevelController.OnExitLevel += ExitCurrentLevel;

        // change state calls
        _input.PauseEvent += HandlePause;
        _input.ResumeEvent += HandleResume;

        UIManager.CallForStartGame += OnStartGameCalled;
        UIManager.CallForSettings += OnSettingsCalled;
        UIManager.CallForQuitToDesktop += OnQuitToDesktopCalled;
        UIManager.CallForPauseMenu += PauseGame;
        UIManager.CallForResumeGame += OnResumeGameCalled;
        UIManager.CallForBackToMainMenu += OnBackToMainMenuCalled;

        GameOverMenu.OnRetryPressed += OnRetryCalled;

            // timeslow effects
        PlayerEvents.OnPlayerSlowsDownTime += SlowDownTimeStart;
        PlayerEvents.OnSlowDownTimeStops += SlowDownTimeStop;
    }

    private void OnDisable()
    {
        UIManager.OnUIManagerSpawned -= SetUIManagerReference;
        PlayerReferences.NewPlayerReference -= SetPlayerReference;
        LevelController.OnLevelLoaded -= SetNewActiveLevelController;

        LevelController.OnExitLevel += ExitCurrentLevel;

        _input.PauseEvent -= HandlePause;
        _input.ResumeEvent -= HandleResume;

        GameOverMenu.OnRetryPressed += OnRetryCalled;

        UIManager.CallForStartGame -= OnStartGameCalled;
        UIManager.CallForSettings -= OnSettingsCalled;
        UIManager.CallForQuitToDesktop -= OnQuitToDesktopCalled;
        UIManager.CallForPauseMenu -= PauseGame;
        UIManager.CallForResumeGame -= OnResumeGameCalled;
        UIManager.CallForBackToMainMenu -= OnBackToMainMenuCalled;

        PlayerEvents.OnPlayerSlowsDownTime -= SlowDownTimeStart;
        PlayerEvents.OnSlowDownTimeStops -= SlowDownTimeStop;
    }

    private void SetUIManagerReference(UIManager uiMngr)
    {
        _uiManager = uiMngr;
        _uiManager.Init(this);
    }
    private void SetNewActiveLevelController(LevelController levelController)
    {
        _activeLevelController = levelController;
    }
    private void SetPlayerReference(PlayerReferences playerRef)
    {
        _playerRef = playerRef;
        _playerRef.Init(this, _input);
    } 




    private void OnRetryCalled()
    {
        StartCoroutine(RetryLevel());
    }
    private IEnumerator RetryLevel()
    {
        ExitCurrentLevel(false);
        UIEvents.ChangeUIState(UIManager.UIState.Loading);
        yield return new WaitForSeconds(1);
        OnStartGameCalled();
    }



    private void OnStartGameCalled()
    {
        _defaultBGForScene0.SetActive(false);
        _currentRunData = RunPlayerDataHolder.GetNewPlayerDataHolder(_startingCurrentHP, _startingMaxHP, _startingCash, _startingWeaponIDs, _startingMaxEnergy);
        Debug.Log("Start Game called - Starting Level1 Load - implement level selection");

        int level = 1;
        StartCoroutine(InitNewLevelSequence(level));
    }

    private void OnBackToMainMenuCalled()
    {
        _defaultBGForScene0.SetActive(true);
        //PauseGame();
        ChangeGameState(GameState.MainMenu);
        UIEvents.ChangeUIState(UIManager.UIState.MainMenu);
        //DespawnPlayer();

        StartCoroutine(ExitingLevelSequence(false, false));
        Debug.Log("Run ended");
    }

    private void OnQuitToDesktopCalled()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    private void OnSettingsCalled()
    {
        UIEvents.ChangeUIState(UIManager.UIState.SettingsMenu);
    }

    private void HandlePause()
    {
        if (_state == GameState.Paused)
        {
            return;
        }
        PauseGame();
    }

    private void OnResumeGameCalled()
    {
        HandleResume();
    }
    private void HandleResume()
    {
        if (_state != GameState.Paused)
        {
            return;
        }
        UnPauseGame();
    }



    private void SlowDownTimeStart(float timeScale/*, float duration*/)
    {
        //_modeEndsTimer = IngameUnscaledTimer + duration;
        //_isTimeSlowed = true;
        _targetTimeScale = timeScale;
        Time.timeScale = _targetTimeScale;
        //Debug.Log("SlowDownTime started at " + _ingameUnscaledTimer + ", timeScale set to " + Time.timeScale);
    }
    private void SlowDownTimeStop()
    {
        //_isTimeSlowed = false;
        _targetTimeScale = 1.0f;
        Time.timeScale = _targetTimeScale;
        //Debug.Log("SlowDownTime ended at " + _ingameUnscaledTimer + ", timeScale set to " + Time.timeScale);
    }



 

    private void ChangeGameState(GameState state)
    {
        _state = state;
        OnGameStateChanged?.Invoke(_state);
    }

    private void Update()
    {
        if (State == GameState.Playing)
        {
            _ingameTimer += Time.deltaTime;
            _ingameUnscaledTimer += Time.unscaledDeltaTime;
        }
    }






    private void ExitCurrentLevel(bool wasLevelCompleted)
    {
       StartCoroutine(ExitingLevelSequence(true, wasLevelCompleted));
    }
    public IEnumerator ExitingLevelSequence(bool quittingFromLevel, bool wasLevelCompleted)
    {
        UIEvents.ChangeUIState(UIManager.UIState.Loading);

        if (quittingFromLevel)
        {
            PlayerHP playerHP = _playerRef.PlayerHP;
            _currentRunData = RunPlayerDataHolder.GetNewPlayerDataHolder(playerHP.CurrentHP, playerHP.MaxHP, _playerRef.PlayerMoney.Money, _playerRef.WeaponManager.HeldWeaponsIDsList(), _playerRef.PlayerEnergy.MaxEnergy);
            Debug.Log("Storing RunData complete, Unloading current level");

            yield return null;
            SaveProgressData(true, _activeLevelController.WaveReached, _activeLevelController.LevelTimer);

            yield return null;
            _sceneMgmt.UnloadCurrentLevel(wasLevelCompleted);
        }
        else
        {
            UIEvents.ChangeUIState(UIManager.UIState.MainMenu);
        }

        GetLoadDataBySaveSlot();
        //Debug.Log("Load data here");
    }
    public void OnLevelUnloaded(bool wasLevelCompleted)
    {
        Debug.Log("GM recieved confirmation Level was unloaded. Deciding next step");
        
        if (wasLevelCompleted)
        {
            Debug.Log("Level completed succesfully: " + _currentLevel);
            int level = _currentLevel + 1;
            StartCoroutine(InitNewLevelSequence(level));
            return;
        }

        Debug.Log("Level not completed succesfuly, going to MainMenuState");
        UIEvents.ChangeUIState(UIManager.UIState.MainMenu);
    }
    private void SaveProgressData(bool wasLevelCompleted, int waveInLevelReached, float levelTimer)
    {
        Debug.Log("Call saving data from GM");
        _saveLoadManager.SaveProgressData(_currentLevel, wasLevelCompleted, waveInLevelReached, levelTimer, _ingameUnscaledTimer);
    }




    private IEnumerator InitNewLevelSequence(int level)
    {
        string levelSceneName = "";
        switch (level)
        {
            default: levelSceneName = SceneNameStrings.Level1Name;
                break;
            case 1:
                levelSceneName = SceneNameStrings.Level1Name;
                break;
            case 2:
                levelSceneName = SceneNameStrings.Level2Name;
                break;
        }
        //Debug.Log("Start of new LevelLoad: " + levelSceneName);
        _sceneMgmt.LoadNewLevel(levelSceneName);

        while (_activeLevelController == null)
        {
            //Debug.Log("GM Waiting for LevelController ref tick");
            yield return null;
            if (_activeLevelController != null)
            {
                //Debug.Log("New LevelController set in GM: " + _activeLevelController.name);
                break;
            }
        }

        _activeLevelController.Init(this);
        //Debug.Log("Level controller Init called, starting level");
        //yield return new WaitForSeconds(1);
        StartLevel(level);
    }
    private void StartLevel(int level)
    {
        //Debug.Log("StartLevel final setup");
        _ingameTimer = 0;
        _ingameUnscaledTimer = 0;
        ChangeGameState(GameState.Playing);
        UnPauseGame();
        UIEvents.ChangeUIState(UIManager.UIState.Ingame);

        _currentLevel = level;

        OnNewLevelStarted?.Invoke(_currentLevel);
        PlayMusicForLevel?.Invoke(_currentLevel);

        if (_currentLevel == 1)
        {
            UIEvents.DisplayNewTutorial(TutorialDisplay.TutorialContent.Tutorial1);
        }
        //Debug.Log("GM finished setting up Level, Starting Level");
    }





    public void GameOver()
    {
        ChangeGameState(GameState.GameOver);
        //PauseGame();
        SaveProgressData(false, _activeLevelController.WaveReached, _activeLevelController.LevelTimer);
        UIEvents.ChangeUIState(UIManager.UIState.GameOver);
        Debug.Log("GAME OVER");
    }



    private void PauseGame()
    {
        ChangeGameState(GameState.Paused);
        Time.timeScale = 0.0f;
        _input.SetUI();
        UIEvents.ChangeUIState(UIManager.UIState.PauseMenu);
    }
    private void UnPauseGame()
    {
        if (_state == GameState.GameOver)
        {
            return;
        }

        ChangeGameState(GameState.Playing);
        Time.timeScale = _targetTimeScale;
        _input.SetGameplay();
        UIEvents.ChangeUIState(UIManager.UIState.Ingame);
    }

}
