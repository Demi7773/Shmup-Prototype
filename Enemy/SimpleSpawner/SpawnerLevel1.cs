using System.Collections.Generic;
using UnityEngine;

public class SpawnerLevel1 : SimpleEnemySpawner
{
    [Header("Level 1 Settings")]
    [Header("Enemy prefabs")]
    [SerializeField] private ZoomerBehavior _zoomerPrefab;
    [SerializeField] private BoomerBehavior _boomerPrefab;
    [SerializeField] private EnemyBehavior _shooterPrefab;
    [Space(30)]
    [Header("Wave1")]
    [SerializeField] private Transform _parentForWave1;
    [SerializeField] private int _zoomerSpawnsForWave1 = 10;
    private int _indexWave1 = 0;
    [SerializeField] private List<EnemyBehavior> _wave1ToShuffle = new List<EnemyBehavior>();
    [SerializeField] private List<EnemyBehavior> _wave1Shuffled = new List<EnemyBehavior>();
    [Space(10)]
    [Header("Wave2")]
    [SerializeField] private Transform _parentForWave2;
    [SerializeField] private int _zoomerSpawnsForWave2 = 15;
    [SerializeField] private int _boomerSpawnsForWave2 = 5;
    private int _indexWave2 = 0;
    [SerializeField] private List<EnemyBehavior> _wave2ToShuffle = new List<EnemyBehavior>();
    [SerializeField] private List<EnemyBehavior> _wave2Shuffled = new List<EnemyBehavior>();
    [Space(10)]
    [Header("Wave3")]
    [SerializeField] private Transform _parentForWave3;
    [SerializeField] private int _zoomerSpawnsForWave3 = 20;
    [SerializeField] private int _boomerSpawnsForWave3 = 10;
    private int _indexWave3 = 0;
    [SerializeField] private List<EnemyBehavior> _wave3ToShuffle = new List<EnemyBehavior>();
    [SerializeField] private List<EnemyBehavior> _wave3Shuffled = new List<EnemyBehavior>();




    public override void Init(EnemyManager enemyManager, Transform player)
    {
        _enemyManager = enemyManager;

        InitWave1(player);
        InitWave2(player);
        InitWave3(player);

        InitCompleteInvoke();
    }

    private void InitWave1(Transform player)
    {
        for (int i = 0; i < _zoomerSpawnsForWave1; i++)
        {
            var enemy = Instantiate(_zoomerPrefab, _parentForWave1);
            int lvl = GetEnemyLevelRoll();
            enemy.Init(lvl, player);

            _wave1ToShuffle.Add(enemy);
        }

        ShuffleList(_wave1ToShuffle, _wave1Shuffled);
    }

    private void InitWave2(Transform player)
    {
        for (int i = 0; i < _zoomerSpawnsForWave2; i++)
        {
            var enemy = Instantiate(_zoomerPrefab, _parentForWave2);
            int lvl = GetEnemyLevelRoll();
            enemy.Init(lvl, player);

            _wave2ToShuffle.Add(enemy);
        }
        for (int i = 0; i < _boomerSpawnsForWave2; i++)
        {
            var enemy = Instantiate(_boomerPrefab, _parentForWave2);
            int lvl = GetEnemyLevelRoll();
            enemy.Init(lvl, player);
            _wave2ToShuffle.Add(enemy);
        }

        ShuffleList(_wave2ToShuffle, _wave2Shuffled);
    }
    private void InitWave3(Transform player)
    {
        for (int i = 0; i < _zoomerSpawnsForWave3; i++)
        {
            var enemy = Instantiate(_zoomerPrefab, _parentForWave3);
            int lvl = GetEnemyLevelRoll();
            enemy.Init(lvl, player);

            _wave3ToShuffle.Add(enemy);
        }
        for (int i = 0; i < _boomerSpawnsForWave3; i++)
        {
            var enemy = Instantiate(_boomerPrefab, _parentForWave3);
            int lvl = GetEnemyLevelRoll();
            enemy.Init(lvl, player);

            _wave3ToShuffle.Add(enemy);
        }

        ShuffleList(_wave3ToShuffle, _wave3Shuffled);
    }



    // spawning behavior

    public override void SpawnEnemy(int wave)
    {
        switch (wave)
        {
            default:
                Debug.Log("Wave num invalid");
                break;
            case 1:
                Wave1SpawnEnemy();
                break;
            case 2:
                Wave2SpawnEnemy();
                break;
            case 3:
                Wave3SpawnEnemy();
                break;
        }
    }
    private void Wave1SpawnEnemy()
    {
        var enemy = _wave1Shuffled[_indexWave1];
        Vector3 spawnPos = GetSpawnPos();
        enemy.SpawnMeAt(spawnPos);
        _livingEnemies.Add(enemy);

        _indexWave1++;
        if (_indexWave1 > _wave1Shuffled.Count - 1)
        {
            _enemyManager.StopSpawning();
            //Debug.Log("Reached end of Wave1 spawns at: " + _indexWave1);
        }
    }
    private void Wave2SpawnEnemy()
    {
        var enemy = _wave2Shuffled[_indexWave2];
        Vector3 spawnPos = GetSpawnPos();
        enemy.SpawnMeAt(spawnPos);
        _livingEnemies.Add(enemy);

        _indexWave2++;
        if (_indexWave2 > _wave2Shuffled.Count - 1)
        {
            _enemyManager.StopSpawning();
            //Debug.Log("Reached end of Wave2 spawns at: " + _indexWave2);
        }
    }
    private void Wave3SpawnEnemy()
    {
        var enemy = _wave3Shuffled[_indexWave3];
        Vector3 spawnPos = GetSpawnPos();
        enemy.SpawnMeAt(spawnPos);
        _livingEnemies.Add(enemy);

        _indexWave3++;
        if (_indexWave3 > _wave3Shuffled.Count - 1)
        {
            _enemyManager.StopSpawning();
            //Debug.Log("Reached end of Wave3 spawns at: " + _indexWave3);
        }
    }
}
