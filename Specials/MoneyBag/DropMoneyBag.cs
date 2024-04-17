using UnityEngine;

public class DropMoneyBag : SpecialAttack
{

    protected NewPlayerController _controls;
    protected PlayerMoney _playerMoney;

    [Space(30)]
    [Header("DropMoneyBag")]
    [Header("Setup")]
    [SerializeField] protected MoneyBag _moneyBagPrefab;
    [SerializeField] protected LayerMask _terrainLayers;
    [SerializeField] protected int _useCost = 50;
    [SerializeField] protected float _spawnY = 30.0f;
    [Header("Debug")]
    [SerializeField] protected MoneyBag _moneyBagInstance;





    public virtual void PlayerMoneyReference(PlayerMoney playerMoney)
    {
        _playerMoney = playerMoney;
    }



    protected void Awake()
    {
        _controls = GetComponentInParent<NewPlayerController>();
        if (_moneyBagInstance == null )
        {
            _moneyBagInstance = Instantiate(_moneyBagPrefab);
        }
        _moneyBagInstance.gameObject.SetActive(false);
    }



    public override bool CanUseSpecial()
    {
        if (_playerMoney.Money >= _useCost && _timer > _nextCanUseTarget)
            return true;
        return false;
    }
    public override void UseSpecial()
    {
        _lastUsed = _timer;
        _endOfUse = _timer;
        _nextCanUseTarget = _timer + _useCooldown;
        _playerMoney.LoseMoney(_useCost);
        SetMoneyBagTrajectory();
    }



    protected virtual void SetMoneyBagTrajectory()
    {
        // add terrain check
        Vector3 endPos = _controls.AimPos;
        Vector3 startPos = new Vector3(endPos.x, _spawnY, endPos.z);

        _moneyBagInstance.Init(startPos, endPos);
    }

}
