using UnityEngine;

public class MoneyBag : MonoBehaviour
{

    [SerializeField] private float _travelDuration = 1.0f;
    [SerializeField] private float _remainOnGroundTime = 0.5f;
    [SerializeField] private LayerMask _hitLayers;
    [SerializeField] private float _hitRadius = 3.0f;
    [SerializeField] private float _damage = 50.0f;
    [SerializeField] private AnimationCurve _curve;

    [Header("Debug")]
    [SerializeField] private bool _isFalling = false;
    [SerializeField] private float _startTime = 0.0f;
    [SerializeField] private float _endTime = 0.0f;
    [SerializeField] private float _despawnTime = 0.0f;
    [SerializeField] private float _progress = 0.0f;

    [SerializeField] private Vector3 _startPos;
    [SerializeField] private Vector3 _endPos;



    public void Init(Vector3 startPos, Vector3 endPos)
    {
        _startPos = startPos;
        _endPos = endPos;

        _startTime = GameManager.Instance.IngameTimer;
        _endTime = _startTime + _travelDuration;
        _despawnTime = _endTime + _remainOnGroundTime;

        transform.position = _startPos;
        _isFalling = true;
        gameObject.SetActive(true);

        Debug.Log("Dropping MoneyBag at: " + _endPos);
    }


    public void Update()
    {
        float timer = GameManager.Instance.IngameTimer;

        if (timer > _endTime)
        {
            FallingComplete();
        }
        if (timer > _despawnTime)
        {
            DespawnMe();
        }

        if (!_isFalling)
        {
            return;
        }

        _progress = (timer - _startTime) / (_endTime - _startTime);
        float progressOnCurve = _curve.Evaluate(_progress);
        transform.position = Vector3.Lerp(_startPos, _endPos, progressOnCurve);
    }


    private void FallingComplete()
    {
        _isFalling = false;

        Collider[] hits = Physics.OverlapSphere(_endPos, _hitRadius, _hitLayers);
        if (hits.Length < 1)
        {
            Debug.Log("MoneyBag didnt hit any Colliders in layers");
            return;
        }
        Debug.Log("MoneyBag hit " + hits.Length + " targets");
        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent<IHittable>(out IHittable hittable))
            {
                hittable.GetHit(_damage, transform.position, 10.0f);
                Debug.Log("MoneyBag hit IHittable " + hittable + " for " + _damage + "dmg");
            }

            Debug.Log("MoneyBag hit Target in LayerMask with no IHittable");
        }
    }
    private void DespawnMe()
    {
        gameObject.SetActive(false);
    }

}
