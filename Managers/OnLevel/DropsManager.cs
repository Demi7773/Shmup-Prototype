using System.Collections.Generic;
using UnityEngine;

public class DropsManager : MonoBehaviour
{

    //[SerializeField] private Transform _playerTransform;
    [SerializeField] private MoneyPickup _moneyPickupPrefab;
    [SerializeField] private List<MoneyPickup> _droppedMoneys = new List<MoneyPickup>();



    private void OnEnable()
    {
        EnemyBehavior.EnemyDropMoney += OnEnemyDropMoney;
        EnemyManager.WaveCleared += OnWaveCleared;
    }
    private void OnDisable()
    {
        EnemyBehavior.EnemyDropMoney -= OnEnemyDropMoney;
        EnemyManager.WaveCleared -= OnWaveCleared;
    }

    private void OnEnemyDropMoney(int amount, Vector3 position)
    {
        MoneyPickup money = Instantiate(_moneyPickupPrefab, transform);
        money.SpawnMe(amount, position);
        _droppedMoneys.Add(money);
    }

    private void OnWaveCleared()
    {
        foreach (var money in _droppedMoneys)
        {
            if (!money.gameObject.activeSelf)
            {
                Debug.Log("Money not active, skipping");
                continue;
            }

            money.StartPickupSequence(GameManager.Instance.PlayerRef.PlayerTransform);
        }
    }

}
