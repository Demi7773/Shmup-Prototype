using ReworkedWeapons;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerUI : MonoBehaviour
{

    [SerializeField] private float _moveTowardsSpeed = 2f;

    [Header("Weapon")]
    [SerializeField] private TextMeshProUGUI _weaponNameText;
    [SerializeField] private Image _weaponImage;
    [SerializeField] private TextMeshProUGUI _ammoText;

    [Header("Special")]
    [SerializeField] private Image _specialImg;
    [SerializeField] private Image _specialCooldownOverlayImg;
    [SerializeField] private Image _specialInUseImg;
    [SerializeField] private Image _specialBarHolder;
    [SerializeField] private Image _specialBar;
    [SerializeField] private float _singleUseSpecialVisualDuration = 0.3f;

    [Header("ReloadBar")]
    [SerializeField] private GameObject _reloadUI;
    [SerializeField] private Image _reloadBar;

    [Header("Health")]
    [SerializeField] private Image _healthBarImage;
    private PlayerHP _playerHP;
    private float _targetHPFill = 1f;

    [Header("Energy")]
    [SerializeField] private Image _energyBar;
    private PlayerEnergy _playerEnergy;
    private float _targetEnergyFill = 1f;

    [Header("DodgeBar")]
    [SerializeField] private Image _dodgeImage1;
    [SerializeField] private Image _dodgeImage2;
    private DodgeRoll _playerDodge;

    [Header("Money")]
    [SerializeField] private TextMeshProUGUI _moneyTMP;

    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI _timerTMP;

    [Header("Announcments")]
    [SerializeField] private bool _isDisplayingMessage = false;
    private float _messageStartTime;
    //[SerializeField] private float _messageLerpingDuration = 1.0f;
    [SerializeField] private float _messageDisplayDuration = 5.0f;
    [SerializeField] private CanvasGroup _messagePanel;
    [SerializeField] private TextMeshProUGUI _messageTxt;
    [SerializeField] private float _messageStartY = 400.0f;
    [SerializeField] private float _messageEndY = -70.0f;
    [SerializeField] private float _messageStartAlpha = 0.0f;
    [SerializeField] private float _messageEndAlpha = 0.2f;
    [SerializeField] private AnimationCurve _messageLerpCurve;

    [Header("Tutorials")]
    [SerializeField] private List<Image> _tutorialImages = new List<Image>();




    private void OnEnable()
    {
        NewWeaponManager.OnSwapWeapon += SetNewWeaponUI;
        NewWeaponManager.OnWeaponStatusChange += UpdateWeaponUI;
        NewWeaponManager.ReloadIsHappening += SetReloadBar;
        NewWeaponManager.ReloadProgress += UpdateReloadBar;

        PlayerHP.OnPlayerHPChange += UpdateHPUI;
        PlayerEnergy.OnPlayerEnergyChange += UpdateEnergyUI;
        DodgeRoll.PlayerDodgeBarProgress += UpdateDodgeUI;
        PlayerMoney.OnPlayerMoneyChange += UpdateMoneyUI;


        //PlayerEvents.OnPlayerUsesSpecial += OnSpecialUsed;
        NewWeaponManager.UsedSpecial += OnSpecialUsed;

        EnterMode.OnExitMode += SpecialModeEndsUI;

        UIEvents.OnNewMessage += DisplayMessage;

        EnemyManager.OnWaveTimerUpdate += UpdateTimerUI;
    }
    private void OnDisable()
    {
        NewWeaponManager.OnSwapWeapon -= SetNewWeaponUI;
        NewWeaponManager.OnWeaponStatusChange -= UpdateWeaponUI;
        NewWeaponManager.ReloadIsHappening -= SetReloadBar;
        NewWeaponManager.ReloadProgress -= UpdateReloadBar;

        PlayerHP.OnPlayerHPChange -= UpdateHPUI;
        PlayerEnergy.OnPlayerEnergyChange -= UpdateEnergyUI;
        DodgeRoll.PlayerDodgeBarProgress -= UpdateDodgeUI;
        PlayerMoney.OnPlayerMoneyChange -= UpdateMoneyUI;

        //PlayerEvents.OnPlayerUsesSpecial -= OnSpecialUsed;
        NewWeaponManager.UsedSpecial -= OnSpecialUsed;

        EnterMode.OnExitMode -= SpecialModeEndsUI;

        UIEvents.OnNewMessage -= DisplayMessage;

        EnemyManager.OnWaveTimerUpdate += UpdateTimerUI;
    }




    // Timer UI
    private void UpdateTimerUI(float timer)
    {
        TimeSpan timespan = TimeSpan.FromSeconds(timer);
        _timerTMP.text = string.Format("{0:00}:{1:00}", timespan.Minutes, timespan.Seconds);
    }



    // Message UI
    private void DisplayMessage(string message)
    {
        StartCoroutine(TryToDisplayMessage(message));
    }
    private IEnumerator TryToDisplayMessage(string message)
    {
        while (_isDisplayingMessage)
        {
            //Debug.Log("Already displaying message, waiting for sequence to finish");
            yield return null;
            if (!_isDisplayingMessage)
                break;
        }

        StartCoroutine(DisplayMessageSequence(message));
    }
    private IEnumerator DisplayMessageSequence(string message)
    {
        _messageTxt.text = message;
        _isDisplayingMessage = true;
        _messageStartTime = Time.unscaledTime;

        while (_isDisplayingMessage)
        {
            float progress = (Time.unscaledTime - _messageStartTime) / _messageDisplayDuration;
            float readCurve = _messageLerpCurve.Evaluate(progress);

            Vector2 newPos = new Vector2(960.0f/*startX*/, Mathf.Lerp(_messageStartY, _messageEndY, readCurve));
            float alpha = Mathf.Lerp(_messageStartAlpha, _messageEndAlpha, readCurve);
            _messagePanel.transform.position = newPos;
            _messagePanel.alpha = alpha;
            if (progress >= 1.0f)
                break;
            yield return null;
        }
        _isDisplayingMessage = false;
    }







    // Weapon and Special UI
    private void SetNewWeaponUI(Weapon weapon)
    {
        _weaponNameText.text = weapon.Name;
        _weaponImage.sprite = weapon.Sprite;
        _specialImg.sprite = weapon.SpecialSprite;
    }
    private void UpdateWeaponUI(Weapon weapon)
    {
        _ammoText.SetText(weapon.CurrentAmmo() + " / " + weapon.MaxAmmo());
        SetCooldownFill(weapon.MySpecial.CooldownProgress());
        SetModeBarFill(weapon.PlayerWeapons);
    }


    // Reload
    public void SetReloadBar(bool onOff)
    {
        _reloadBar.fillAmount = 0f;
        _reloadUI.SetActive(onOff);
    }
    public void UpdateReloadBar(float ratio)
    {
        _reloadBar.fillAmount = ratio;
    }


    // Special
    private void OnSpecialUsed(SpecialAttack special)
    {
        ISpecialAttack.SpecialBehavior typeOfSpecial = special.MySpecialBehavior;
        switch (typeOfSpecial)
        {
            default:
                Debug.Log("ISpecialAttack.SpecialBehavior not recognized");
                SpecialUsedShortSequence(special);
                break;
            case ISpecialAttack.SpecialBehavior.SingleUse:
                SpecialUsedShortSequence(special);
                break;

            case ISpecialAttack.SpecialBehavior.Mode:
                SpecialModeStartUI(special);
                break;
        }

    }
    private void SpecialUsedShortSequence(SpecialAttack special)
    {
        StartCoroutine(SpecialUIBurst(special));
    }
    private IEnumerator SpecialUIBurst(SpecialAttack special)
    {
        ToggleSpecialInUseUI(true);

        float endOfSequence = GameManager.Instance.IngameUnscaledTimer + _singleUseSpecialVisualDuration;
        while (GameManager.Instance.IngameUnscaledTimer < endOfSequence)
        {
            yield return null;
            if (GameManager.Instance.IngameUnscaledTimer > endOfSequence)
                break;
        }

        ToggleSpecialInUseUI(false);
        ToggleCooldownUI(true);
    }

    private void SpecialModeStartUI(SpecialAttack special)
    {
        ToggleSpecialInUseUI(true);
    }

    private void SetModeBarFill(NewWeaponManager weapons)
    {
        float ratio = weapons.ThisModeDurationRatio();
        float inverse = 1f - ratio;
        _specialBar.fillAmount = inverse;
    }

    private void SpecialModeEndsUI(SpecialAttack special)
    {
        ToggleSpecialInUseUI(false);
        ToggleCooldownUI(true);
    }

    private void ToggleSpecialInUseUI(bool enable)
    {
        _specialInUseImg.gameObject.SetActive(enable);
        _specialBarHolder.gameObject.SetActive(enable);
        //Debug.Log("SpecialUI toggled " + enable);
    }

    private void ToggleCooldownUI(bool enable)
    {
        _specialCooldownOverlayImg.gameObject.SetActive(enable);
    }
    
    private void SetCooldownFill(float progress)
    {
        if (progress >= 1.0f)
        {
            ToggleCooldownUI(false);
            return;
        }
        _specialCooldownOverlayImg.fillAmount = progress;
    }



        // PlayerUI
    private void UpdateHPUI(PlayerHP hp)
    {
        _playerHP = hp;
        _targetHPFill = _playerHP.CurrentHP / _playerHP.MaxHP;
    }

    private void UpdateEnergyUI(PlayerEnergy energy)
    {
        _playerEnergy = energy;
        _targetEnergyFill = _playerEnergy.CurrentEnergy / _playerEnergy.MaxEnergy;
    }

    private void UpdateDodgeUI(DodgeRoll dodge)
    {
        _playerDodge = dodge;
        _dodgeImage1.fillAmount = _playerDodge.Bar1Fill;
        _dodgeImage2.fillAmount = _playerDodge.Bar2Fill;
    }

    private void UpdateMoneyUI(PlayerMoney money)
    {
        _moneyTMP.text = "$" + money.Money;
    }

    



    private void Update()
    {
        if (_playerHP == null)
            _targetHPFill = 0;
        _healthBarImage.fillAmount = Mathf.MoveTowards(_healthBarImage.fillAmount, _targetHPFill, _moveTowardsSpeed * Time.deltaTime);

        if (_playerEnergy == null)
            _targetEnergyFill = 0;
        _energyBar.fillAmount = Mathf.MoveTowards(_energyBar.fillAmount, _targetEnergyFill, _moveTowardsSpeed * Time.deltaTime);
    }



    

}
