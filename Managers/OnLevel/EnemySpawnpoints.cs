using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.AI;

public class EnemySpawnpoints : MonoBehaviour
{

    [SerializeField] private List<Transform> _spawnPoints = new List<Transform>();
    //[SerializeField] private NavMeshSurface _surface;
    //[SerializeField] private LayerMask _navmeshLayer;
    //[SerializeField] private NavMeshAgent _agent;



    private void Awake()
    {
        //foreach (Transform t in _spawnPoints)
        //{
        //    NavMeshHit hit;
        //    if (NavMesh.SamplePosition(t.position, out hit, 10.0f, 1))
        //    {
        //        Debug.Log("Spawnpoint is on Navmesh");
        //        continue;
        //    }
        //    Debug.Log("SpawnPoint was outside of NavMesh, new position: " + hit.position);
        //    t.position = hit.position;
        //}
    }

    public Vector3 GetSpawnPosition()
    {
        int rollSpawnpoint = Random.Range(0, _spawnPoints.Count);
        return _spawnPoints[rollSpawnpoint].position;
    }

}
