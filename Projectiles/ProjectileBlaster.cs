using UnityEngine;
using UnityEngine.VFX;

public class ProjectileBlaster : Projectile
{

    [SerializeField] protected VisualEffect _laserVFX;



    public override void Init(Vector3 direction, float dmg)
    {
        base.Init(direction, dmg);

        _laserVFX.Play();
    }

    protected override void Despawn()
    {
        _laserVFX.Stop();
        base.Despawn();
    }

}
