using UnityEngine;
using UnityEngine.VFX;

public class FractalProjectileRoot : Projectile
{

    [Header("FractalProjectileRoot")]
    //[SerializeField] protected int _splitNumber = 2;
    [SerializeField] protected float _damageModifierAfterSplit = 0.5f;
    [SerializeField] protected Transform _point1;
    [SerializeField] protected Transform _point2;
    [SerializeField] protected Transform _point3;
    [SerializeField] protected Transform _point4;

    //[SerializeField] protected VisualEffect _splitVFX;

    [SerializeField] protected bool _splitIntoFour = false;




    public virtual void ToggleSplitIntoFour(bool enable)
    {
        _splitIntoFour = enable;
    }


    protected override void Update()
    {
        if (GameManager.Instance.IngameTimer > _despawnAt)
        {
            SplitIntoFractalParts();
        }
    }



    protected virtual void SplitIntoFractalParts()
    {
        //_splitVFX.Play();

        float damage = _damage * _damageModifierAfterSplit;

        if (!_splitIntoFour)
        {
            var projectile1 = _pool.GetObjectFromSecondaryPool();
            var projectile2 = _pool.GetObjectFromSecondaryPool();
            FireProjectileAt(projectile1, _point1, damage);
            FireProjectileAt(projectile2, _point2, damage);

            //Debug.Log("FractalProjectileRoot split into 2 Parts");
            _pool.AddObjectToPool(this);

            return;
        }

        var modedProjectile1 = _pool.GetObjectFromSecondaryPool();
        var modedProjectile2 = _pool.GetObjectFromSecondaryPool();
        var modedProjectile3 = _pool.GetObjectFromSecondaryPool();
        var modedProjectile4 = _pool.GetObjectFromSecondaryPool();
        FireProjectileAt(modedProjectile1, _point1, damage);
        FireProjectileAt(modedProjectile2, _point2, damage);
        FireProjectileAt(modedProjectile3, _point3, damage);
        FireProjectileAt(modedProjectile4, _point4, damage);

        //Debug.Log("FractalProjectileRoot split into 4 Parts");
        _pool.AddObjectToPool(this);
    }

    protected virtual void FireProjectileAt(Projectile projectile, Transform point, float damage)
    {
        projectile.transform.position = point.position;
        projectile.transform.rotation = point.rotation;
        projectile.gameObject.SetActive(true);
        projectile.Init(projectile.transform.forward, damage);
    }

}
