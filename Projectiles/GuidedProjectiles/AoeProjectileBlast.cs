using System.Collections.Generic;
using UnityEngine;

public class AoeProjectileBlast : GuidedProjectilesController
{

    [Header("AOE Burst Settings")]
    [SerializeField] protected List<Transform> _shootPositions = new List<Transform>();
    [SerializeField] protected float _startRadius = 1.0f;
    [SerializeField] protected float _endRadius = 20.0f;

    [Header("AOE Burst Debug")]
    [SerializeField] protected List<Vector3> _directions = new List<Vector3>();







    public override void Init()
    {
        base.Init();

        StoreDirections();
        Debug.Log("AOE Projectile Blast SpawnMeAt complete");
    }

    private void StoreDirections()
    {
        foreach (Transform t in _shootPositions)
        {
            Vector3 direction = t.forward;
            _directions.Add(direction);
        }
    }


    public override void Fire()
    {
        base.Fire();

        foreach (Vector3 dir in _directions)
        {
            Vector3 startPos = CalculatePosition(transform.position, dir, _startRadius);
            Vector3 endPos = CalculatePosition(startPos, dir, _endRadius);

            DeployProjectileFromQueue(startPos, endPos);
        }
    }

    protected virtual Vector3 CalculatePosition(Vector3 startPos, Vector3 direction, float distance)
    {
        return startPos + (direction * distance);
    }

}
