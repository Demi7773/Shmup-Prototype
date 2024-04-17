using UnityEngine;
using UnityEngine.AI;

public class ZoomerBehavior : EnemyBehavior
{

    [Space(30)]
    [Header("Zoomer")]
    [SerializeField] protected NavMeshAgent _navmeshAgent;
    [Header("State")]
    [SerializeField] protected CombatState _state;
    public enum CombatState
    {
        Moving,
        Staggered,
        ChargingAttack,
        Attacking
    }

    [Header("Behavior Settings")]
    [SerializeField] protected float _attempAttackRange = 3.0f;
    [SerializeField] protected float _staggerAfterGetHit = 0.3f;
    [SerializeField] protected float _staggerAfterAttack = 0.3f;
    [SerializeField] protected float _chargeDuration = 0.1f;

    [Header("Attack Settings")]
    [SerializeField] protected AnimationCurve _attackMovementCurve;
    [SerializeField] protected float _attackDuration = 0.3f;
    [SerializeField] protected float _dashDistance = 5.0f;
    [SerializeField] protected float _damageRadius = 1.0f;
    [SerializeField] protected float _attackDamage = 5.0f;

    [Space(30)]
    [Header("Debug")]
    [SerializeField] protected Vector3 _dashStartPos;
    [SerializeField] protected Vector3 _dashEndPos;
    [SerializeField] protected Vector3 _currentDashDirection;
    [SerializeField] protected float _attackStartTime = -10.0f;
    [SerializeField] protected float _lastAttackEndTime = -10.0f;
    [SerializeField] protected float _staggerEndTime = -10.0f;
    [SerializeField] protected float _chargingEndTime = -10.0f;




   

    public override void Init(int level, Transform player)
    {
        _playerTransform = player;
        _level = level;
        SetStatsForLevel(_level);
        _hpDisplay.Init();
        gameObject.SetActive(false);
    }

    protected override void SetStatsForLevel(int level)
    {
        switch (level)
        {
            default:
                Debug.Log("Level parameter(int) invalid");
                SetLevel1Stats();
                break;
            case 1: 
                SetLevel1Stats();
                break;
            case 2: 
                SetLevel2Stats();
                break;
            case 3: 
                SetLevel3Stats();
                break;
        }

        Heal(_maxHP);
    }
    protected void SetLevel1Stats()
    {
        _minMoneyDrop = EnemyStatsPerLevel.ZoomerLvl1_MinMoney;
        _maxMoneyDrop = EnemyStatsPerLevel.ZoomerLvl1_MaxMoney;

        _maxHP = EnemyStatsPerLevel.ZoomerLvl1_MaxHP;
        _navmeshAgent.speed = EnemyStatsPerLevel.ZoomerLvl1_Speed;
        _attackDamage = EnemyStatsPerLevel.ZoomerLvl1_AttackDamage;
        _damageRadius = EnemyStatsPerLevel.ZoomerLvl1_DamageRadius;
    }
    protected void SetLevel2Stats()
    {
        _minMoneyDrop = EnemyStatsPerLevel.ZoomerLvl2_MinMoney;
        _maxMoneyDrop = EnemyStatsPerLevel.ZoomerLvl2_MaxMoney;

        _maxHP = EnemyStatsPerLevel.ZoomerLvl2_MaxHP;
        _navmeshAgent.speed = EnemyStatsPerLevel.ZoomerLvl2_Speed;
        _attackDamage = EnemyStatsPerLevel.ZoomerLvl2_AttackDamage;
        _damageRadius = EnemyStatsPerLevel.ZoomerLvl2_DamageRadius;
    }
    protected void SetLevel3Stats()
    {
        _minMoneyDrop = EnemyStatsPerLevel.ZoomerLvl3_MinMoney;
        _maxMoneyDrop = EnemyStatsPerLevel.ZoomerLvl3_MaxMoney;

        _maxHP = EnemyStatsPerLevel.ZoomerLvl3_MaxHP;
        _navmeshAgent.speed = EnemyStatsPerLevel.ZoomerLvl3_Speed;
        _attackDamage = EnemyStatsPerLevel.ZoomerLvl3_AttackDamageLvl3;
        _damageRadius = EnemyStatsPerLevel.ZoomerLvl3_DamageRadius;
    }



