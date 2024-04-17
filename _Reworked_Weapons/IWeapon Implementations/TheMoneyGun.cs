using UnityEngine;
using UnityEngine.VFX;

namespace ReworkedWeapons
{
    public class TheMoneyGun : Weapon
    {

        [Space(30)]
        [Header("TheMoneyGun")]
        [Header("Pool")]
        [SerializeField] protected ProjectilePool _pool;
        [SerializeField] protected int _poolSize = 100;
        [SerializeField] protected Projectile _projectilePrefab;
        public ProjectilePool Pool => _pool;
        [Space(30)]
        [Header("Settings")]
        [SerializeField] protected int _currentAmmo = 0;
        [SerializeField] protected Transform _shootPos;
        [SerializeField] protected int _ammoConsumedPerShot = 1;
        [SerializeField] protected bool _isReloadable = false;
        [SerializeField] protected float _baseDamage = 14.0f;
        [SerializeField] protected float _baseTimeBetweenShots = 0.1f;
        [SerializeField] protected float _baseReloadDuration = 0.0f;
        [SerializeField] protected float _baseAimOffset = 12.0f;
        [Space(30)]
        [Header("Debug")]
        //[SerializeField] protected bool _isInMode = false;
        [SerializeField] protected VisualEffect _shootVFX;
        [SerializeField] protected PlayerMoney _playerMoney;



        public override void Init(PlayerReferences playerRef)
        {
            _playerRef = playerRef;
            _playerWeapons = _playerRef.WeaponManager;
            _pool.InitializePool(_projectilePrefab, _poolSize);

            _shootVFX = _playerWeapons.PistolVFX;
            //_shootVFX = _playerWeapons.MoneyGunVFX;

            _playerMoney = _playerRef.PlayerMoney;
            // needs ISpecialAttack integration
            (_mySpecial as DropMoneyBag).PlayerMoneyReference(_playerMoney);

            CalculateAmmo();
        }



        public override bool HasAmmoForShot()
        {
            if (_playerMoney.Money > 0)
                return true;
            return false;
        }
        public override bool CanReload()
        {
            return false;
        }
        public override bool IsInMode()
        {
            return false;
        }



        public override float CurrentDamage()
        {
            return DamageBasedOnMoney(_playerMoney.Money);
        }
        public override float CurrentAimOffset()
        {
            return _baseAimOffset;
        }
        public override float CurrentReloadDuration()
        {
            return 0.0f;
        }
        public override float CurrentTimeBetweenShots()
        {
            return _baseTimeBetweenShots;
        }
        public override int CurrentAmmo()
        {
            return (int)(_playerMoney.Money / _ammoConsumedPerShot);
        }
        public override int MaxAmmo()
        {
            return (int)(_playerMoney.Money / _ammoConsumedPerShot);
        }



        public override void CalculateAmmo()
        {
            _currentAmmo = (int)(_playerMoney.Money / _ammoConsumedPerShot);
        }
        public override void Fire()
        {
            var projectile = _pool.GetObjectFromPool();
            FireProjectileAt(projectile, _shootPos);
            _shootVFX.Play();

            _playerMoney.LoseMoney(_ammoConsumedPerShot);
            CalculateAmmo();
            Debug.Log("Money Gun used " + _ammoConsumedPerShot + " money");
        }
        public override void Reload()
        {
            return;
        }
        public override void ToggleUseModeStats(bool onOff)
        {
            return;
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

        // MoneyGun Specific
        protected float DamageBasedOnMoney(int money)
        {
            float dmg = _baseDamage;

            // here just to be safe, because apparently switch with reference is not reliable
            // ( tho it should already be cached when passing parameter)
            int cash = money;
            switch (cash)
            {
                default:
                    {
                        dmg = 50;
                        break;
                    }
                case int n when n < 50:
                    {
                        dmg = 20;
                        break;
                    }
                case int n when n < 100:
                    {
                        dmg = 25;
                        break;
                    }
                case int n when n < 200:
                    {
                        dmg = 30;
                        break;
                    }
                case int n when n < 300:
                    {
                        dmg = 35;
                        break;
                    }
                case int n when n < 500:
                    {
                        dmg = 40;
                        break;
                    }
                case int n when n < 1000:
                    {
                        dmg = 45;
                        break;
                    }
            }

            Debug.Log("PlayerMoney: " + cash + ", MoneyGun CurrentDamage set to: " + dmg);
            return dmg;
        }

    }
}