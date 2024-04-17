using UnityEngine;

public class TrainingDummy : MonoBehaviour, IHittable
{
    //[SerializeField] private FloatingHPBar _healthBar;
    [SerializeField] private EnemyHPUIAdvanced _numbersUI;

    [Header("HP")]
    [SerializeField] private float _currentHP = 0.0f;
    [SerializeField] private float _maxHP = 1000.0f;



    private void Awake()
    {
        _numbersUI.Init();
    }

    public void GetHit(float damage, Vector3 sourceDirection, float force)
    {
        AdjustHP(-damage);
        _numbersUI.DisplayFloatingNumber(FloatingNumber.Context.Damage, transform.position, damage);
    }
    public void Heal(float amount)
    {
        AdjustHP(+amount);
        _numbersUI.DisplayFloatingNumber(FloatingNumber.Context.Heal, transform.position, amount);
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
        UpdateUIOnHPChange();
    }

    private void Death()
    {
        gameObject.SetActive(false);
    }



    protected virtual void UpdateUIOnHPChange()
    {
        _numbersUI.UpdateHealthBar(_currentHP, _maxHP);
        //_healthBar.UpdateHealthBar(_currentHP, _maxHP);
    }
}
