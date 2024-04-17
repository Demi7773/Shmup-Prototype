using System;
using UnityEngine;

public class PlayerHP : MonoBehaviour, IHittable
{

    //[SerializeField] private PlayerHPBar _hpBar;
    [SerializeField] private DodgeRoll _dodge;
    private GameManager _gm;

    [SerializeField] private float _currentHP = 0f;
    [SerializeField] private float _maxHP = 0f;

    //[SerializeField] private float _thisLevelTimer = 0.0f;
    [SerializeField] private float _iFrameDuration = 0.1f;
    [SerializeField] private float _vulnerableTimerTarget = -10.0f;


    //[SerializeField] private float _thisLevelTimer = 0.0f;
    private float timer => GameManager.Instance.IngameUnscaledTimer;


    public float CurrentHP => _currentHP;
    public float MaxHP => _maxHP;

    public static event Action<PlayerHP> OnPlayerHPChange;
    public static event Action<float> OnPlayerGetHit;





    private void OnEnable()
    {
        UIEvents.OnUpdateAllHUD += UIForLevelStart;
    }
    private void OnDisable()
    {
        UIEvents.OnUpdateAllHUD -= UIForLevelStart;
    }

    private void UIForLevelStart()
    {
        OnPlayerHPChange?.Invoke(this);
    }



    public void Init(GameManager gm, float maxHP, float currentHP)
    {
        _gm = gm;
        //Heal(_maxHP);
        _maxHP = maxHP;
        _currentHP = currentHP;
    }
    private void Start()
    {
        OnPlayerHPChange?.Invoke(this);
    }

    public void InvulnerabilityMode(float duration)
    {
        Debug.Log("Player Entered Invulnerability mode until " + _vulnerableTimerTarget);
        _vulnerableTimerTarget = timer + duration;
    }




    private void UpdateUI()
    {
        OnPlayerHPChange?.Invoke(this);
        //_hpBar.UpdateTargetHP(_currentHP, _maxHP);
    }


    public bool IsFullHP()
    {
        if (_currentHP >= _maxHP - 0.1f)
        {
            return true;
        }
        return false;
    }

    public void Heal(float amount)
    {
        AdjustHP(+amount);
    }

    public void GetHit(float damage, Vector3 sourceDirection, float force)
    {
        if (_dodge.IsDodging)
        {
            //Debug.Log("Dodging on GetHit");
            return;
        }

        if (timer < _vulnerableTimerTarget)
        {
            //Debug.Log("IFrame on GetHit");
            return;
        }

        _vulnerableTimerTarget = timer + _iFrameDuration;
        AdjustHP(-damage);
        OnPlayerGetHit?.Invoke(damage);
    }

    private void AdjustHP(float adjustment)
    {
        _currentHP += adjustment;

        if (_currentHP < 0.1f)
        {
            _currentHP = 0.0f;
            Death();
        }
        else if (_currentHP > _maxHP)
        {
            _currentHP = _maxHP;
        }

        UpdateUI();
    }

    private void Death()
    {
        Debug.Log("Player died!");
        _gm.GameOver();
        gameObject.SetActive(false);
    }

}
