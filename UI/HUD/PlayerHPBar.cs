using UnityEngine;
using UnityEngine.UI;

public class PlayerHPBar : MonoBehaviour
{

    [SerializeField] private Image _healthBarImage;
    [SerializeField] private float _reduceSpeed = 2f;
    private float _targetValue = 1f;



    public void UpdateHealthBar(float currentHP, float maxHP)
    {
        _targetValue = currentHP / maxHP;
    }

    private void Update()
    {
        _healthBarImage.fillAmount = Mathf.MoveTowards(_healthBarImage.fillAmount, _targetValue, _reduceSpeed * Time.deltaTime);
    }

}
