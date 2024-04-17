

using System;
using static UIManager;

public static class UIEvents
{

    //public static event Action<bool> DisplayMainMenu;
    //public static event Action<bool> DisplayIngameHUD;
    //public static event Action<bool> DisplayPauseMenu;
    //public static event Action<bool> DisplaySettingsMenu;
    //public static event Action<bool> DisplayGameOverMenu;

    public static event Action<UIState> OnUIStateChange;
    public static event Action SettingsButtonPressed;
    public static event Action CloseSettingsPressed;

    public static event Action OnUpdateAllHUD;

    public static event Action<string> OnNewMessage;

    public static event Action<TutorialDisplay.TutorialContent> OnDisplayNewTutorial;

    public static event Action<float> OnLoadingProgress;







    public static void ChangeUIState(UIState state)
    {
        OnUIStateChange?.Invoke(state);
    }
    public static void SettingsClick()
    {
        SettingsButtonPressed?.Invoke();
    }
    public static void CloseSettingsClick()
    {
        CloseSettingsPressed?.Invoke();
    }
    public static void CallHUDUpdate()
    {
        OnUpdateAllHUD?.Invoke();
    }
    public static void DisplayNewMessage(string message)
    {
        OnNewMessage?.Invoke(message);
    }

    public static void DisplayNewTutorial(TutorialDisplay.TutorialContent content)
    {
        OnDisplayNewTutorial?.Invoke(content);
    }
    public static void SetLoadBarProgress(float progress)
    {
        OnLoadingProgress?.Invoke(progress);
    }

    //public static void ToggleMainMenu(bool enabled)
    //{
    //    DisplayMainMenu?.Invoke(enabled);
    //}
    //public static void ToggleIngameHUD(bool enabled)
    //{
    //    DisplayIngameHUD?.Invoke(enabled);
    //}
    //public static void TogglePauseMenu(bool enabled)
    //{
    //    DisplayPauseMenu?.Invoke(enabled);
    //}
    //public static void ToggleSettingsMenu(bool enabled)
    //{
    //    DisplaySettingsMenu?.Invoke(enabled);
    //}
    //public static void ToggleGameOverMenu(bool enabled)
    //{
    //    DisplayGameOverMenu?.Invoke(enabled);
    //}

}
