using UnityEngine;

public class Smashable : MonoBehaviour, IHittable
{

    [SerializeField] private float _currentHP = 0f;
    [SerializeField] private float _maxHP = 1f;




    private void OnEnable()
    {
        _currentHP = _maxHP;
    }


    public void GetHit(float damage, Vector3 sourceDirection, float force)
    {
        _currentHP -= damage;
        if (_currentHP < 1f)
        {
            SmashMe();
        }
    }

    public void SmashMe()
    {
        gameObject.SetActive(false);
    }
}
