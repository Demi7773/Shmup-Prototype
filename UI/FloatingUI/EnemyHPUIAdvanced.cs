using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPUIAdvanced : MonoBehaviour
{

    [Header("Dependencies")]
    [SerializeField] private List<FloatingNumber> _floatingNumbers = new List<FloatingNumber>();
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private Image _healthBarImage;
    [SerializeField] private GameObject _healthBar;

    [Header("HPBarSettings")]
    [SerializeField] private Vector3 _hpBarOffset = new Vector3(0.0f, 2.0f, 3.0f);
    //[SerializeField] private float _barFillSpeed = 2f;
    //private float _targetValue = 1f;

    [Space(20)]
    [Header("Debug")]
    [SerializeField] private Camera _cam;
    [SerializeField] private int _lastDeployedIndex = 0;



    public void Init()
    {
        _cam = Camera.main;
        foreach (FloatingNumber number in _floatingNumbers)
        {
            number.gameObject.SetActive(false);
        }

        _lastDeployedIndex = 0;
        gameObject.SetActive(true);
    }
    public void UpdateHealthBar(float currentHP, float maxHP)
    {
        _healthBarImage.fillAmount = currentHP / maxHP;
    }
    public void DisplayFloatingNumber(FloatingNumber.Context context, Vector3 startPos, float amount)
    {
        var floatingNumber = GetAvailableFloatingNumber();
        floatingNumber.Init(context, startPos, amount);
    }

    private FloatingNumber GetAvailableFloatingNumber()
    {
        int index = _lastDeployedIndex;
        _lastDeployedIndex++;
        return _floatingNumbers[index];
    }


    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - _cam.transform.position, _cam.transform.up);
        _healthBar.transform.position = _targetTransform.position + _hpBarOffset;
        //_healthBarImage.fillAmount = Mathf.MoveTowards(_healthBarImage.fillAmount, _targetValue, _barFillSpeed * Time.deltaTime);
    }

    

}