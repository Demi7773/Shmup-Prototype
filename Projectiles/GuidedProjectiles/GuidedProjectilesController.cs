using System.Collections.Generic;
using UnityEngine;
using static GuidedProjectile;

public class GuidedProjectilesController : MonoBehaviour
{

    [Header("Pool settings")]
    [SerializeField] protected Transform _poolParentPrefab;
    [SerializeField] protected Transform _poolParentInstance;
    [SerializeField] protected GuidedProjectile _guidedProjectilePrefab;
    [SerializeField] protected int _poolSize = 100;
    [SerializeField] protected Queue<GuidedProjectile> _pool = new Queue<GuidedProjectile>();

    [Header("Projectile settings")]
    [SerializeField] protected bool _projectilesDespawnOnHit = true;
    [SerializeField] protected LayerMask _hitLayers;
    [SerializeField] protected float _flyDuration = 1.0f;
    [SerializeField] protected MovePattern _movePattern;
    [SerializeField] protected float _hitRadius = 0.5f;
    [SerializeField] protected float _damage = 10.0f;

    protected AnimationCurve _moveProgressionCurve;
    public AnimationCurve MoveProgressionCurve => _moveProgressionCurve;

    [SerializeField] protected AnimationCurve _straightAnimationCurve;
    [SerializeField] protected AnimationCurve _speedUpAnimationCurve;
    [SerializeField] protected AnimationCurve _slowDownAnimationCurve;




    
    public virtual void SetProjetileMovementType(MovePattern movePattern)
    {
        _movePattern = movePattern;

        switch (movePattern)
        {
            case MovePattern.Default:
                _moveProgressionCurve = _straightAnimationCurve;
                break;
            case MovePattern.SpeedUp:
                _moveProgressionCurve = _speedUpAnimationCurve;
                break;
            case MovePattern.SlowDown:
                _moveProgressionCurve = _slowDownAnimationCurve;
                break;

            default:
                _moveProgressionCurve = _straightAnimationCurve;
                break;
        }

        Debug.Log("Projectile MovementCurve on Controller set to " + movePattern);
    }



    public virtual void Fire()
    {

    }


    public virtual void Init()
    {
        _pool.Clear();
        InitPool(_poolSize);
        SetProjetileMovementType(_movePattern);
    }

    protected virtual void InitPool(int poolSize)
    {
        if (_poolParentInstance == null)
        {
            _poolParentInstance = Instantiate(_poolParentPrefab);
        }

        for (int i = 0; i < poolSize; i++)
        {
            var spawn = Instantiate(_guidedProjectilePrefab, _poolParentInstance);
            DespawnAndReturnToQueue(spawn);
        }
    }

    protected virtual void DeployProjectileFromQueue(Vector3 startPos, Vector3 endPos)
    {
        if (_pool.Count < 10)
        {
            Debug.Log("Expanding pool");
            InitPool(_poolSize);
        }
        var guidedProjectile = _pool.Dequeue();
        guidedProjectile.Init(this, startPos, endPos, _flyDuration, _movePattern, _projectilesDespawnOnHit, _hitLayers, _hitRadius, _damage);
    }

    public virtual void DespawnAndReturnToQueue(GuidedProjectile projectile)
    {
        projectile.gameObject.SetActive(false);
        _pool.Enqueue(projectile);
    }

}
