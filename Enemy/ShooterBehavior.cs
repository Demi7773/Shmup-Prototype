using UnityEngine;
using UnityEngine.AI;

public class ShooterBehavior : EnemyBehavior
{

    [Space(30)]
    [Header("Shooter")]
    [SerializeField] protected NavMeshAgent _navmeshAgent;
    [SerializeField] protected LayerMask _terrainLayers;
    [SerializeField] protected Transform _shootPos;
    [SerializeField] protected SimpleProjectile _projectilePrefab;
    [Header("State")]
    [SerializeField] protected CombatState _state;
    public enum CombatState
    {
        Moving,
        Staggered,
        ChargingAttack,
        Attacking,
        Reloading
    }
    [Header("Behavior Settings")]
    [SerializeField] protected float _staggerAfterGetHit = 0.5f;
    [Header("RunAway")]
    [SerializeField] protected float _runAwayDistance = 10.0f;
    [SerializeField] protected Vector3 _targetPos;
    [Header("Attack Settings")]
    [SerializeField] protected float _tryAttackRange = 15.0f;
    [SerializeField] protected float _chargeDuration = 0.3f;
    [SerializeField] protected float _reloadDuration = 0.3f;
    [SerializeField] protected float _reloadingMoveSpeed = 1.5f;
    [SerializeField] protected float _attackDuration = 0.2f;
    [Header("AttackCooldownTotal needs to be higher than projectile flight duration")]
    [SerializeField] protected float _attackCooldownTotal = 1.5f;
    [SerializeField] protected float _attackDamage = 5.0f;
    [SerializeField] protected float _aimOffset = 1.5f;
    [Space(30)]
    [Header("Debug")]
    [SerializeField] protected Vector3 _aimPos = Vector3.zero;
    [SerializeField] protected float _attackStartTime = -10.0f;
    [SerializeField] protected float _attackEndTime = -10.0f;
    [SerializeField] protected float _staggerEndTime = -10.0f;
    [SerializeField] protected float _chargingEndTime = -10.0f;
    [SerializeField] protected float _reloadingEndTime = -10.0f;
    [SerializeField] protected float _canShootAgainTime = -10.0f;
    [SerializeField] protected SimpleProjectile _projectileInstance;



    // From Base

    public override void Init(int level, Transform player)
    {
        _playerTransform = player;
        _level = level;
        SetStatsForLevel(_level);
        _hpDisplay.Init();
        gameObject.SetActive(false);

        InitProjectile();
    }

    protected void InitProjectile()
    {
        if (_projectileInstance == null)
        {
            _projectileInstance = Instantiate(_projectilePrefab);
        }
        _projectileInstance.gameObject.SetActive(false);
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
        _minMoneyDrop = EnemyStatsPerLevel.ShooterLvl1_MinMoney;
        _maxMoneyDrop = EnemyStatsPerLevel.ShooterLvl1_MaxMoney;

        _maxHP = EnemyStatsPerLevel.ShooterLvl1_MaxHP;
        _navmeshAgent.speed = EnemyStatsPerLevel.ShooterLvl1_Speed;
        _tryAttackRange = EnemyStatsPerLevel.ShooterLvl1_TryAttackRange;
        _chargeDuration = EnemyStatsPerLevel.ShooterLvl1_ChargeDuration;
        _reloadingMoveSpeed = EnemyStatsPerLevel.ShooterLvl1_ReloadingMoveSpeed;
        _reloadDuration = EnemyStatsPerLevel.ShooterLvl1_ReloadDuration;
        _attackCooldownTotal = EnemyStatsPerLevel.ShooterLvl1_AttackCooldownTotal;
        _attackDamage = EnemyStatsPerLevel.ShooterLvl1_AttackDamage;
        _aimOffset = EnemyStatsPerLevel.ShooterLvl1_AimOffset;
    }
    protected void SetLevel2Stats()
    {
        _minMoneyDrop = EnemyStatsPerLevel.ShooterLvl2_MinMoney;
        _maxMoneyDrop = EnemyStatsPerLevel.ShooterLvl2_MaxMoney;

        _maxHP = EnemyStatsPerLevel.ShooterLvl2_MaxHP;
        _navmeshAgent.speed = EnemyStatsPerLevel.ShooterLvl2_Speed;
        _tryAttackRange = EnemyStatsPerLevel.ShooterLvl2_TryAttackRange;
        _chargeDuration = EnemyStatsPerLevel.ShooterLvl2_ChargeDuration;
        _reloadingMoveSpeed = EnemyStatsPerLevel.ShooterLvl2_ReloadingMoveSpeed;
        _reloadDuration = EnemyStatsPerLevel.ShooterLvl2_ReloadDuration;
        _attackCooldownTotal = EnemyStatsPerLevel.ShooterLvl2_AttackCooldownTotal;
        _attackDamage = EnemyStatsPerLevel.ShooterLvl2_AttackDamage;
        _aimOffset = EnemyStatsPerLevel.ShooterLvl2_AimOffset;
    }
    protected void SetLevel3Stats()
    {
        _minMoneyDrop = EnemyStatsPerLevel.ShooterLvl3_MinMoney;
        _maxMoneyDrop = EnemyStatsPerLevel.ShooterLvl3_MaxMoney;

        _maxHP = EnemyStatsPerLevel.ShooterLvl3_MaxHP;
        _navmeshAgent.speed = EnemyStatsPerLevel.ShooterLvl3_Speed;
        _tryAttackRange = EnemyStatsPerLevel.ShooterLvl3_TryAttackRange;
        _chargeDuration = EnemyStatsPerLevel.ShooterLvl3_ChargeDuration;
        _reloadingMoveSpeed = EnemyStatsPerLevel.ShooterLvl3_ReloadingMoveSpeed;
        _reloadDuration = EnemyStatsPerLevel.ShooterLvl3_ReloadDuration;
        _attackCooldownTotal = EnemyStatsPerLevel.ShooterLvl3_AttackCooldownTotal;
        _attackDamage = EnemyStatsPerLevel.ShooterLvl3_AttackDamage;
        _aimOffset = EnemyStatsPerLevel.ShooterLvl3_AimOffset;
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
        //_nextUpdatePathTime = _timer + _timeBetweenPathAdjustment;
        gameObject.SetActive(true);
        _navmeshAgent.SetDestination(_playerTransform.position);
        _state = CombatState.Moving;
    }



