using UnityEngine;

public class FractalProjectilePart : Projectile
{
    
    protected override void Despawn()
    {
        //Debug.Log("FractalProjectilePart returning to SecondaryPool");
        _pool.AddObjectToSecondaryPool(this);
    }

}
