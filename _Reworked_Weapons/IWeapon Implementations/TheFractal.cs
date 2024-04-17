using UnityEngine;
using UnityEngine.VFX;

namespace ReworkedWeapons
{
    public class TheFractal : Weapon
    {

        [Space(30)]
        [Header("TheFractal")]
        [Space(10)]
        [Header("Pool")]
        [SerializeField] protected ProjectilePool _pool;
        [SerializeField] protected int _poolSize = 100;
        [SerializeField] protected FractalProjectileRoot _fractalProjectileRoot;
        [SerializeField] protected FractalProjectilePart _fractalPartPrefab;
        public ProjectilePool Pool => _pool;
        [Space(10)]
        [Header("Stats")]
        [SerializeField] protected int _currentAmmo = 0;
        [SerializeField] protected int _maxAmmo = 4;
        [SerializeField] protected Transform _shootPos1;
        [SerializeField] protected Transform _shootPos2;
        [SerializeField] protected Transform _shootPos3;
        [SerializeField] protected Transform _shootPos4;
        [SerializeField] protected int _ammoConsumedPerShot = 2;
        [SerializeField] protected bool _isReloadable = true;
        [SerializeField] protected float _baseDamage = 32.0f;
        [SerializeField] protected float _baseTimeBetweenShots = 0.4f;
        [SerializeField] protected float _baseReloadDuration = 1.6f;
        [SerializeField] protected float _baseAimOffset = 4.0f;
        [Space(30)]
        [Header("Debug")]
        [SerializeField] protected bool _isInMode = false;
        [SerializeField] protected VisualEffect _shootVFX;





        public override void Init(PlayerReferences playerRef)
        {
            _currentAmmo = MaxAmmo();
            _playerRef = playerRef;
            _playerWeapons = _playerRef.WeaponManager;
            _pool.InitializePool(_fractalProjectileRoot, _poolSize);

            _shootVFX = _playerWeapons.ShotgunVFX;
            //_shootVFX = _playerWeapons.FractalVFX;

            _currentAmmo = _maxAmmo;
            _pool.InitSecondaryPool(_fractalPartPrefab, 100);
        }



        public override bool HasAmmoForShot()
        {
            if (_currentAmmo > _ammoConsumedPerShot)
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
            return _baseDamage;
        }
        public override float CurrentTimeBetweenShots()
        {
            return _baseTimeBetweenShots;
        }
        public override float CurrentReloadDuration()
        {
            return _baseReloadDuration;
        }
        public override float CurrentAimOffset()
        {
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
            _shootVFX.Play();
            _currentAmmo -= _ammoConsumedPerShot;
            //PlayerEvents.PlayerShootsWeapon(this);

            if (!_isInMode)
            {
                FireDefault();
                return;
            }

            FireInMode();
        }
        public override void Reload()
        {
            _currentAmmo = MaxAmmo();

            //PlayerEvents.PlayerCompletesReload(this);
        }



        protected void FireDefault()
        {
            var projectile1 = _pool.GetObjectFromPool();
            var projectile2 = _pool.GetObjectFromPool();

            (projectile1 as FractalProjectileRoot).ToggleSplitIntoFour(false);
            (projectile2 as FractalProjectileRoot).ToggleSplitIntoFour(false);

            FireProjectileAt(projectile1, _shootPos1);
            FireProjectileAt(projectile2, _shootPos2);
        }

        protected void FireInMode()
        {
            var projectile1 = _pool.GetObjectFromPool();
            var projectile2 = _pool.GetObjectFromPool();
            var projectile3 = _pool.GetObjectFromPool();
            var projectile4 = _pool.GetObjectFromPool();

            (projectile1 as FractalProjectileRoot).ToggleSplitIntoFour(true);
            (projectile2 as FractalProjectileRoot).ToggleSplitIntoFour(true);
            (projectile3 as FractalProjectileRoot).ToggleSplitIntoFour(true);
            (projectile4 as FractalProjectileRoot).ToggleSplitIntoFour(true);

            FireProjectileAt(projectile1, _shootPos1);
            FireProjectileAt(projectile2, _shootPos2);
            FireProjectileAt(projectile3, _shootPos3);
            FireProjectileAt(projectile4, _shootPos4);
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