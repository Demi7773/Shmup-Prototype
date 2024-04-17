using UnityEngine;
using UnityEngine.AI;

public class BoomerBehavior : EnemyBehavior
{

    [Space(30)]
    [Header("Boomer")]
    [SerializeField] protected NavMeshAgent _navmeshAgent;
    [SerializeField] protected ExplosionHelper _explosion;
    [SerializeField] protected LayerMask _explosionLayers;
    // [SerializeField] protected GameObject _aimIndicator;
    [Header("State")]
    [SerializeField] protected CombatState _state;
    public enum CombatState
    {
        Moving,
        Staggered,
        ChargingAttack,
        Attacking
    }
    [Header("Stats")]
    [SerializeField] protected float _tryAttackRange = 3.0f;
    [SerializeField] protected float _staggerAfterGetHit = 0.3f;
    [SerializeField] protected float _chargeDuration = 0.3f;
    [SerializeField] protected float _explosionRadius = 5.0f;
    [SerializeField] protected float _explosionDamage = 10.0f;
    [SerializeField] protected float _explosionForce = 10.0f;
    [SerializeField] protected float _explosionDuration = 0.5f;
    [Header("Debug")]
    [SerializeField] protected float _attackStartTime = -10.0f;
    [SerializeField] protected float _staggerEndTime = -10.0f;
    [SerializeField] protected float _chargingEndTime = -10.0f;
    [SerializeField] protected float _explosionTime = -10.0f;





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
        _minMoneyDrop = EnemyStatsPerLevel.BoomerLvl1_MinMoney;
        _maxMoneyDrop = EnemyStatsPerLevel.BoomerLvl1_MaxMoney;

        _maxHP = EnemyStatsPerLevel.BoomerLvl1_MaxHP;
        _navmeshAgent.speed = EnemyStatsPerLevel.BoomerLvl1_Speed;
        _tryAttackRange = EnemyStatsPerLevel.BoomerLvl1_TryAttackRange;
        _chargeDuration = EnemyStatsPerLevel.BoomerLvl1_ChargeDuration;
        _explosionRadius = EnemyStatsPerLevel.BoomerLvl1_ExplosionRadius;
        _explosionDamage = EnemyStatsPerLevel.BoomerLvl1_ExplosionDmg;
    }
    protected void SetLevel2Stats()
    {
        _minMoneyDrop = EnemyStatsPerLevel.BoomerLvl2_MinMoney;
        _maxMoneyDrop = EnemyStatsPerLevel.BoomerLvl2_MaxMoney;

        _maxHP = EnemyStatsPerLevel.BoomerLvl2_MaxHP;
        _navmeshAgent.speed = EnemyStatsPerLevel.BoomerLvl2_Speed;
        _tryAttackRange = EnemyStatsPerLevel.BoomerLvl2_TryAttackRange;
        _chargeDuration = EnemyStatsPerLevel.BoomerLvl2_ChargeDuration;
        _explosionRadius = EnemyStatsPerLevel.BoomerLvl2_ExplosionRadius;
        _explosionDamage = EnemyStatsPerLevel.BoomerLvl2_ExplosionDmg;
    }
    protected void SetLevel3Stats()
    {
        _minMoneyDrop = EnemyStatsPerLevel.BoomerLvl3_MinMoney;
        _maxMoneyDrop = EnemyStatsPerLevel.BoomerLvl3_MaxMoney;

        _maxHP = EnemyStatsPerLevel.BoomerLvl3_MaxHP;
        _navmeshAgent.speed = EnemyStatsPerLevel.BoomerLvl3_Speed;
        _tryAttackRange = EnemyStatsPerLevel.BoomerLvl3_TryAttackRange;
        _chargeDuration = EnemyStatsPerLevel.BoomerLvl3_ChargeDuration;
        _explosionRadius = EnemyStatsPerLevel.BoomerLvl3_ExplosionRadius;
        _explosionDamage = EnemyStatsPerLevel.BoomerLvl3_ExplosionDmg;
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
            Debug.Log("SpawnPoint was outside of NavMesh, new position: " + hit.position);
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

        if (_state == CombatState.ChargingAttack)
        {
            return;
        }
        if (_state == CombatState.Attacking)
        {
            return;
        }

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
                if (!(Vector3.Distance(transform.position, _playerTransform.position) < _tryAttackRange))
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
                if (_timer < _chargingEndTime)
                {
                    break;
                }

                _attackStartTime = _timer;
                _explosionTime = _timer + _explosionDuration;
                _state = CombatState.Attacking;
                break;
            }

            case CombatState.Attacking:
            {
                if (_timer < _explosionTime)
                { 
                    break;
                }

                Death();
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

                //Debug.Log("Staggered after attack");
                break;
            }
        }

    }


    protected override void Death()
    {
        var boom = Instantiate(_explosion, transform.position, Quaternion.identity);
        //Debug.Log("Boom");

        base.Death();
        boom.Init(_explosionLayers, _explosionDuration, _explosionRadius, _explosionDamage, _explosionForce);
        //VFXEvents.InvokeExplosion(explosionCenter, _explosionRadius);
    }

}
