using UnityEngine;
using UnityEngine.VFX;

public class ExplosionHelper : MonoBehaviour
{

    [SerializeField] private VisualEffect _explosionVFX;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _explosionAudio;

    [SerializeField] private float _volume = 1.0f;
   


    private LayerMask _hitLayers;
    //private float _duration = 0.5f;
    private float _despawnTime = 100.0f;
    private float _radius = 5.0f;
    private float _damage = 10.0f;
    private float _explosionForce = 10.0f;



    //private void OnEnable()
    //{
    //    OnExplode();
    //}

    public void Init(LayerMask hitLayers, float duration, float radius, float damage, float force)
    {
        _hitLayers = hitLayers;
        //_duration = duration;
        _radius = radius;
        _damage = damage;
        _explosionForce = force;

        Explode();

        _despawnTime = GameManager.Instance.IngameTimer + duration;
        gameObject.SetActive(true);
        _audioSource.PlayOneShot(_explosionAudio, _volume);
        //_explosionVFX.Play();
    }
    private void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, _radius, _hitLayers);
        foreach (var hit in hits)
        {
            IHittable target = hit.GetComponent<IHittable>();
            if (target == null)
            {
                Debug.Log("No IHittable on " + target.ToString());
                continue;
            }
            //Debug.Log("Explosion hit " + target.ToString());
            target.GetHit(_damage, (hit.transform.position - transform.position).normalized, _explosionForce);
        }
    }



    private void Update()
    {
        if (GameManager.Instance.IngameTimer > _despawnTime)
        {
            gameObject.SetActive(false);
        }
    }
}
