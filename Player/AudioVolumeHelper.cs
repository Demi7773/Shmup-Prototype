using UnityEngine;

public class AudioVolumeHelper : MonoBehaviour
{

    [SerializeField] private AudioSource _playerAudioSource;



    public void OnEnable()
    {
        GlobalGameSettings.OnAudioSettingsChange += SetPlayerSFXVolume;
        SetPlayerSFXVolume();
    }
    private void OnDisable()
    {
        GlobalGameSettings.OnAudioSettingsChange -= SetPlayerSFXVolume;
    }

    private void SetPlayerSFXVolume()
    {
        _playerAudioSource.volume = GlobalGameSettings.AdjustedSFXVolume;
        //_playerAudioSource.volume = GlobalGameSettings.AdjustedSFXVolume();
        //Debug.Log("PlayerSFX Volume adjusted");
    }

}