    public override void SpawnMeAt(Vector3 position)
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(position, out hit, 10.0f, 1))
        {
            //Debug.Log("Spawnpoint is on Navmesh");
            transform.position = hit.position;
        }
        else
        {
            //Debug.Log("SpawnPoint was outside of NavMesh, new position: " + hit.position);
            transform.position = hit.position;
        }

        _timer = 0.0f;
        gameObject.SetActive(true);
        _navmeshAgent.SetDestination(_playerTransform.position);
        _state = CombatState.Moving;
    }



    protected override void OnUpdateEnemyPathing()
    {
        if (_playerTransform == null)
            return;
        if (_navmeshAgent.isOnNavMesh)
            _navmeshAgent.SetDestination(_playerTransform.position);
    }



    public override void GetHit(float damage, Vector3 sourceDirection, float force)
    {
        base.GetHit(damage, sourceDirection, force);

        _staggerEndTime = _timer + _staggerAfterGetHit;
        _state = CombatState.Staggered;
    }



    protected void Update()
    {
        _timer += Time.deltaTime;

        switch (_state)
        {
            case CombatState.Moving:
                {
                    if (!IsPlayerInAttemptAttackRange())
                    {
                        break;
                    }

                    _chargingEndTime = _timer + _chargeDuration;
                    _navmeshAgent.isStopped = true;
                    _state = CombatState.ChargingAttack;
                    break;
                }

            case CombatState.ChargingAttack:
                {
                    if (IsStillChargingAttack())
                    {
                        transform.LookAt(_playerTransform.position, Vector3.up);
                        break;
                    }

                    _attackStartTime = _timer;
                    _lastAttackEndTime = _timer + _attackDuration;
                    SetAttackTrajectory();
                    _state = CombatState.Attacking;
                    break;
                }

            case CombatState.Attacking:
                {
                    if (IsStillAttacking())
                    {
                        HandleAttacking();
                        break;
                    }

                    _staggerEndTime = _lastAttackEndTime + _staggerAfterAttack;
                    _state = CombatState.Staggered;
                    //Debug.Log("End of AttackSequenceStep at " + _thisLevelTimer + " with position: " + transform.position);
                    break;
                }

            case CombatState.Staggered:
                {
                    if (IsStillStaggered())
                    {
                        break;
                    }

                    _state = CombatState.Moving;
                    _navmeshAgent.isStopped = false;

                    //Debug.Log("Staggered after attack");
                    break;
                }
        }
    }
    protected bool IsStillStaggered()
    { return _timer < _staggerEndTime; }
    protected bool IsStillChargingAttack()
    { return _timer < _chargingEndTime; }
    protected bool IsStillAttacking()
    { return _timer < _lastAttackEndTime; }
    protected bool IsPlayerInAttemptAttackRange()
    { return (Vector3.Distance(transform.position, _playerTransform.position) < _attempAttackRange); }

    protected void SetAttackTrajectory()
    {
        _dashStartPos = transform.position;
        _currentDashDirection = (_playerTransform.position - _dashStartPos).normalized;
        _dashEndPos = _dashStartPos + _currentDashDirection * _dashDistance;
    }

    protected void HandleAttacking()
    {
        float timeSinceStart = _timer - _attackStartTime;
        float completion = timeSinceStart / _attackDuration;
        float posModifier = _attackMovementCurve.Evaluate(completion);

        transform.position = Vector3.Lerp(_dashStartPos, _dashEndPos, posModifier);

        if (Vector3.Distance(transform.position, _playerTransform.position) < _damageRadius)
        {
            _playerTransform.GetComponent<PlayerHP>().GetHit(_attackDamage, _currentDashDirection, 1.0f);
        }
    }

}
