using UnityEngine;
using UnityEngine.UI;

public class FloatingHPBar : MonoBehaviour
{

    [SerializeField] private Transform _targetTransform;
    [SerializeField] private Image _healthBarImage;
    [SerializeField] private GameObject _healthBar;
    [SerializeField] private Vector3 _offset = new Vector3(0.0f, 3.0f, 3.0f);
    //[SerializeField] private float _barFillSpeed = 2f;
    //private float _targetValue = 1f;
    private Camera _cam;

    private void Start()
    {
        _cam = Camera.main;
    }

    public void UpdateHealthBar(float currentHP, float maxHP)
    {
        _healthBarImage.fillAmount = currentHP / maxHP;
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - _cam.transform.position, _cam.transform.up);
        _healthBar.transform.position = _targetTransform.position + _offset;
        //_healthBarImage.fillAmount = Mathf.MoveTowards(_healthBarImage.fillAmount, _targetValue, _barFillSpeed * Time.deltaTime);
    }

}
