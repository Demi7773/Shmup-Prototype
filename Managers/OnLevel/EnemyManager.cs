using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    private LevelController _levelController;
    [Header("Setup")]
    [SerializeField] private SimpleEnemySpawner _enemySpawner;
    [Space(20)]
    [Header("Debug")]
    [SerializeField] private PlayerReferences _playerRef;
    [SerializeField] private Transform _playerTransform;
    [Header("Status")]
    [SerializeField] private bool _levelStarted = false;
    [SerializeField] private bool _isTimerRunning = false;
    [SerializeField] private bool _isSpawning = false;
    [SerializeField] private float _thisLevelTimer = 0.0f;
    [Space(10)]
    [Header("Wave")]
    [SerializeField] private int _waveNumber = 0;
    [SerializeField] private int _finalWave => _enemySpawner.NumberOfWaves;
    [Space(10)]
    [Header("Timer Settings")]
    //[SerializeField] private AnimationCurve _timeBetweenSpawnsCurve;
    [SerializeField] private float _timeBetweenSpawns = 1.0f;
    private float _nextSpawn = 1.0f;
    [Space(10)]
    [Header("Tracking")]
    [SerializeField] private List<EnemyBehavior> _enemiesKilled;

    public bool LevelStarted => _levelStarted;
    public float ThisLevelTimer => _thisLevelTimer;
    public int WaveNumber => _waveNumber;
    //public int EnemiesKilledCount => _enemiesKilled.Count;

    public static event Action<EnemyManager> EnemyManagerSpawned;
    public static event Action<int> OnNewWave;
    public static event Action<float> OnWaveTimerUpdate;
    public static event Action WaveCleared;
    public static event Action<int, float> OnLevelCleared;



    public void Init(LevelController levelController, PlayerReferences playerRef)
    {
        _levelController = levelController;
        _playerRef = playerRef;
        _playerTransform = playerRef.PlayerTransform;

        _enemySpawner.Init(this, _playerTransform);
        _levelStarted = true;
        _thisLevelTimer = 0.0f;
        OnWaveTimerUpdate?.Invoke(_thisLevelTimer);
    }

    private void Awake()
    {
        EnemyManagerSpawned?.Invoke(this);
        //Debug.Log("EnemyManager spawned");
    }
    private void OnEnable()
    {
        EnemyBehavior.EnemyDeath += OnEnemyDeath;
    }
    private void OnDisable()
    {
        EnemyBehavior.EnemyDeath -= OnEnemyDeath;
    }
    private void OnEnemyDeath(EnemyBehavior enemy)
    {
        _enemiesKilled.Add(enemy);
        //if (_enemySpawner.EnemiesLeftCount < 1)
        //{
        //    if (!_isSpawning)
        //    {
        //        ClearedWave();
        //    }
        //}
    }
    public void CheckIfWaveOver()
    {
        if (!_isSpawning)
        {
            ClearedWave();
        }
    }



    private void Update()
    {
        if (_isTimerRunning)
        {
            _thisLevelTimer += Time.deltaTime;
            OnWaveTimerUpdate?.Invoke(_thisLevelTimer);

            if (_isSpawning)
            {
                if (_thisLevelTimer > _nextSpawn)
                {
                    _enemySpawner.SpawnEnemy(_waveNumber);
                    _nextSpawn = _thisLevelTimer + _timeBetweenSpawns;
                    //Debug.Log("EnemySpawn, nextSpawn at: " + _nextSpawn);
                }
            }
        }
    }



    private void ClearedWave()
    {
        _isTimerRunning = false;
        WaveCleared?.Invoke();
        if (_waveNumber >= _finalWave)
        {
            OnLevelCleared?.Invoke(GameManager.Instance.CurrentLevel, _thisLevelTimer);
            UIEvents.DisplayNewMessage("Level Cleared!");
            return;
        }
        UIEvents.DisplayNewMessage("Wave " + _waveNumber + " completed!");
    }

    public void StartNextWave()
    {

        if (!_levelStarted)
        {
            Debug.Log("Level hasnt started on Next Wave call !");
            return;
        }
        if (_isTimerRunning)
        {
            Debug.Log("Timer is still running which implies previous wave wasnt completed");
            return;
        }
        if (_waveNumber > _finalWave - 1)
        {
            Debug.Log("No more waves!");
            UIEvents.DisplayNewMessage("No more waves! Move on to shop to continue towards the next level");
            return;
        }

        _waveNumber++;
        
        //_currentEnemiesLevel = (int) _enemyLevelsByWaveCurve.Evaluate(_waveNumber);

        //_spawnScoreToSpend = (int)_spawnScoreByWave.Evaluate(_waveNumber);

        //_enemy1SpawnRate = _enemy1SpawnRateByWave.Evaluate(_waveNumber);
        //_enemy2SpawnRate = _enemy2SpawnRateByWave.Evaluate(_waveNumber);
        //_enemy3SpawnRate = _enemy3SpawnRateByWave.Evaluate(_waveNumber);
        //_totalSpawnRates = _enemy1SpawnRate + _enemy2SpawnRate + _enemy3SpawnRate;

        _isTimerRunning = true;
        _isSpawning = true;
        OnNewWave?.Invoke(_waveNumber);

        if (_waveNumber == _finalWave)
        {
            UIEvents.DisplayNewMessage("Final Wave Starting!");
            return;
        }
        UIEvents.DisplayNewMessage("Wave " + _waveNumber + " starting!");
    }

    public void StopSpawning()
    {
        _isSpawning = false;        
    }



    // old Spawning


    //[SerializeField] private EnemiesPool _pool;
    //[SerializeField] private EnemySpawnpoints _spawnPoints;
    //[SerializeField] private List<EnemyBehavior> _enemiesOnLevel;
    //public int EnemiesLeftCount => _enemiesOnLevel.Count;

    // old ver
    //[Space(10)]
    //[Header("Enemy Level Settings")]
    //[SerializeField] private AnimationCurve _enemyLevelsByWaveCurve;
    //[SerializeField] private int _currentEnemiesLevel = 1;


    //[Space(10)]
    //[Header("SpawnScore")]
    //[SerializeField] private AnimationCurve _spawnScoreByWave;
    //[SerializeField] private int _spawnScoreToSpend;
    //[SerializeField] private float _totalSpawnRates = 0.0f;

    //[Space(10)]
    //[Header("Enemy1")]
    //[SerializeField] private AnimationCurve _enemy1SpawnRateByWave;
    //[SerializeField] private float _enemy1SpawnRate = 0.0f;
    //[SerializeField] private int _enemy1SpawnScore = 1;
    //[Header("Enemy2")]
    //[SerializeField] private AnimationCurve _enemy2SpawnRateByWave;
    //[SerializeField] private float _enemy2SpawnRate = 0.0f;
    //[SerializeField] private int _enemy2SpawnScore = 2;
    //[Header("Enemy3")]
    //[SerializeField] private AnimationCurve _enemy3SpawnRateByWave;
    //[SerializeField] private float _enemy3SpawnRate = 0.0f;
    //[SerializeField] private int _enemy3SpawnScore = 3;


    //public static event Action<float> OnEnemyTimerTick;




    //private void SpawnEnemy()
    //{
    //EnemyBehavior enemy = null;
    //int rollSpawnpoint = UnityEngine.Random.Range(0, _spawnPoints.Count);
    //float rollEnemy = UnityEngine.Random.Range(0.0f, _totalSpawnRates);

    //if (rollEnemy < _enemy1SpawnRate)
    //{
    //    enemy = _pool.GetEnemy1FromPool();
    //    _spawnScoreToSpend -= _enemy1SpawnScore;
    //}
    //else if (rollEnemy > _enemy2SpawnRate)
    //{
    //    enemy = _pool.GetEnemy2FromPool();
    //    _spawnScoreToSpend -= _enemy2SpawnScore;
    //}
    //else
    //{
    //    enemy = _pool.GetEnemy3FromPool();
    //    _spawnScoreToSpend -= _enemy3SpawnScore;
    //}

    //enemy.transform.position = _spawnPoints[rollSpawnpoint].position;
    //_enemiesOnLevel.Add(enemy);

    // enemy.SpawnMeAt(/*this,*/ _currentEnemiesLevel, _playerTransform, _spawnPoints.GetSpawnPosition());
    // Debug.Log("EnemySpawn, rolled: " + rollEnemy + " at SpawnPoint: " + rollSpawnpoint + ", enemy: " + enemy.name + ", new SpawnScore: " + _spawnScoreToSpend);

    //CheckConditionsAfterSpawn();

    //}

    //private void CheckConditionsAfterSpawn()
    //{
    //    if (_spawnScoreToSpend < _enemy3SpawnScore)
    //    {
    //        _totalSpawnRates -= _enemy3SpawnRate;
    //        Debug.Log("Not enough score for Enemy3, removing from pool");

    //        if (_spawnScoreToSpend < _enemy2SpawnScore)
    //        {
    //            Debug.Log("Not enough score for Enemy2, removing from pool");
    //            _totalSpawnRates -= _enemy2SpawnRate;

    //            if (_spawnScoreToSpend < _enemy1SpawnScore)
    //            {
    //                StopSpawning();
    //                return;
    //            }
    //        }
    //    }
    //}

}
