using System;
using UnityEngine;

public abstract class EnemyBehavior : MonoBehaviour, IHittable
{

    protected Transform _playerTransform;
    [Header("EnemyBehavior Dependencies and settings")]
    [SerializeField] protected EnemyHPUIAdvanced _hpDisplay;
    [SerializeField] protected MoneyPickup _moneyDrop;
    [SerializeField] protected EnemyType _type;
    [SerializeField] protected int _level = 1;
    [SerializeField] protected float _currentHP = 0.0f;
    [SerializeField] protected float _maxHP = 10.0f;
    [SerializeField] protected float _moveSpeed = 10.0f;
    [SerializeField] protected int _minMoneyDrop = 8;
    [SerializeField] protected int _maxMoneyDrop = 12;
    protected float _timer = 0.0f;
    public enum EnemyType
    {
        Zoomer,
        Boomer,
        Shooter
    }
    public EnemyType Type => _type;



    public static event Action<EnemyBehavior> EnemyDeath;
    public static event Action<int, Vector3> EnemyDropMoney;



    protected virtual void OnEnable()
    {
        SpawnerLevel2.OnUpdateEnemyPathing += OnUpdateEnemyPathing;
    }
    private void OnDisable()
    {
        SpawnerLevel2.OnUpdateEnemyPathing -= OnUpdateEnemyPathing;
    }
    protected abstract void OnUpdateEnemyPathing();



    // IHittable
    public virtual void GetHit(float damage, Vector3 sourceDirection, float force)
    {
        AdjustHP(-damage);
        _hpDisplay.DisplayFloatingNumber(FloatingNumber.Context.Damage, transform.position, damage);
    }
    public virtual void Heal(float amount)
    {
        AdjustHP(+amount);
        _hpDisplay.DisplayFloatingNumber(FloatingNumber.Context.Heal, transform.position, amount);
    }
    protected virtual void AdjustHP(float adjustment) 
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
    protected virtual void Death()
    {
        int rolledDrop = UnityEngine.Random.Range(_minMoneyDrop, _maxMoneyDrop + 1);
        EnemyDropMoney?.Invoke(rolledDrop, transform.position);
        EnemyDeath?.Invoke(this);
        gameObject.SetActive(false);
    }



    public abstract void Init(int level, Transform player);
    public abstract void SpawnMeAt(Vector3 position);

    protected abstract void SetStatsForLevel(int level);

    protected virtual void UpdateUI()
    {
        //_healthBar.UpdateHealthBar(_currentHP, _maxHP);
        _hpDisplay.UpdateHealthBar(_currentHP, _maxHP);
    }

}
