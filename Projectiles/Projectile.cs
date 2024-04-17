using UnityEngine;

public class Projectile : MonoBehaviour
{

    [SerializeField] protected Rigidbody _rb;
    protected ProjectilePool _pool;
    //[SerializeField] protected ProjectileStats _stats;
    //[SerializeField] protected LayerMask _hittableLayers;

    [Header("Debug")]
    [SerializeField] protected float _damage = 10.0f;
    [SerializeField] protected float _speed = 100.0f;
    [SerializeField] protected float _despawnAfter = 1.0f;
    [SerializeField] protected float _despawnAt = 0.0f;
    //[SerializeField] protected float _thisLevelTimer = 0.0f;

    public float Damage => _damage;




    public void SetObjectPoolReference(ProjectilePool pool)
    {
        _pool = pool;
    }




    //protected void OnEnable()
    //{
    //    _speed = _stats.Speed;
    //    _despawnTimer = _stats.DespawnTimer;
    //}

    protected void OnDisable()
    {
        _rb.velocity = Vector3.zero;
    }

    public virtual void Init(Vector3 direction, float dmg)
    {
        _damage = dmg;
        _rb.velocity = direction * _speed;
        _despawnAt = GameManager.Instance.IngameTimer + _despawnAfter;
        //Debug.Log("Projectile SpawnMeAt");
    }

    protected virtual void Update()
    {
        if (GameManager.Instance.IngameTimer > _despawnAt)
        {
            Despawn();
        }
    }



    protected virtual void OnTriggerEnter(Collider other)
    {
        var hittable = other.GetComponent<IHittable>();
        if (hittable == null)
        {
            Debug.Log("No IHittable on hit: " + other.name);
            Despawn();
            return;
        }

        hittable.GetHit(_damage, _rb.velocity, _rb.velocity.magnitude);
        //Debug.Log("IHittable hit: " + other.name);
        if (hittable is Smashable)
        {
            return;
        }

        Despawn();

    }

    protected virtual void Despawn()
    {
        _pool.AddObjectToPool(this);
    }

}
