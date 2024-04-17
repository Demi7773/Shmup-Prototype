using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class SimpleEnemySpawner : MonoBehaviour
{

    [Header("Spawner")]
    public int NumberOfWaves = 1;
    [SerializeField] protected float _chanceForLevel2Enemies = 0f;
    [SerializeField] protected float _chanceForLevel3Enemies = 0f;

    [SerializeField] protected EnemyManager _enemyManager;
    [SerializeField] protected Transform[] _spawnPoints;
    [Header("Timer")]
    [SerializeField] protected float _timeBetweenPathUpdates = 0.3f;
    [Space(10)]
    [Header("Debug")]
    [SerializeField] protected float _nextPathUpdate = 0.3f;
    [SerializeField] protected List<EnemyBehavior> _livingEnemies = new List<EnemyBehavior>();
    public int EnemiesLeftCount => _livingEnemies.Count;


    public static event Action OnUpdateEnemyPathing;
    public static event Action OnEnemySpawnerInitComplete;





    protected virtual void OnEnable()
    {
        EnemyBehavior.EnemyDeath += OnEnemyDeath;
        EnemyManager.OnWaveTimerUpdate += OnEnemyManagerTimerUpdate;
    }
    protected virtual void OnDisable()
    {
        EnemyBehavior.EnemyDeath -= OnEnemyDeath;
        EnemyManager.OnWaveTimerUpdate -= OnEnemyManagerTimerUpdate;
    }
    protected void OnEnemyDeath(EnemyBehavior enemyBehavior)
    {
        _livingEnemies.Remove(enemyBehavior);

        if (_livingEnemies.Count < 1)
            _enemyManager.CheckIfWaveOver();
    }



    protected virtual void OnEnemyManagerTimerUpdate(float timer)
    {
        if (timer > _nextPathUpdate)
        {
            OnUpdateEnemyPathing?.Invoke();
            _nextPathUpdate = timer + _timeBetweenPathUpdates;
            //Debug.Log("Enemies Update Pathing ping");
        }
    }




    protected int GetEnemyLevelRoll()
    {
        float roll = UnityEngine.Random.Range(0f, 100f);
        //Debug.Log("Roll for enemyLvl: " +  roll);
        if (roll < _chanceForLevel3Enemies)
        {
            Debug.Log("Rolled Level 3 enemy");
            return 3;
        }

        if (roll < _chanceForLevel3Enemies + _chanceForLevel2Enemies)
        {
            Debug.Log("Rolled Level 2 enemy");
            return 2;
        }

        return 1;
    }




    public abstract void Init(EnemyManager enemyManager, Transform player);
    public abstract void SpawnEnemy(int wave);

    protected void InitCompleteInvoke()
    {
        OnEnemySpawnerInitComplete?.Invoke();
        //Debug.Log("Spawner Init complete");
    }

    protected void ShuffleList(List<EnemyBehavior> listToShuffle, List<EnemyBehavior> shuffled)
    {
        shuffled.Clear();

        List<EnemyBehavior> temp = new List<EnemyBehavior>();
        temp.AddRange(listToShuffle);

        for (int i = 0; i < listToShuffle.Count; i++)
        {
            int index = UnityEngine.Random.Range(0, temp.Count - 1);
            shuffled.Add(temp[index]);
            temp.RemoveAt(index);
        }
    }

    protected Vector3 GetSpawnPos()
    {
        int rollSpawnpoint = UnityEngine.Random.Range(0, _spawnPoints.Length);
        Vector3 spawnPos = _spawnPoints[rollSpawnpoint].position;
        return spawnPos;
    }

}
