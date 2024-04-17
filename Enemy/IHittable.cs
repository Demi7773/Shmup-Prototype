using UnityEngine;

public interface IHittable
{

    public void GetHit(float damage, Vector3 sourceDirection, float force);

}
