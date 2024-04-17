

public interface IWeapon
{

    public enum GunID
    {
        Glockerson,
        DoubleShot,
        TrustyRevolver,
        CloseEncounter,
        Deagle,
        Fractal,
        MoneyGun
    }
    public enum AmmoType
    {
        Bullets,
        Energy,
        Money,
        Special
    }
    public enum ShootingType
    {
        SingleFire,
        SemiAuto,
        Auto,
        HoldToCharge
    }





    public void Init(PlayerReferences playerRef);



    public bool HasAmmoForShot();
    public bool CanReload();
    public bool IsInMode();



    public float CurrentDamage();
    public int CurrentAmmo();
    public int MaxAmmo();
    public float CurrentTimeBetweenShots();
    public float CurrentReloadDuration();
    public float CurrentAimOffset();



    public void CalculateAmmo();
    public void ToggleUseModeStats(bool onOff);
    public void Fire();
    public void Reload();

}
