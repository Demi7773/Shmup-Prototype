using UnityEngine;
using UnityEngine.VFX;

namespace ReworkedWeapons
{
    public class TheDeagle : Weapon
    {

        [Space(30)]
        [Header("TheDeagle")]
        [Header("Pool")]
        [SerializeField] protected ProjectilePool _pool;
        [SerializeField] protected int _poolSize = 100;
        [SerializeField] protected Projectile _projectilePrefab;
        public ProjectilePool Pool => _pool;
        [Space(10)]
        [Header("Stats")]
        [SerializeField] protected Transform _shootPos;
        [SerializeField] protected int _currentAmmo = 0;
        [SerializeField] protected int _maxAmmo = 10;
        [SerializeField] protected int _ammoConsumedPerShot = 1;
        [SerializeField] protected bool _isReloadable = true;
        [SerializeField] protected float _baseDamage = 25.0f;
        [SerializeField] protected float _baseTimeBetweenShots = 0.35f;
        [SerializeField] protected float _baseReloadDuration = 1.5f;
        [SerializeField] protected float _baseAimOffset = 7.0f;
        [Space(10)]
        [Header("Mode Active Stats")]
        [SerializeField] private float _modeDamage = 40.0f;
        [SerializeField] private float _modeTimeBetweenShots = 0.25f;
        [SerializeField] private float _modeReloadDuration = 1.2f;
        [SerializeField] private float _modeAimOffset = 4.0f;
        [Space(30)]
        [Header("Debug")]
        [SerializeField] protected bool _isInMode = false;
        [SerializeField] protected VisualEffect _shootVFX;






        public override void Init(PlayerReferences playerRef)
        {
            _currentAmmo = MaxAmmo();
            _playerRef = playerRef;
            _playerWeapons = _playerRef.WeaponManager;
            _pool.InitializePool(_projectilePrefab, _poolSize);

            _shootVFX = _playerWeapons.PistolVFX;
            //_shootVFX = _playerWeapons.DeagleVFX;

            _currentAmmo = _maxAmmo;
        }



        public override bool HasAmmoForShot()
        {
            if (_currentAmmo > 0)
                return true;
            return false;
        }

        public override bool CanReload()
        {
            if (_currentAmmo < _maxAmmo)
                return true;
            return false;
        }
        public override bool IsInMode()
        {
            return _isInMode;
        }




        public override int CurrentAmmo()
        {
            return _currentAmmo;
        }
        public override int MaxAmmo()
        {
            return _maxAmmo;
        }
        public override float CurrentDamage()
        {
            if (_isInMode)
            {
                return _modeDamage;
            }
            return _baseDamage;
        }
        public override float CurrentTimeBetweenShots()
        {
            if (_isInMode)
            {
                return _modeTimeBetweenShots;
            }
            return _baseTimeBetweenShots;
        }
        public override float CurrentReloadDuration()
        {
            if (_isInMode)
            {
                return _modeReloadDuration;
            }
            return _baseReloadDuration;
        }
        public override float CurrentAimOffset()
        {
            if (_isInMode)
            {
                return _modeAimOffset;
            }
            return _baseAimOffset;
        }




        public override void CalculateAmmo()
        {
            return;
        }
        public override void ToggleUseModeStats(bool onOff)
        {
            _isInMode = onOff;
        }
        public override void Fire()
        {
            var projectile1 = _pool.GetObjectFromPool();
            FireProjectileAt(projectile1, _shootPos);
            _shootVFX.Play();
            _currentAmmo -= _ammoConsumedPerShot;

            //PlayerEvents.PlayerShootsWeapon(this);
        }
        public override void Reload()
        {
            _currentAmmo = MaxAmmo();

            //PlayerEvents.PlayerCompletesReload(this);
        }



        protected void FireProjectileAt(Projectile projectile, Transform point)
        {
            projectile.transform.position = point.position;
            projectile.transform.rotation = RollOffsetRotationOnYAxis(point.rotation, CurrentAimOffset());
            projectile.gameObject.SetActive(true);
            projectile.Init(projectile.transform.forward, CurrentDamage());
        }
        protected Quaternion RollOffsetRotationOnYAxis(Quaternion startingRotation, float offsetRange)
        {
            float xOffset = Random.Range(-offsetRange, offsetRange);
            Quaternion rotation = startingRotation * Quaternion.Euler(Vector3.up * xOffset);
            return rotation;
        }

    }
}