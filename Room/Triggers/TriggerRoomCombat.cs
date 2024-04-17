using UnityEngine;

public class TriggerRoomCombat : MonoBehaviour
{

    [SerializeField] private RoomEnemySpawner _enemySpawner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _enemySpawner.OnStartCombat();
        }
    }

}
