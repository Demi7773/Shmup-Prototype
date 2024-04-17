using UnityEngine;

public class MusicManager : MonoBehaviour
{

    [SerializeField] private AudioSource _musicAudioSource;

    [SerializeField] private AudioClip _mainMenuMusic;
    [SerializeField] private AudioClip _defaultLevelMusic;
    [SerializeField] private AudioClip _combatMusic;




    private void OnEnable()
    {
        GlobalGameSettings.OnAudioSettingsChange += SetVolume;

        //GameManager.OnEnterMainMenu += PlayMainMenuMusic;
        GameManager.PlayMusicForLevel += PlayLevelMusic;
        //GameManager.OnEnterCombat += PlayCombatMusic;

        SetVolume();
        PlayMainMenuMusic();
    }
    private void OnDisable()
    {
        GlobalGameSettings.OnAudioSettingsChange -= SetVolume;

        //GameManager.OnEnterMainMenu -= PlayMainMenuMusic;
        GameManager.PlayMusicForLevel -= PlayLevelMusic;
        //GameManager.OnEnterCombat -= PlayCombatMusic;
    }
    private void SetVolume()
    {
        _musicAudioSource.volume = GlobalGameSettings.AdjustedMusicVolume;
        //_musicAudioSource.volume = GlobalGameSettings.AdjustedMusicVolume();
        //Debug.Log("Music Volume adjusted");
    }




    private void PlayMainMenuMusic()
    {
        _musicAudioSource.clip = _mainMenuMusic;
        _musicAudioSource.Play();
    }
    private void PlayLevelMusic(int level)
    {
        //Debug.Log("Playing Level " + level + " music. Implement music for other levels / states");
        _musicAudioSource.clip = _defaultLevelMusic;
        _musicAudioSource.Play();
    }
    private void PlayCombatMusic()
    {
        _musicAudioSource.clip = _combatMusic;
        _musicAudioSource.Play();
    }

}
