using UnityEngine;

public class GuidedProjectile : MonoBehaviour
{

    [Space(20)]
    [Header("Debug - set on init and through behavior")]
    [SerializeField] private GuidedProjectilesController _controller;

    [Header("Movement")]
    [SerializeField] private bool _isMoving = false;
    [SerializeField] private Vector3 _startPos = Vector3.zero;
    [SerializeField] private Vector3 _endPos = Vector3.zero;
    [SerializeField] private MovePattern _movePattern = MovePattern.Default;

    public enum MovePattern
    {
        Default,
        SpeedUp,
        SlowDown
    }

    [Header("Timing")]
    [SerializeField] private float _duration = 0.5f;
    [SerializeField] private float _startTime = 0.0f;
    [SerializeField] private float _endTime = 0.0f;
    [SerializeField] private float _progress = 0.0f;
    private float _timer => GameManager.Instance.IngameTimer;

    [Header("BaseDamage")]
    [SerializeField] private bool _despawnOnHit = true;
    [SerializeField] private LayerMask _hitLayers;
    [SerializeField] private float _hitRadius = 1.0f;
    [SerializeField] private float _damage = 10.0f;





        // Sets the trajectory and required interaction settings
    public void Init(GuidedProjectilesController controller, Vector3 startPos, Vector3 endPos, float duration, MovePattern movePattern, 
                    bool despawnOnHit , LayerMask hitLayers, float hitRadius, float damage)
    {
        _controller = controller;
        _startPos = startPos;
        _endPos = endPos;
        _duration = duration;
        _movePattern = movePattern;
        _despawnOnHit = despawnOnHit;
        _hitLayers = hitLayers;
        _hitRadius = hitRadius;
        _damage = damage;

        _startTime = _timer;
        _endTime = _startTime + _duration;

        _isMoving = true;
        gameObject.SetActive(true);
    }

        // Progress through trajectory and check hits
    private void Update()
    {
        if (!_isMoving)
            return;

        _progress = (_timer - _startTime) / (_endTime - _startTime);
        if (_progress >= 1.0f)
        {
            //Debug.Log("Progress is >= 1 so end of trajectory reached. Despawning");
            Despawn();
        }

        float progressInCurveContext = _controller.MoveProgressionCurve.Evaluate(_progress);
        Vector3 posInTrajectory = Vector3.Lerp(_startPos, _endPos, progressInCurveContext);
        CheckForHits(posInTrajectory);
        transform.position = posInTrajectory;
    }



        // HitCheck
    private void CheckForHits(Vector3 scanPos)
    {
        Collider[] hits = Physics.OverlapSphere(scanPos, _hitRadius, _hitLayers);

        if (hits.Length < 1)
        {
            //Debug.Log("No hits on scan");
            return;
        }

        Debug.Log("Scan hit " + hits.Length + " targets in layers");
        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent<IHittable>(out IHittable hittable))
            {
                hittable.GetHit(_damage, transform.position, 1.0f);

                if (_despawnOnHit)
                {
                    Debug.Log("Scan hit IHittable " + hittable + ", did " + _damage + "dmg and is set to Despawn on hit, Despawning");
                    Despawn();
                    break;
                }

                Debug.Log("Scan hit IHittable " + hittable + ", did " + _damage + "dmg. Continuing scans");
            }

            //Debug.Log("Scan hit Target in LayerMask with no IHittable");
        }

        if (_despawnOnHit)
        {
            Debug.Log("Scan hit with Despawn on hit, no IHittables were found, Despawning");
            Despawn();
        }
    }
    private void Despawn()
    {
        _controller.DespawnAndReturnToQueue(this);
    }

}
