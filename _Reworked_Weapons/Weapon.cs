using UnityEngine;
//using UnityEngine.VFX;


namespace ReworkedWeapons
{
    public abstract class Weapon : MonoBehaviour, IWeapon
    {

        [Header("Base Weapon Dependencies")]
        // Base Weapon Dependencies
        [SerializeField] protected IWeapon.GunID _myGunID;
        [SerializeField] protected IWeapon.ShootingType _myShooting;
        [SerializeField] protected IWeapon.AmmoType _myAmmo;
        [SerializeField] protected SpecialAttack _mySpecial;
        [SerializeField] protected Transform _weaponModel;
        [SerializeField] protected WeaponData _myData;
        protected PlayerReferences _playerRef;
        protected NewWeaponManager _playerWeapons;

        public IWeapon.GunID MyGunID => _myGunID;
        public IWeapon.ShootingType MyShooting => _myShooting;
        public IWeapon.AmmoType MyAmmo => _myAmmo;
        public SpecialAttack MySpecial => _mySpecial;
        public Transform WeaponModel => _weaponModel;
        // Data
        public WeaponData MyData => _myData;
        public string Name => _myData.Name;
        public Sprite Sprite => _myData.Sprite;
        public Sprite SpecialSprite => _mySpecial.Sprite;
        public AudioClip FireAudio => _myData.FireAudio;
        public AudioClip EmptyClipAudio => _myData.EmptyClipAudio;
        public AudioClip ReloadStartAudio => _myData.ReloadStartAudio;
        public AudioClip ReloadedAudio => _myData.ReloadedAudio;
        public NewWeaponManager PlayerWeapons => _playerWeapons;


      




        // IWeapon
        public abstract bool CanReload();
        public abstract bool HasAmmoForShot();
        public abstract bool IsInMode();


        public abstract int CurrentAmmo();
        public abstract int MaxAmmo();

        public abstract float CurrentAimOffset();
        public abstract float CurrentReloadDuration();
        public abstract float CurrentTimeBetweenShots();
        public abstract float CurrentDamage();



        public abstract void Init(PlayerReferences playerRef);

        public abstract void CalculateAmmo();
        public abstract void Fire();
        public abstract void Reload();
        public abstract void ToggleUseModeStats(bool onOff);
        // IWeapon

    }
}
