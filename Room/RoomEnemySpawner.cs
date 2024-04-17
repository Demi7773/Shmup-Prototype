using System.Collections.Generic;
using UnityEngine;

public class RoomEnemySpawner : MonoBehaviour
{

    [SerializeField] protected Transform _playerTransform;
    [SerializeField] protected List<EnemyBehavior> _enemiesLeftInRoom = new List<EnemyBehavior>();
    [SerializeField] protected List<EnemyBehavior> _enemiesKilledInRoom = new List<EnemyBehavior>();
    [SerializeField] protected List<Door> _roomDoors = new List<Door>();



    [SerializeField] protected bool _hasRoomBeenStarted = false;
    [SerializeField] protected bool _hasRoomBeenCleared = false;

    //[SerializeField] protected List<Transform> _spawnPoints = new List<Transform>();





    protected void OnEnable()
    {
        EnemyBehavior.EnemyDeath += OnEnemyDeath;
    }
    protected void OnDisable()
    {
        EnemyBehavior.EnemyDeath -= OnEnemyDeath;
    }
    protected virtual void OnEnemyDeath(EnemyBehavior enemy)
    {

        if (!_hasRoomBeenStarted)
        {
            Debug.Log("Room hasnt been started yet");
            return;
        }

        if (_enemiesLeftInRoom.Contains(enemy))
        {
            _enemiesLeftInRoom.Remove(enemy);
            _enemiesKilledInRoom.Add(enemy);

            int enemiesLeft = _enemiesLeftInRoom.Count;

            Debug.Log("Enemy killed, enemies left in room: " + enemiesLeft);

            if (enemiesLeft < 1)
            {
                RoomCleared();
            }

            return;
        }

        Debug.Log("Room doesnt have the enemy on List");

    }
    protected void RoomCleared()
    {
        foreach(Door door in _roomDoors)
        {
            door.SetOpened(false);
        }
        _hasRoomBeenCleared = true;
    }



    protected void Awake()
    {
        Init(/*_enemyManager,*/ _playerTransform);
    }

    public void Init(/*EnemyManager enemyManager,*/ Transform playerTransform)
    {
        //_enemyManager = enemyManager;
        _playerTransform = playerTransform;

        for (int i = 0; i < _enemiesLeftInRoom.Count; i++)
        {
            EnemyBehavior enemy = _enemiesLeftInRoom[i];
            //enemy.SpawnMeAt(/*_enemyManager,*/ 1, _playerTransform, enemy.transform.position/*_spawnPoints[i].position*/);
            enemy.gameObject.SetActive(false);
        }
    }

    public void OnStartCombat()
    {
        if (_hasRoomBeenCleared)
        {
            Debug.Log("Room has already been cleared");
            return;
        }

        foreach (Door door in _roomDoors)
        {
            door.SetOpened(false);
        }
        foreach (EnemyBehavior enemy in _enemiesLeftInRoom)
        {
            enemy.gameObject.SetActive(true);
        }
        _hasRoomBeenStarted = true;
    }
  
}
