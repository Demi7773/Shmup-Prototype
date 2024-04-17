using ReworkedWeapons;
using System;
using UnityEngine;

public class PlayerReferences : MonoBehaviour
{

    public static event Action<PlayerReferences> NewPlayerReference;


    /*[SerializeField] */private Transform _playerTransform => transform;
    [SerializeField] private PlayerMoney _playerMoney;
    [SerializeField] private PlayerHP _playerHP;
    [SerializeField] private PlayerSensors _playerSensors;
    [SerializeField] private PlayerEnergy _playerEnergy;
    [SerializeField] private DodgeRoll _dodge;

    [SerializeField] private NewWeaponManager _weaponManager;
    //[SerializeField] private WeaponController _weaponController;
    //[SerializeField] private PlayerInventory _playerInventory;

    private GameManager _gm;
    private InputReaderAdvanced _input;
    public GameManager GM => _gm;
    public InputReaderAdvanced Input => _input;



    public Transform PlayerTransform => _playerTransform;
    public PlayerMoney PlayerMoney => _playerMoney;
    //public PlayerInventory PlayerInventory => _playerInventory;
    public PlayerHP PlayerHP => _playerHP;
    public PlayerSensors PlayerSensors => _playerSensors;
    public PlayerEnergy PlayerEnergy => _playerEnergy;
    //public WeaponController WeaponController => _weaponController;
    public DodgeRoll Dodge => _dodge;

    public NewWeaponManager WeaponManager => _weaponManager;



    private void OnEnable()
    {
        NewPlayerReference?.Invoke(this);
    }

    public void Init(GameManager gm, InputReaderAdvanced input)
    {
        _gm = gm;
        _input = input;

        AdjustStatsByData(_gm.CurrentRunData);
        //Debug.Log("PlayerRef Init");
    }
    private void AdjustStatsByData(RunPlayerDataHolder data)
    {
        _playerHP.Init(_gm, data.PlayerMaxHP, data.PlayerCurrentHP);
        _playerMoney.Init(data.PlayerCash);
        _weaponManager.Init(data.HeldWeaponIDs);
        _playerEnergy.Init(data.PlayerMaxEnergy);
    }

}
