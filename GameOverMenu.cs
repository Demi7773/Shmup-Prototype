using System;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{

    [SerializeField] private Button _restartBtn;
    [SerializeField] private Button _backToMainBtn;
    [SerializeField] private Button _quitBtn;

    public static event Action OnRetryPressed;
    public static event Action QuitToMainMenu;
    public static event Action QuitToDesktop;



    private void OnEnable()
    {
        _restartBtn.onClick.AddListener(RestartClicked);
        _backToMainBtn.onClick.AddListener(BackToMainClicked);
        _quitBtn.onClick.AddListener(QuitClicked);
    }
    private void OnDisable()
    {
        _restartBtn.onClick.RemoveAllListeners();
        _backToMainBtn.onClick.RemoveAllListeners();
        _quitBtn.onClick.RemoveAllListeners();
    }



    private void RestartClicked()
    {
        OnRetryPressed?.Invoke();
        gameObject.SetActive(false);
    }
    private void BackToMainClicked()
    {
        QuitToMainMenu?.Invoke();
    }
    private void QuitClicked()
    {
        QuitToDesktop?.Invoke();
    }

}
