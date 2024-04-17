using UnityEngine;

public class DropMoney : MonoBehaviour
{
    [SerializeField] private MoneyPickup _moneyPrefab;

    [SerializeField] private int _moneyMinValue = 1;
    [SerializeField] private int _moneyMaxValue = 20;



    public void Drop()
    {

        float xOffset = Random.Range(-5.0f, 5.0f);
        float zOffset = Random.Range(-5.0f, 5.0f);
        Vector3 pos = transform.position + new Vector3(xOffset, 0.0f, zOffset);

        int dropValue = Random.Range(_moneyMinValue, _moneyMaxValue + 1);

        MoneyPickup drop = Instantiate(_moneyPrefab, transform);
        drop.SpawnMe(dropValue, pos);

        Debug.Log("Dropping " + dropValue + " cash");

    }

}
