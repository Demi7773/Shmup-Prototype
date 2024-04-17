
using System;
using UnityEngine;

public static class GlobalGameSettings
{

    private static float HiddenMainAudioModifier = 1.0f;

    public static float MainAudioVolume = 1.0f;
    public static float MusicVolume = 1.0f;
    public static float SFXVolume = 1.0f;


    public static event Action OnAudioSettingsChange;

    public static float AdjustedSFXVolume => Mathf.Clamp01(MainAudioVolume * SFXVolume * HiddenMainAudioModifier);
    public static float AdjustedMusicVolume => Mathf.Clamp01(MainAudioVolume * MusicVolume * HiddenMainAudioModifier);

    public static void SetAudioSettings(float main, float music, float sfx)
    {
        MainAudioVolume = Mathf.Clamp01(main);
        MusicVolume = Mathf.Clamp01(music);
        SFXVolume = Mathf.Clamp01(sfx);

        OnAudioSettingsChange?.Invoke();
    }


    //public static float AdjustedSFXVolume()
    //{
    //    return Mathf.Clamp01(MainAudioVolume * SFXVolume * HiddenMainAudioModifier);
    //}
    //public static float AdjustedMusicVolume()
    //{
    //    return Mathf.Clamp01(MainAudioVolume * MusicVolume * HiddenMainAudioModifier);
    //}

}
