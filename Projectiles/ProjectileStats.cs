using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileStats")]
public class ProjectileStats : ScriptableObject
{

    [SerializeField] private float _speed = 100.0f;    
    [SerializeField] private float _despawnTimer = 5.0f;
    //[SerializeField] private float _damage = 10.0f;    

    public float Speed => _speed;
    public float DespawnTimer => _despawnTimer;
    //public float BaseDamage => _damage;
    
}
