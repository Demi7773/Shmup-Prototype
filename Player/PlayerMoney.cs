using System;
using UnityEngine;

public class PlayerMoney : MonoBehaviour
{

    [SerializeField] private int _money;
    public int Money => _money;

    public static event Action<PlayerMoney> OnPlayerMoneyChange;



    private void OnEnable()
    {
        UIEvents.OnUpdateAllHUD += UpdateUI;
    }
    private void OnDisable()
    {
        UIEvents.OnUpdateAllHUD -= UpdateUI;
    }



    public void Init(int cash)
    {
        _money = cash;
        UpdateUI();
    }




    private void Start()
    {
        UpdateUI();
    }

    public void GetMoney(int amount)
    {
        _money += amount;
        UpdateUI();
    }

    public bool CanAfford(int amount)
    {
        if (_money >= amount)
        {
            return true;
        }
        return false;
    }
    public void LoseMoney(int amount) 
    {
        _money -= amount;
        UpdateUI();
    }

    private void UpdateUI()
    {
        OnPlayerMoneyChange?.Invoke(this);
    }

}
