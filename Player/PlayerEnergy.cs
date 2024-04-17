using System;
using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{

    [SerializeField] private float _currentEnergy = 0.0f;
    [SerializeField] private float _maxEnergy = 30.0f;
    public float CurrentEnergy => _currentEnergy;
    public float MaxEnergy => _maxEnergy;

    [SerializeField] private float _energyRegenPerSec = 1.0f;

    //[SerializeField] private Image _energyBar;


    public static event Action<PlayerEnergy> OnPlayerEnergyChange;



    private void OnEnable()
    {
        UIEvents.OnUpdateAllHUD += UpdateUI;
    }
    private void OnDisable()
    {
        UIEvents.OnUpdateAllHUD -= UpdateUI;
    }


    public void Init(float maxEnergy)
    {
        _maxEnergy = maxEnergy;
        _currentEnergy = _maxEnergy;
        UpdateUI();
    }



    private void Awake()
    {
        _currentEnergy = _maxEnergy;
        UpdateUI();
    }
    private void Update()
    {
        RegenTick();
        UpdateUI();
    }
    private void RegenTick()
    {
        float regenAmount = _energyRegenPerSec * Time.deltaTime;
        GainEnergy(regenAmount);
    }



    public bool HasEnoughEnergyFor(float amount)
    {
        if (_currentEnergy >= amount)
        {
            return true;
        }
        return false;
    }

    public void GainEnergy(float amount)
    {
        AdjustEnergyAmount(amount);
    }
    public void LoseEnergy(float amount)
    {
        AdjustEnergyAmount(-amount);
    }

    private void AdjustEnergyAmount(float adjustment)
    {
        float newAmount = _currentEnergy + adjustment;

        _currentEnergy = Mathf.Clamp(newAmount, 0.0f, _maxEnergy);
        //UpdateUIOnHPChange();
    }

    private void UpdateUI()
    {
        OnPlayerEnergyChange?.Invoke(this);
        //_energyBar.fillAmount = _currentEnergy / _maxEnergy;
        //_energyBar.fillAmount = Mathf.MoveTowards(_energyBar.fillAmount, _currentEnergy / _maxEnergy, 1f * Time.deltaTime);
    }

}
