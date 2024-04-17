using UnityEngine;

public class MoneyPickup : MonoBehaviour
{

    [SerializeField] private bool _isGettingPickedUp = false;
    //[SerializeField] private int _heldAmount;
    public int Amount = 0;
    public bool IsGettingPickedUp => _isGettingPickedUp;

    [Space(20)]
    [Header("Movement")]
    [SerializeField] private Transform _modelTransform;
    [SerializeField] private float _rotationSpeed = 10.0f;

    [SerializeField] private float _bobbingAnchorY = 1.0f;
    [SerializeField] private float _bobbingFrequency = 0.5f;
    [SerializeField] private float _bobbingAmplitude = 0.5f;

    [SerializeField] private float _moveTowardsPlayerSpeed = 1.0f;

    private float _timer = 0.0f;
    private Vector3 _spawnPos = Vector3.zero;
    private Transform _player;



    public void SpawnMe(int value, Vector3 position)
    {
        Amount = value;
        _isGettingPickedUp = false;
        _timer = 0.0f;
        _spawnPos = position;
        _bobbingAnchorY = _spawnPos.y + 0.5f;
        transform.position = _spawnPos;

        gameObject.SetActive(true);
        //_isGettingPickedUp = false;
    }

    public void StartPickupSequence(Transform player)
    {
        _player = player;
        _isGettingPickedUp = true;
        _timer = 0.0f;

        Debug.Log("Getting picked up");
    }

    //public int PickUpValue()
    //{
    //    return _heldAmount;
    //}



    private void Update()
    {
        _timer += Time.deltaTime;

        _modelTransform.Rotate(0.0f, _rotationSpeed * Time.deltaTime, 0.0f);

        if (!IsGettingPickedUp)
        {
            _modelTransform.position = new Vector3(_spawnPos.x, _bobbingAnchorY + SinStep(_timer), _spawnPos.z);
            return;
        }

        transform.position = LerpToPlayer(_timer);
    }



    private float SinStep(float time)
    {
        return _bobbingAmplitude * Mathf.Sin(time * _bobbingFrequency);
    }
    private Vector3 LerpToPlayer(float time)
    {
        return Vector3.Lerp(_spawnPos, _player.position, time * _moveTowardsPlayerSpeed);
    }

}
