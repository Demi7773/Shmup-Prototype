using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{

    [SerializeField] private GameObject _panel;

    [SerializeField] private Slider _mainVolume;
    [SerializeField] private Slider _musicVolume;
    [SerializeField] private Slider _sfxVolume;

    [SerializeField] private Button _applyChangesButton;
    [SerializeField] private Button _returnButton;

    [SerializeField] private float _mainVolumeValue = 1.0f;
    [SerializeField] private float _musicVolumeValue = 1.0f;
    [SerializeField] private float _sfxVolumeValue = 1.0f;



    private void OnEnable()
    {
        LoadSettings();

        _mainVolume.onValueChanged.AddListener(MainVolumeSliderChanged);
        _musicVolume.onValueChanged.AddListener(MusicVolumeSliderChanged);
        _sfxVolume.onValueChanged.AddListener(SFXVolumeSliderChanged);

        _applyChangesButton.onClick.AddListener(Apply);
        _returnButton.onClick.AddListener(Return);

        //MainMenuButtonsOld.SettingsPressed += OpenSettingsMenu;
    }
    private void OnDisable()
    {
        _mainVolume.onValueChanged.RemoveAllListeners();
        _musicVolume.onValueChanged.RemoveAllListeners();
        _sfxVolume.onValueChanged.RemoveAllListeners();

        _applyChangesButton.onClick.RemoveAllListeners();
        _returnButton.onClick.RemoveAllListeners();

        //MainMenuButtonsOld.SettingsPressed -= OpenSettingsMenu;
    }

    private void LoadSettings()
    {
        _mainVolume.value = GlobalGameSettings.MainAudioVolume;
        _sfxVolume.value = GlobalGameSettings.SFXVolume;
        _musicVolume.value = GlobalGameSettings.MusicVolume;
    }


    private void OpenSettingsMenu()
    {
        _panel.SetActive(true);
    }
    private void CloseSettingsMenu()
    {
        _panel.SetActive(false);
    }


    private void MainVolumeSliderChanged(float value)
    {
        _mainVolumeValue = value;
    }
    private void MusicVolumeSliderChanged(float value)
    {
        _musicVolumeValue = value;
    }
    private void SFXVolumeSliderChanged(float value)
    {
        _sfxVolumeValue = value;
    }


    private void Apply()
    {
        //GlobalGameSettings.MainAudioVolume = _mainVolumeValue;
        //GlobalGameSettings.MusicVolume = _musicVolumeValue;
        //GlobalGameSettings.SFXVolume = _sfxVolumeValue;
        GlobalGameSettings.SetAudioSettings(_mainVolumeValue, _musicVolumeValue, _sfxVolumeValue);
        Debug.Log("Audio settings applied: " +  _mainVolumeValue + ", " + _musicVolumeValue + ", " + _sfxVolumeValue);
    }
    private void Return()
    {
        UIEvents.CloseSettingsClick();
        //CloseSettingsMenu();
    }

}
