using UnityEngine;
using UnityEngine.VFX;

public class ProjectileExplosive : MonoBehaviour
{

    //[SerializeField] protected Rigidbody _rb;
    [SerializeField] protected float _speed = 100.0f;

    [SerializeField] protected float _timer = 0.0f;
    [SerializeField] protected float _despawnTimer = 5.0f;

    [SerializeField] protected float _damage = 10.0f;
    public float Damage => _damage;




    [SerializeField] protected VisualEffect _explosionVFX;
    [SerializeField] protected LayerMask _explosionLayers;
    // [SerializeField] protected GameObject _aimIndicator;

    [SerializeField] protected float _explosionRadius = 5.0f;
    [SerializeField] protected float _explosionDamage = 10.0f;
    [SerializeField] protected float _explosionForce = 10.0f;

    [SerializeField] protected float _flightDuration = 1.0f;

    [Header("Debug")]
    [SerializeField] protected float _interpolateProgress = 0.0f;
    [SerializeField] protected Vector3 _startPos = Vector3.zero;
    [SerializeField] protected Vector3 _middlePos = Vector3.zero;
    [SerializeField] protected Vector3 _endPos = Vector3.zero;
    [SerializeField] protected Vector3 _ABPos = Vector3.zero;
    [SerializeField] protected Vector3 _BCPos = Vector3.zero;
    [SerializeField] protected Vector3 _ABCPos = Vector3.zero;



    //[SerializeField] protected AnimationCurve _xCurve;
    //[SerializeField] protected AnimationCurve _yCurve;
    //[SerializeField] protected AnimationCurve _zCurve;



    public virtual void Init(Vector3 targetPos, float explosionRadius, float damage)
    {

        _damage = damage;
        _startPos = transform.position;
        _endPos = targetPos;
        _middlePos = Vector3.Lerp(_startPos, _endPos, 0.5f);
        float distance = Vector3.Distance(_startPos, _endPos);
        float maxDistance = 7.0f;
        distance = Mathf.Clamp(distance, 1.0f, maxDistance);
        float arcHeight = maxDistance * (1f + (1f / distance));
        _middlePos.y = arcHeight;
        _interpolateProgress = 0.0f;

        //Debug.Log("Trajectory Points set, Start: " + _startPos + ", Middle: " + _middlePos + ", End: " + _endPos);

    }


    protected virtual void Update()
    {
        if (_startPos ==  _endPos)
        {
            //Debug.Log("Start point same as end point");
            return;
        }

        _interpolateProgress += Time.deltaTime;
        
        if (_interpolateProgress > 1.0f)
        {
            _interpolateProgress = 1.0f;
        }

        transform.position = LerpThroughPoints(_interpolateProgress);

        if (_interpolateProgress > 0.99f)
        {
            Debug.Log("Trajectory reached end with no hits, exploding");
            Explode(transform.position);
        }

    }

    protected Vector3 LerpThroughPoints(float t)
    {
        _ABPos = Vector3.Lerp(_startPos, _middlePos, t);
        _BCPos = Vector3.Lerp(_middlePos, _endPos, t);
        _ABCPos = Vector3.Lerp(_ABPos, _BCPos, t);
        return _ABCPos;
    }


    protected virtual void OnTriggerEnter(Collider other)
    {
        var hittable = other.GetComponent<IHittable>();
        if (hittable == null)
        {
            Debug.Log("No IHittable on hit!");
            Explode(transform.position);
            return;
        }

        if (hittable is Smashable)
        {
            hittable.GetHit(_damage, transform.forward, 1.0f);
            return;
        }

        Explode(transform.position);
        hittable.GetHit(_damage, transform.forward, 1.0f);

    }

    protected void Explode(Vector3 explosionCenter)
    {

        Collider[] hits = Physics.OverlapSphere(explosionCenter, _explosionRadius, _explosionLayers);
        foreach (var hit in hits)
        {
            IHittable target = hit.GetComponent<IHittable>();
            if (target == null)
            {
                continue;
            }
            //Debug.Log("Explosion hit " + target);
            target.GetHit(_explosionDamage, (hit.transform.position - explosionCenter).normalized, _explosionForce);
        }
        //VFXEvents.InvokeExplosion(explosionCenter, _explosionRadius);
        Instantiate(_explosionVFX, explosionCenter, Quaternion.identity);

        Despawn();

    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }

    protected virtual void Despawn()
    {
        Destroy(gameObject);
    }

}
