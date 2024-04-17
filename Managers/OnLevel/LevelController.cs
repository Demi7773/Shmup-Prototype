using System;
using UnityEngine;

public class LevelController : MonoBehaviour
{

    [SerializeField] private PlayerReferences _playerRefPrefab;
    /*[SerializeField] */private PlayerReferences _playerInstance;
    public PlayerReferences PlayerRef => _playerInstance;

    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private FollowPlayer _playerCam;
    [SerializeField] private Transform _playerSpawnPoint;
    //private LevelsManager _levelsManager;
    private GameManager _gm;

    public int WaveReached => _enemyManager.WaveNumber;
    public float LevelTimer => _enemyManager.ThisLevelTimer;

    public static event Action<LevelController> OnLevelLoaded;

    public static event Action<bool> OnExitLevel;




    private void Awake()
    {
        OnLevelLoaded?.Invoke(this);
    }

    public void Init(GameManager gm)
    {
        _gm = gm;
        _playerInstance = Instantiate(_playerRefPrefab, _playerSpawnPoint.position, _playerSpawnPoint.rotation, _playerSpawnPoint);
        _playerCam.SetPlayerTransform(_playerInstance.PlayerTransform);
        _enemyManager.Init(this, _playerInstance);
        //Debug.Log("LevelController Init Complete");
    }



    public void ExitLevel()
    {
        OnExitLevel?.Invoke(true);
    }


}
