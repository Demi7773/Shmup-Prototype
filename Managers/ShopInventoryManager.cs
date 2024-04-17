using ReworkedWeapons;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopInventoryManager : MonoBehaviour
{

    [SerializeField] private ShopWeaponHolder _shopWeaponPrefab;

    [SerializeField] private PlayerReferences _playerRef;
    [SerializeField] private NewWeaponManager _playerWeapons;

    [SerializeField] private List<IWeapon.GunID> _availableInventory = new List<IWeapon.GunID>();

    [Header("Level1 settings")]
    [SerializeField] private int _minItems1 = 2;
    [SerializeField] private int _maxItems1 = 3;
    [Header("Level2 settings")]
    [SerializeField] private int _minItems2 = 2;
    [SerializeField] private int _maxItems2 = 4;
    [Header("Level3 settings")]
    [SerializeField] private int _minItems3 = 3;
    [SerializeField] private int _maxItems3 = 4;



    // add event for returning IDs back to pool on exiting shop

    private void OnEnable()
    {
        //ShopEvents.OnPlayerExitsShop += OnPlayerExitsShop;
    }
    private void OnDisable()
    {
        //ShopEvents.OnPlayerExitsShop -= OnPlayerExitsShop;
    }
    private void OnPlayerExitsShop(List<IWeapon.GunID> remainingInventory)
    {
        foreach (var weapon in remainingInventory)
        {
            _availableInventory.Add(weapon);
        }
    }




    // Add init ( from gm? )
    public void Init(PlayerReferences playerRef, int levelNum)
    {
        _playerRef = playerRef;

        switch (levelNum)
        {
            default:
                Debug.Log("Level num is not recognized");
                break;
            case 1:
                HandleLevel1Shop();
                break; 
            case 2:
                HandleLevel2Shop();
                break; 
            case 3:
                HandleLevel3Shop();
                break;
        }
    }

    private void HandleLevel1Shop()
    {
        int numberOfItems = Random.Range(_minItems1, _maxItems1 + 1);

        for (int i = 0; i < numberOfItems; i++)
        {
            int rollItem = Random.Range(0, _availableInventory.Count);
            IWeapon.GunID rolledGun = _availableInventory[rollItem];
            CreateItemFromWeapon(rolledGun);
            _availableInventory.Remove(rolledGun);
        }
    }
    private void HandleLevel2Shop()
    {
        int numberOfItems = Random.Range(_minItems2, _maxItems2 + 1);

        for (int i = 0; i < numberOfItems; i++)
        {
            int rollItem = Random.Range(0, _availableInventory.Count);
            IWeapon.GunID rolledGun = _availableInventory[rollItem];
            CreateItemFromWeapon(rolledGun);
            _availableInventory.Remove(rolledGun);
        }
    }
    private void HandleLevel3Shop()
    {
        int numberOfItems = Random.Range(_minItems3, _maxItems3 + 1);

        for (int i = 0; i < numberOfItems; i++)
        {
            int rollItem = Random.Range(0, _availableInventory.Count);
            IWeapon.GunID rolledGun = _availableInventory[rollItem];
            CreateItemFromWeapon(rolledGun);
            _availableInventory.Remove(rolledGun);
        }
    }


    // add logic to spawn holders and place them in shop
    private void CreateItemFromWeapon(IWeapon.GunID id)
    {

    }

}
