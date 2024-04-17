using TMPro;
using UnityEngine;

public class WaveStartButton : Interactible
{

    [SerializeField] private EnemyManager _enemyManager;
    private PlayerReferences _playerRef;
    private Transform _playerTransform;
    private Camera _cam;
    [SerializeField] private float _distanceToDisplayInfo = 2.5f;
    [SerializeField] private GameObject _uiPopUp;
    [SerializeField] private TextMeshProUGUI _text = null;

    [SerializeField] private bool _isPressable = true;



    private void Awake()
    {
        _cam = Camera.main;
    }

    private void OnEnable()
    {
        EnemyManager.WaveCleared += OnWaveCleared;
        PlayerReferences.NewPlayerReference += SetPlayerRef;
    }
    private void OnDisable()
    {
        EnemyManager.WaveCleared -= OnWaveCleared;
        PlayerReferences.NewPlayerReference -= SetPlayerRef;
    }
    private void SetPlayerRef(PlayerReferences playerRef)
    {
        _playerRef = playerRef;
        _playerTransform = _playerRef.PlayerTransform;
    }
    private void OnWaveCleared()
    {
        _isPressable = true;
    }

    public override void Interact(PlayerReferences playerRef)
    {

        if (!_isPressable)
        {
            return;
        }

        _isPressable = false;
        _enemyManager.StartNextWave();

    }



    private void Update()
    {

        if (_playerTransform == null)
        {
            return;
        }

        if (!_isPressable)
        {
            _uiPopUp.SetActive(false);
            return;
        }

        if (Vector3.Distance(transform.position, _playerTransform.position) > _distanceToDisplayInfo)
        {
            _uiPopUp.SetActive(false);
            return;
        }
        else
        {
            _uiPopUp.SetActive(true);
            _text.SetText("Press E to start next wave");
        }

        _uiPopUp.transform.rotation = Quaternion.LookRotation(transform.position - _cam.transform.position, _cam.transform.up);
    }

}