    protected override void OnUpdateEnemyPathing()
    {
        if (_playerTransform == null)
            return;
        if (_navmeshAgent.isOnNavMesh)
        {
            Vector3 playerPos = _playerTransform.position;
            if (Vector3.Distance(transform.position, playerPos) < _runAwayDistance)
            {
                _targetPos = GetTargetPosAwayFromPlayer(playerPos);
            }
            else
            {
                _targetPos = _playerTransform.position;
            }
            _navmeshAgent.SetDestination(_targetPos);
        }
            
    }



    public override void GetHit(float damage, Vector3 sourceDirection, float force)
    {
        base.GetHit(damage, sourceDirection, force);

        _staggerEndTime = _timer + _staggerAfterGetHit;
        _state = CombatState.Staggered;
    }



    // Behavior
    protected void Update()
    {
        _timer += Time.deltaTime;

        switch (_state)
        {
            case CombatState.Moving:
                {
                    if (!(Vector3.Distance(transform.position, _playerTransform.position) < _tryAttackRange))
                        break;
                    if (!HasLOSToPlayer(_playerTransform.position))
                        break;
                    if (_timer < _canShootAgainTime)
                        break;

                    _chargingEndTime = _timer + _chargeDuration;
                    _navmeshAgent.isStopped = true;
                    _state = CombatState.ChargingAttack;
                    break;
                }

            case CombatState.ChargingAttack:
                {
                    if (_timer < _chargingEndTime)
                    {
                        transform.LookAt(_playerTransform.position, Vector3.up);
                        break;
                    }

                    _attackStartTime = _timer;
                    _attackEndTime = _timer + _attackDuration;

                    Vector3 shootPos = _shootPos.position;
                    float randomX = Random.Range(-_aimOffset, _aimOffset);
                    float randomZ = Random.Range(-_aimOffset, _aimOffset);
                    Vector3 aimPos = _playerTransform.position + new Vector3(randomX, 0.0f, randomZ);
                    //var projectile = Instantiate(_projectileInstance);
                    _projectileInstance.FireMe(shootPos, aimPos, _attackDamage);

                    _state = CombatState.Attacking;
                    break;
                }

            case CombatState.Attacking:
                {
                    if (_timer < _attackEndTime)
                    {
                        break;
                    }

                    _canShootAgainTime = _attackEndTime + _attackCooldownTotal;
                    _state = CombatState.Reloading;
                    _reloadingEndTime = _timer + _reloadDuration;
                    _navmeshAgent.speed = _reloadingMoveSpeed;
                    _navmeshAgent.isStopped = false;
                    break;
                }

            case CombatState.Staggered:
                {
                    if (_timer < _staggerEndTime)
                    {
                        break;
                    }

                    _state = CombatState.Moving;
                    _navmeshAgent.isStopped = false;
                    break;
                }
            case CombatState.Reloading:
                {
                    if (_timer < _reloadingEndTime)
                    {
                        break;
                    }
                    _state = CombatState.Moving;
                    _navmeshAgent.speed = _moveSpeed;
                    break;
                }
        }
    }








    protected bool HasLOSToPlayer(Vector3 playerPos)
    {
        Vector3 scanStartPos = transform.position;
        Vector3 towardsPlayer = playerPos - scanStartPos;
        if (Physics.Raycast(scanStartPos, towardsPlayer, _tryAttackRange, _terrainLayers))
        {
            return false;
        }
        return true;
    }

    protected Vector3 GetTargetPosAwayFromPlayer(Vector3 playerPos)
    {
        Vector3 awayFromPlayer = transform.position - playerPos;
        Vector3 targetDir = Quaternion.AngleAxis(Random.Range(-45.0f, 45.0f), Vector3.up) * awayFromPlayer;
        Vector3 targetPos = transform.position + (targetDir * 5.0f);
        return targetPos;
    }

}
