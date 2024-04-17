using System;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] private Button _resumeBtn;
    [SerializeField] private Button _settingsBtn;
    [SerializeField] private Button _backToMainBtn;
    [SerializeField] private Button _quitBtn;

    [SerializeField] private GameObject _settingsPopUp;

    [SerializeField] private GameObject _quitToMainPopUp;
    [SerializeField] private Button _confirmQuitToMain;
    [SerializeField] private Button _cancelQuitToMain;

    [SerializeField] private GameObject _quitToDesktopPopUp;
    [SerializeField] private Button _confirmQuitToDesktop;
    [SerializeField] private Button _cancelQuitToDesktop;


    public static event Action ResumeButtonPressed;
    //public static event Action SettingsButtonPressed;
    public static event Action QuitToMainMenuButtonPressed;
    public static event Action QuitToDesktopButtonPressed;




    private void OnEnable()
    {
        _resumeBtn.onClick.AddListener(ResumeClicked);
        _settingsBtn.onClick.AddListener(SettingsClicked);
        _backToMainBtn.onClick.AddListener(BackToMainClicked);
        _quitBtn.onClick.AddListener(QuitClicked);

        _confirmQuitToMain.onClick.AddListener(ConfirmToMainClicked);
        _cancelQuitToMain.onClick.AddListener(CancelToMainClicked);

        _confirmQuitToDesktop.onClick.AddListener(ConfirmToDesktopClicked);
        _cancelQuitToDesktop.onClick.AddListener(CancelToDesktopClicked);   
    }
    private void OnDisable()
    {
        _resumeBtn.onClick.RemoveAllListeners();
        _settingsBtn.onClick.RemoveAllListeners();
        _backToMainBtn.onClick.RemoveAllListeners();
        _quitBtn.onClick.RemoveAllListeners();

        _confirmQuitToMain.onClick.RemoveAllListeners();
        _cancelQuitToMain.onClick.RemoveAllListeners();

        _confirmQuitToDesktop.onClick.RemoveAllListeners();
        _cancelQuitToDesktop.onClick.RemoveAllListeners();
    }


    private void ResumeClicked()
    {
        ResumeButtonPressed?.Invoke();
    }
    private void SettingsClicked()
    {
        UIEvents.SettingsClick();
        //SettingsButtonPressed?.Invoke();
    }
    private void BackToMainClicked()
    {
        _quitToMainPopUp.SetActive(true);
    }
    private void QuitClicked()
    {
        _quitToDesktopPopUp.SetActive(true);
    }


    private void ConfirmToMainClicked()
    {
        _quitToMainPopUp.SetActive(false);
        QuitToMainMenuButtonPressed?.Invoke();
        gameObject.SetActive(false);
    }
    private void CancelToMainClicked()
    {
        _quitToMainPopUp.SetActive(false);
    }

    private void ConfirmToDesktopClicked()
    {
        _quitToDesktopPopUp.SetActive(false);
        QuitToDesktopButtonPressed?.Invoke();
        gameObject.SetActive(false);
    }
    private void CancelToDesktopClicked()
    {
        _quitToDesktopPopUp.SetActive(false);
    }

}
