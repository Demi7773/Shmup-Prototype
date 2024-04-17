using System.Collections;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{

    [Header("Dependencies")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Camera _cam;

    [Header("Setup")]
    [SerializeField] private Vector3 _offset;
    [SerializeField] private float _speed = 3.0f;

    [Header("CameraShake")]
    [SerializeField] private float _onGetHitShakeDuration = 0.15f;
    [SerializeField] private float _minShakeIntensity = 5.0f;
    [SerializeField] private float _maxShakeIntensity = 10.0f;
    [SerializeField] private AnimationCurve _shakeX;
    [SerializeField] private AnimationCurve _shakeY;
    [SerializeField] private AnimationCurve _shakeZ;
    [SerializeField] private float _finalXMod = 1.0f;
    [SerializeField] private float _finalYMod = 1.0f;
    [SerializeField] private float _finalZMod = 1.0f;
    //[SerializeField] private float _catchUpThreshold = 2.0f;
    //[SerializeField] private float _catchUpSpeed = 10.0f;

    [Header("Debug")]
    [SerializeField] private Vector3 _targetPos;



    private void OnEnable()
    {
        PlayerHP.OnPlayerGetHit += OnPlayerGetHit;
    }
    private void OnDisable()
    {
        PlayerHP.OnPlayerGetHit -= OnPlayerGetHit;
    }
    private void OnPlayerGetHit(float dmg)
    {
        StartCoroutine(ShakeCamera(dmg, _onGetHitShakeDuration));
    }



    public void SetPlayerTransform(Transform playerTransform)
    {
        _playerTransform = playerTransform;
    }


    private void Update()
    {

        if (_playerTransform == null) return;

        _targetPos = _playerTransform.position + _offset;
        float distance = Vector3.Distance(transform.position, _targetPos);
        transform.position = Vector3.Lerp(transform.position, _targetPos, _speed * distance * Time.deltaTime);
    }



    private IEnumerator ShakeCamera(float intensity, float duration)
    {
        Quaternion startRotation = _cam.transform.rotation;
        intensity = Mathf.Clamp(intensity, _minShakeIntensity, _maxShakeIntensity);
        float timer = 0.0f;

        while (timer < duration)
        {
            float progress = timer / duration;
            float x = ((intensity * _shakeX.Evaluate(timer)) * _finalXMod) + 90.0f;
            float y = (intensity * _shakeY.Evaluate(timer)) * _finalYMod;
            float z = (intensity * _shakeZ.Evaluate(timer)) * _finalZMod;
            _cam.transform.rotation = Quaternion.Euler(x, y, z);
            yield return null;
            timer += Time.unscaledDeltaTime;

            if (timer > duration)
            {
                break;
            }
        }
        
        _cam.transform.rotation = startRotation;
    }

}
