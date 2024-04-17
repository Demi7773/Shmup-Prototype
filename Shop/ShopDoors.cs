using UnityEngine;

public class ShopDoors : MonoBehaviour
{

    private PlayerReferences _playerRef;
    private Transform _playerTransform;

    private bool _doorsUnlocked = true;

    [SerializeField] private float _distanceToOpen = 1.0f;
    [SerializeField] private float _timerForOpenProgression = 0.0f;

    [SerializeField] private Transform _leftDoor;
    [SerializeField] private Transform _rightDoor;
    private Vector3 _leftDoorStartingPos;
    private Vector3 _rightDoorStartingPos;
    [SerializeField] private AnimationCurve _openCloseCurve;





    private void OnEnable()
    {
        //EnemyManager.WaveCleared += OnWaveCleared;
        //EnemyManager.OnNewWave += OnWaveStart;
        PlayerReferences.NewPlayerReference += SetPlayerRef;

        EnemyManager.OnLevelCleared += OnLevelCleared;
    }
    private void OnDisable()
    {
        //EnemyManager.WaveCleared -= OnWaveCleared;
        //EnemyManager.OnNewWave -= OnWaveStart;
        PlayerReferences.NewPlayerReference -= SetPlayerRef;

        EnemyManager.OnLevelCleared -= OnLevelCleared;
    }
    private void SetPlayerRef(PlayerReferences playerRef)
    {
        _playerRef = playerRef;
        _playerTransform = _playerRef.PlayerTransform;
    }




    private void Awake()
    {
        _leftDoorStartingPos = _leftDoor.position;
        _rightDoorStartingPos = _rightDoor.position;

        _doorsUnlocked = false;
    }

    //private void OnWaveCleared()
    //{
    //    _doorsUnlocked = true;
    //}
    //private void OnWaveStart(int waveNum)
    //{
    //    _doorsUnlocked = false;
    //}

    private void OnLevelCleared(int level, float time)
    {
        _doorsUnlocked = true;
    }


    private void Update()
    {
        if (_playerTransform == null)
        {
            return;
        }

        if (!_doorsUnlocked)
        {
            _timerForOpenProgression -= Time.deltaTime;
        }
        if (Vector3.Distance(transform.position, _playerTransform.position) > _distanceToOpen)
        {
            _timerForOpenProgression -= Time.deltaTime;
        }
        else
        {
            _timerForOpenProgression += Time.deltaTime;
        }

        _timerForOpenProgression = Mathf.Clamp01(_timerForOpenProgression);

        _leftDoor.position = new Vector3(_leftDoorStartingPos.x - _openCloseCurve.Evaluate(_timerForOpenProgression), _leftDoorStartingPos.y, _leftDoorStartingPos.z);
        _rightDoor.position = new Vector3(_rightDoorStartingPos.x + _openCloseCurve.Evaluate(_timerForOpenProgression), _rightDoorStartingPos.y, _rightDoorStartingPos.z);

    }

}
