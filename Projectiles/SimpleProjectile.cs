using UnityEngine;

public class SimpleProjectile : MonoBehaviour
{

    [Header("Setup")]
    [SerializeField] private LayerMask _hitLayers;
    [SerializeField] private float _flightDistance = 20.0f;
    [SerializeField] private float _timer = 0.0f;
    [SerializeField] private float _duration = 2.0f;
    [SerializeField] private float _hitRadius = 0.3f;
    [Space(10)]
    [Header("Debug")]
    [SerializeField] private Vector3 _startPos;
    [SerializeField] private Vector3 _endPos;
    [SerializeField] private float _damage = 10.0f;



    public void FireMe(Vector3 startPos, Vector3 aimPos/*Vector3 direction*/, float dmg)
    {
        SetTrajectory(startPos, aimPos);
        _damage = dmg;
        transform.position = startPos;
        _timer = 0.0f;
        gameObject.SetActive(true);
        //Debug.Log("StartPos: " + _startPos + ", endPos: " + _endPos);
    }
    private void SetTrajectory(Vector3 startPos, Vector3 aimPos)
    {
        _startPos = startPos;
        Vector3 dir = (aimPos - _startPos).normalized;
        Vector3 end = dir * _flightDistance;
        _endPos = _startPos + end;

    }



    public void Update()
    {
        _timer += Time.deltaTime;
        float progress = _timer / _duration;

        if (progress < 1.0f) 
        {
            Vector3 newPos = Vector3.Lerp(_startPos, _endPos, progress);
            //Debug.Log("SimpleProjectile progress, pos: " + progress + ", " + newPos);
            transform.position = newPos;
            CheckForHits();
            return;
        }
        //Debug.Log("SimpleProjectile Timer ran out, despawning");
        Despawn();
        
    }
    private void CheckForHits()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _hitRadius, _hitLayers);

        if (hits.Length < 1)
        {
            //Debug.Log("No hits on scan");
            return;
        }

        //Debug.Log("Scan hit " + hits.Length + " targets in layers");
        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent(out IHittable hittable))
            {
                hittable.GetHit(_damage, transform.position, 1.0f);
                //Debug.Log("Scan hit IHittable " + hittable + ", did " + _damage + "dmg. Continuing scans");
                Despawn();
                break;
            }
        }

        //Debug.Log("Scan hit Target in LayerMask with no IHittable, despawning");
        Despawn();
    }



    private void Despawn()
    {
        _timer = 0.0f;
        _startPos = Vector3.zero; 
        _endPos = Vector3.zero;
        gameObject.SetActive(false);
    }

}
