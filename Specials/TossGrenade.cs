using UnityEngine;

public class TossGrenade : SpecialAttack
{

    protected NewPlayerController _controls;

    [Space(30)]
    [Header("TossGrenade")]
    [Header("Setup")]
    [SerializeField] protected float _launcherXOffset = 1.0f;
    [SerializeField] protected float _laucnherYOffset = 1.0f;
    [SerializeField] protected LayerMask _terrainLayers;
    [SerializeField] protected ProjectileExplosive _explosivePrefab;
    [SerializeField] protected Transform _spawnPos;

    [SerializeField] protected float _throwMinRadius= 5.0f;
    [SerializeField] protected float _throwMaxRadius= 10.0f;

    [SerializeField] protected float _explosionRadius = 5.0f;
    [SerializeField] protected float _explosionDamage= 30.0f;





    public override float CooldownProgress()
    {
        return (_timer - _endOfUse) / (_nextCanUseTarget - _endOfUse);
    }



    protected void Awake()
    {
        _controls = GetComponentInParent<NewPlayerController>();
    }



    public override bool CanUseSpecial()
    {
        if (_timer > _nextCanUseTarget)
            return true;
        return false;
    }
    public override void UseSpecial()
    {
        _lastUsed = _timer;
        _endOfUse = _timer;
        _nextCanUseTarget = _timer + _useCooldown;
        SetProjectileTrajectory();
    }



    protected void SetProjectileTrajectory()
    {
        Vector3 spawnPosition = _spawnPos.position;
        var projectile = Instantiate(_explosivePrefab, spawnPosition, _spawnPos.rotation);
        projectile.Init(CalculateEndPositionOutsideTerrain(spawnPosition), _explosionRadius, _explosionDamage);
    }
    protected Vector3 CalculateEndPositionOutsideTerrain(Vector3 startPos)
    {
        Vector3 endPos = _controls.AimPos;
        Vector3 direction = endPos - startPos;

        float distance = Vector3.Distance(startPos, endPos);
        if (distance < _throwMinRadius)
        {
            Vector3 newEndPos = startPos + (direction.normalized * _throwMinRadius);
            //Debug.Log("Tried to toss grenade too close, endPos changed: " + endPos + " -> " + newEndPos);

            endPos = newEndPos;
            distance = Vector3.Distance(startPos, endPos);
        }
        if (distance > _throwMaxRadius)
        {
            Vector3 newEndPos = startPos + (direction.normalized * _throwMaxRadius);
            //Debug.Log("Tried to toss grenade too far, endPos changed: " + endPos + " -> " + newEndPos);

            endPos = newEndPos;
            distance = Vector3.Distance(startPos, endPos);
        }

        Ray ray = new Ray(startPos, direction);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, distance, _terrainLayers))
        {
            Vector3 dir2 = direction + (hitInfo.normal * 0.5f);
            Vector3 endPos2 = startPos + (dir2 * distance);
            endPos = endPos2;

            //Debug.Log("RaycastHit1 at: " + hitInfo.point + ", adjusting trajectory to: " + dir2 + ", endPos: " + endPos);
            //Debug.DrawLine(startPos, endPos2, Color.red, 2.0f);

            Ray ray2 = new Ray(startPos, dir2);
            RaycastHit hitInfo2;
            if (Physics.Raycast(ray2, out hitInfo2, distance, _terrainLayers))
            {
                Vector3 dir3 = dir2 + (hitInfo2.normal * 0.5f);
                Vector3 endPos3 = startPos + (dir3 * distance);
                endPos = endPos3;

                //Debug.Log("RaycastHit2 at: " + hitInfo2.point + ", adjusting trajectory to: " + dir3 + ", endPos: " + endPos);
                //Debug.DrawLine(startPos, endPos3, Color.red, 2.0f);

                Ray ray3 = new Ray(startPos, dir3);
                RaycastHit hitInfo3;
                if (Physics.Raycast(ray3, out hitInfo3, distance, _terrainLayers))
                {
                    endPos = hitInfo.point;
                    Debug.Log("RaycastHit3 at: " + hitInfo3.point + ". Defaulting to 1st Raycast hit point: " + hitInfo.point);
                }
            }
        }

        return OffsetFinalResult(endPos);
    }
    protected Vector3 OffsetFinalResult(Vector3 pos)
    {
        Vector3 adjustedPos = pos;
        float x = Random.Range(-_launcherXOffset, _launcherXOffset);
        float y = Random.Range(-_laucnherYOffset, _laucnherYOffset);
        adjustedPos.x += x;
        adjustedPos.y += y;
        return adjustedPos;
    }

}
