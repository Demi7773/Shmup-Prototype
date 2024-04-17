

using UnityEngine;

public interface IEnemy
{

    public void Init(int level, Transform player, Vector3 position);

    protected void SetStatsForLevel(int level);

    protected void Death();

}
