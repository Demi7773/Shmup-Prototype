using System;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    private GameManager _gm;
    [SerializeField] private MainMenuButtonsOld _mainMenu;
    [SerializeField] private GameObject/*LoadingScreen*/ _loading;
    [SerializeField] private Image _loadBar;
    [SerializeField] private PlayerUI _hudPanel;
    [SerializeField] private PauseMenu _pauseMenu;
    [SerializeField] private SettingsMenu _settingsMenu;
    [SerializeField] private GameOverMenu _gameOverPanel;




    [SerializeField] private UIState _currentState;
    [SerializeField] private UIState _lastState;

    public static event Action<UIManager> OnUIManagerSpawned;

    public static event Action CallForStartGame;
    public static event Action CallForQuitToDesktop;
    public static event Action CallForPauseMenu;
    public static event Action CallForBackToMainMenu;
    public static event Action CallForSettings;
    public static event Action CallForResumeGame;



    
    public enum UIState
    {
        MainMenu,
        Loading,
        Ingame,
        PauseMenu,
        SettingsMenu,
        GameOver
    }


    private void Awake()
    {
        OnUIManagerSpawned?.Invoke(this);
        //Debug.Log("New UIManager spawn ping");
    }

    public void Init(GameManager gm)
    {
        _gm = gm;
        _currentState = UIState.MainMenu;
        ChangeUIToState(_currentState);
        //Debug.Log("UIManager SpawnMeAt from GM completed");
    }




    private void OnEnable()
    {
        UIEvents.OnUIStateChange += ChangeUIToState;
        

        PauseMenu.ResumeButtonPressed += OnResumeButtonPressed;

        MainMenuButtonsOld.StartPressed += OnStartButtonPressed;
        //MainMenuButtonsOld.SettingsPressed += OnSettingsButtonPressed;
        MainMenuButtonsOld.QuitPressed += OnQuitToDesktopButtonPressed;

        UIEvents.SettingsButtonPressed += OnSettingsButtonPressed;
        UIEvents.CloseSettingsPressed += OnCloseSettings;

        PauseMenu.ResumeButtonPressed += OnResumeButtonPressed;
        //PauseMenu.SettingsButtonPressed += OnSettingsButtonPressed;
        PauseMenu.QuitToMainMenuButtonPressed += OnQuitToMainButtonPressed;
        PauseMenu.QuitToDesktopButtonPressed += OnQuitToDesktopButtonPressed;

        GameOverMenu.QuitToMainMenu += OnQuitToMainButtonPressed;
        GameOverMenu.QuitToDesktop += OnQuitToDesktopButtonPressed;

        UIEvents.OnLoadingProgress += OnLoadingProgress;
    }

    private void OnDisable()
    {
        UIEvents.OnUIStateChange -= ChangeUIToState;

        PauseMenu.ResumeButtonPressed -= OnResumeButtonPressed;

        MainMenuButtonsOld.StartPressed -= OnStartButtonPressed;
        //MainMenuButtonsOld.SettingsPressed -= OnSettingsButtonPressed;
        MainMenuButtonsOld.QuitPressed -= OnQuitToDesktopButtonPressed;

        UIEvents.SettingsButtonPressed -= OnSettingsButtonPressed;
        UIEvents.CloseSettingsPressed -= OnCloseSettings;

        PauseMenu.ResumeButtonPressed -= OnResumeButtonPressed;
        //PauseMenu.SettingsButtonPressed -= OnSettingsButtonPressed;
        PauseMenu.QuitToMainMenuButtonPressed -= OnQuitToMainButtonPressed;
        PauseMenu.QuitToDesktopButtonPressed -= OnQuitToDesktopButtonPressed;

        GameOverMenu.QuitToMainMenu -= OnQuitToMainButtonPressed;
        GameOverMenu.QuitToDesktop -= OnQuitToDesktopButtonPressed;

        UIEvents.OnLoadingProgress += OnLoadingProgress;
    }

    private void OnResumeButtonPressed()
    {
        CallForResumeGame?.Invoke();
    }
    private void OnStartButtonPressed()
    {
        CallForStartGame?.Invoke();
    }
    private void OnSettingsButtonPressed()
    {
        _lastState = _currentState;
        //Debug.Log("Settings button pressed, Last State set to " + _lastState);
        CallForSettings?.Invoke();
    }
    private void OnCloseSettings()
    {
        if (_lastState == UIState.PauseMenu)
        {
            //Debug.Log("Last State was PauseMenu, calling for swap back to PauseMenu");
            CallForPauseMenu?.Invoke();
            return;
        }

        //Debug.Log("Last State was " + _lastState + ", calling for swap back to MainMenu");
        CallForBackToMainMenu?.Invoke();
    }
    private void OnQuitToDesktopButtonPressed()
    {
        CallForQuitToDesktop?.Invoke();
    }
    private void OnQuitToMainButtonPressed()
    {
        CallForBackToMainMenu?.Invoke();
    }
    private void OnLoadingProgress(float progress)
    {
        _loadBar.fillAmount = progress;
    }



    private void ChangeUIToState(UIState state)
    {
        _lastState = _currentState;

        //Debug.Log("Switching to UIState: " + state);
        switch (state)
        {
            default:
                Debug.LogError("UIState not recognized");
                break;

            case UIState.MainMenu:
                EnterMainMenuState();
                break;

            case UIState.Loading:
                EnterLoadingState();
                break;

            case UIState.Ingame:
                EnterIngameState();
                break;

            case UIState.PauseMenu:
                EnterPauseMenuState();
                break;

            case UIState.SettingsMenu:
                EnterSettingsMenuState();
                break;

            case UIState.GameOver:
                EnterGameOverState();
                break;
        }

        _currentState = state;
    }

    private void EnterMainMenuState()
    {
        _mainMenu.gameObject.SetActive(true);

        _loading.gameObject.SetActive(false);
        _hudPanel.gameObject.SetActive(false);
        _pauseMenu.gameObject.SetActive(false);
        _settingsMenu.gameObject.SetActive(false);
        _gameOverPanel.gameObject.SetActive(false);
    }
    private void EnterLoadingState()
    {
        _mainMenu.gameObject.SetActive(false);

        _loading.gameObject.SetActive(true);

        _hudPanel.gameObject.SetActive(false);
        _pauseMenu.gameObject.SetActive(false);
        _settingsMenu.gameObject.SetActive(false);
        _gameOverPanel.gameObject.SetActive(false);
    }
    private void EnterIngameState()
    {
        _mainMenu.gameObject.SetActive(false);
        _loading.gameObject.SetActive(false);

        _hudPanel.gameObject.SetActive(true);

        _pauseMenu.gameObject.SetActive(false);
        _settingsMenu.gameObject.SetActive(false);
        _gameOverPanel.gameObject.SetActive(false);

        UIEvents.CallHUDUpdate();
    }
    private void EnterPauseMenuState()
    {
        _pauseMenu.gameObject.SetActive(true);
        _settingsMenu.gameObject.SetActive(false);
    }
    private void EnterSettingsMenuState()
    {
        _settingsMenu.gameObject.SetActive(true);
    }
    private void EnterGameOverState()
    {
        _gameOverPanel.gameObject.SetActive(true);
    }











    //private void OnDisplayMainMenu(bool enable)
    //{
    //    _mainMenu.gameObject.SetActive(enable);
    //}
    //private void OnDisplayIngameHUD(bool enable)
    //{
    //    _hudPanel.gameObject.SetActive(enable);
    //}
    //private void OnDisplayPauseMenu(bool enable)
    //{
    //    _pauseMenu.gameObject.SetActive(enable);
    //}
    //private void OnDisplaySettingsMenu(bool enable)
    //{
    //    _settingsMenu.gameObject.SetActive(enable);
    //}
    //private void OnDisplayGameOverMenu(bool enable)
    //{
    //    _gameOverPanel.gameObject.SetActive(enable);
    //}

}
