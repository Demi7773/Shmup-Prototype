using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuButtonsOld : MonoBehaviour
{
    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _quitButton;

    public static event Action StartPressed;
    //public static event Action SettingsPressed;
    public static event Action QuitPressed;



    private void OnEnable()
    {
        _startButton.onClick.AddListener(StartClick);
        _settingsButton.onClick.AddListener(SettingsClick);
        _quitButton.onClick.AddListener(QuitClick);
    }
    private void OnDisable()
    {
        _startButton.onClick.RemoveAllListeners();
        _settingsButton.onClick.RemoveAllListeners();
        _quitButton.onClick.RemoveAllListeners();
    }

    private void StartClick()
    {
        StartPressed?.Invoke();
        //CloseMenu();
    }
    private void SettingsClick()
    {
        UIEvents.SettingsClick();
        //SettingsPressed?.Invoke();
    }
    private void QuitClick()
    {
        QuitPressed?.Invoke();
        //CloseMenu();
    }

    //public void OpenMenu()
    //{
    //    _mainMenuPanel.SetActive(true);
    //}
    //public void CloseMenu()
    //{
    //    _mainMenuPanel.SetActive(false);
    //}

}
