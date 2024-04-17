using UnityEngine;

public class BreakableBarrier : MonoBehaviour, IHittable
{

    [SerializeField] private float _currentHP = 0f;
    [SerializeField] private float _maxHP = 50f;




    private void OnEnable()
    {
        _currentHP = _maxHP;
    }


    public void GetHit(float damage, Vector3 sourceDirection, float force)
    {
        _currentHP -= damage;
        if (_currentHP < 0.1f ) 
        {
            BreakMe();
        }
    }

    public void BreakMe()
    {
        gameObject.SetActive(false);
    }

}
