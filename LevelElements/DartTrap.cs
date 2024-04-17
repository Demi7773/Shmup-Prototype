using System.Collections;
using UnityEngine;

public class DartTrap : Trap
{

    [Header("Setup")]
    [SerializeField] protected SimpleProjectile[] _projectiles;
    [SerializeField] protected Transform _shootPos;
    [SerializeField] protected float _damage = 10.0f;
    [SerializeField] protected float _cooldown = 1.0f;
    [SerializeField] protected float _delayBetweenShots = 0.1f;
    [Header("Debug")]
    [SerializeField] protected bool _isOnCooldown = false;


    public override void TryToProcMe()
    {
        if (_isOnCooldown)
            return;
        StartCoroutine(ProcMe());
    }



    protected IEnumerator ProcMe()
    {
        _isOnCooldown = true;
        float timer = 0.0f;
        int numberOfShots = _projectiles.Length;
        int i = 0;
        while (i < numberOfShots)
        {
            yield return null;
            timer += Time.deltaTime;
            if (timer > (_delayBetweenShots * i))
            {
                ShootProjectile(i);
                i++;
                if (i > numberOfShots - 1)
                    break;
            }
        }
        StartCoroutine(Cooldown());
    }

    protected IEnumerator Cooldown()
    {
        float timer = 0.0f;
        while (timer < _cooldown)
        {
            yield return null;
            timer += Time.deltaTime;
            if (timer > _cooldown)
                break;
        }
        _isOnCooldown = false;
    }

    protected void ShootProjectile(int index)
    {
        Vector3 startPos = _shootPos.position;
        Vector3 aimPos = startPos + _shootPos.forward;
        _projectiles[index].FireMe(startPos, aimPos, _damage);
        Debug.Log("Dart trajectory: " + startPos + ", " + aimPos);
    }

}
