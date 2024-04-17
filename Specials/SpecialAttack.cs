using UnityEngine;

public abstract class SpecialAttack : MonoBehaviour, ISpecialAttack
{

    [SerializeField] protected Sprite _sprite;
    public Sprite Sprite => _sprite;

    [Header("Setup")]
    [SerializeField] protected float _useCooldown = 20.0f;

    [Header("Debug")]
    [SerializeField] protected float _timer = 0f;
    [SerializeField] protected float _lastUsed = 0f;
    [SerializeField] protected float _endOfUse = 0f;
    [SerializeField] protected float _nextCanUseTarget = -100.0f;

    [SerializeField] protected ISpecialAttack.SpecialBehavior _mySpecialBehavior;
    public ISpecialAttack.SpecialBehavior MySpecialBehavior => _mySpecialBehavior;




    protected void OnEnable()
    {
        EnemyManager.OnNewWave += OnNewWave;
    }
    protected void OnDisable()
    {
        EnemyManager.OnNewWave -= OnNewWave;
    }

    protected void OnNewWave(int index)
    {
        _nextCanUseTarget = _timer;
        //Debug.Log("Special Cooldown rest on New Wave call");
    }



    // timer
    public virtual float CooldownProgress()
    {
        return (_timer - _endOfUse) / (_nextCanUseTarget - _endOfUse);
    }
    protected virtual void Update()
    {
        _timer = GameManager.Instance.IngameUnscaledTimer;
    }



    // ISpecialAttack
    public abstract void UseSpecial();
    //{
    //    if (!CanUseSpecial())
    //    {
    //        Debug.Log("Conditions for Special not met!");
    //        return;
    //    }

    //    _lastUsed = _thisLevelTimer;

    //    // these need override in EnterMode Specials
    //    _endOfUse = _thisLevelTimer;
    //    _nextCanUseTarget = _thisLevelTimer + _useCooldown;
    //}

    public abstract bool CanUseSpecial();
    //{
    //if (_thisLevelTimer > _nextCanUseTarget)
    //        return true;
    //    return false;
    //}

}
