using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool: MonoBehaviour
{

    [Header("MainPool")]
    [SerializeField] protected Transform _poolParentPrefab;
    [SerializeField] protected Transform _poolParent;
    [SerializeField] protected Queue<Projectile> _projectilesQueue = new Queue<Projectile>();
    [SerializeField] protected int _checkIfRunningEmptyThreshold = 10;
    [SerializeField] protected int _refillAmount = 100;

    [Header("Secondary pool for FractalProjectiles")]
    [SerializeField] protected Transform _pool2Parent;
    [SerializeField] protected ProjectilePool _secondaryPool;
    public ProjectilePool SecondaryPool => _secondaryPool;




    // Pool Behavior
    public virtual void AddObjectToPool(Projectile poolObject)
    {
        poolObject.gameObject.SetActive(false);
        _projectilesQueue.Enqueue(poolObject);
    }
    public virtual Projectile GetObjectFromPool()
    {
        Projectile objectFromPool = _projectilesQueue.Dequeue();

        if (_projectilesQueue.Count >= _checkIfRunningEmptyThreshold)
        {
            return objectFromPool;
        }
        else
        {
            Debug.Log("Pool running empty, refilling");

            for (int i = 0; i < _refillAmount; i++)
            {
                InitializePool(objectFromPool, _refillAmount);
            }

            return objectFromPool;
        }
    }



    // Initialization
    public virtual void InitializePool(Projectile objectPrefab, int poolSize)
    {
        if (_poolParent == null)
        {
            _poolParent = new GameObject().transform;       /* Instantiate(_poolParentPrefab);*/
        }
        for (int i = 0; i < poolSize; i++)
        {
            Projectile objectInstance = Instantiate(objectPrefab, _poolParent);
            objectInstance.SetObjectPoolReference(this);
            AddObjectToPool(objectInstance);
        }
        //Debug.Log(this.name + " expanded pool by " + poolSize);
    }



    // Secondary Pool
    public virtual void InitSecondaryPool(Projectile objectPrefab, int poolSize)
    {
        if (_poolParent == null)
        {
            _pool2Parent = _poolParent = new GameObject().transform;     /*Instantiate(_poolParentPrefab);*/
        }
        for (int i = 0; i < poolSize; i++)
        {
            Projectile objectInstance = Instantiate(objectPrefab, _pool2Parent);
            objectInstance.SetObjectPoolReference(this);
            AddObjectToSecondaryPool(objectInstance);
        }
        //Debug.Log(this.gameObject.transform.parent.name + " expanded secondary pool by " + poolSize);
    }
    public virtual void AddObjectToSecondaryPool(Projectile poolObject)
    {
        _secondaryPool.AddObjectToPool(poolObject);
    }
    public virtual Projectile GetObjectFromSecondaryPool()
    {
        return _secondaryPool.GetObjectFromPool();
    }

}
