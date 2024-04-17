using UnityEngine;

public class PressurePlate : MonoBehaviour
{

    [Header("Setup")]
    [SerializeField] private Trap[] _traps;

    [SerializeField] private Transform _plate;
    [SerializeField] private float _cooldownAfterUnpress = 0.3f;
    [SerializeField] private float _pressAnimationSpeed = 1.0f;
    [SerializeField] private Vector3 _onPressOffset;

    [SerializeField] private LayerMask _triggerMeLayers;
    [SerializeField] private Transform[] _sensors;

    [Header("Debug")]
    [SerializeField] private float _timer = 0.0f;
    [SerializeField] private float _lastPressedAt = -10.0f;
    [SerializeField] private bool _isPressed = false;
    [SerializeField] private Vector3 _unpressedPos;
    [SerializeField] private Vector3 _pressedPos;




    private void Awake()
    {
        _unpressedPos = _plate.position;
        _pressedPos = _unpressedPos + _onPressOffset;
    }



    private void Update()
    {
        _timer += Time.deltaTime;
        foreach (var sensor in _sensors)
        {
            if (Physics.Raycast(sensor.position, sensor.up, 2.0f, _triggerMeLayers))
            {
                _isPressed = true;
                _lastPressedAt = _timer;
                break;
            }
            _isPressed = false;
        }
        if (!_isPressed)
        {
            if (_timer < _lastPressedAt + _cooldownAfterUnpress)
                return;
            _plate.position = Vector3.MoveTowards(_plate.position, _unpressedPos, _pressAnimationSpeed * Time.deltaTime);
            return;
        }
        _plate.position = Vector3.MoveTowards(_plate.position, _pressedPos, _pressAnimationSpeed * Time.deltaTime);

        foreach(var trap in _traps)
        {
            trap.TryToProcMe();
        }
    }

}
