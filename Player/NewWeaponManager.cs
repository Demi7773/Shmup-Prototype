using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace ReworkedWeapons
{
    public class NewWeaponManager : MonoBehaviour
    {

        [Header("All Weapons Prefabs")]
        [SerializeField] private Weapon _glockersonPrefab;
        [SerializeField] private Weapon _doubleShotPrefab;
        [SerializeField] private Weapon _closeEncounterPrefab;
        [SerializeField] private Weapon _trustyRevolverPrefab;
        [SerializeField] private Weapon _fractalPrefab;
        [SerializeField] private Weapon _deaglePrefab;
        [SerializeField] private Weapon _moneyGunPrefab;

        [Space(30)]
        [Header("Setup")]
        //[SerializeField] private Weapon _startingWeapon;
        [SerializeField] private PlayerReferences _playerRef;
        [SerializeField] private PlayerEnergy _energy;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private float _swapWeaponDuration = 0.3f;
        [SerializeField] private AudioClip _swapWeaponsAudio;
        [SerializeField] private VisualEffect _pistolVFX;
        [SerializeField] private VisualEffect _shotgunVFX;
        //[SerializeField] private VisualEffect _blasterVFX;
        public VisualEffect PistolVFX => _pistolVFX;
        public VisualEffect ShotgunVFX => _shotgunVFX;
        //public VisualEffect BlasterVFX => _blasterVFX;
        [Space(30)]
        [Header("Debug")]
        [Header("Weapon Status")]
        [SerializeField] private List<Weapon> _heldWeapons = new List<Weapon>();
        [SerializeField] private List<IWeapon.GunID> _heldWeaponsIDs = new List<IWeapon.GunID>();
        [SerializeField] private int _selectedWeaponIndex = 0;
        [SerializeField] private Weapon _currentWeapon;
        public Weapon CurrentWeapon => _currentWeapon;
        public int SelectedWeaponIndex => _selectedWeaponIndex;
        public SpecialAttack CurrentSpecial => _currentWeapon.MySpecial;
        public IWeapon.AmmoType CurrentAmmoType => _currentWeapon.MyAmmo;
        [Space(10)]
        [Header("Controller state")]
        [SerializeField] private State _state;
        public enum State
        {
            Idle,
            Shooting,
            Reloading,
            Swapping
        }
        [Space(10)]
        [Header("Timer")]
        [SerializeField] private float _timer = 0.0f;
        [SerializeField] private float _actionsLockedTimerTarget = 0.0f;
        [SerializeField] private float _reloadStarted = 0.0f;
        [Space(10)]
        [Header("Mode")]
        [SerializeField] private bool _isInMode = false;
        [SerializeField] private EnterMode _usedMode;
        [SerializeField] private float _modeTimerEndsTarget = 0.0f;
        [SerializeField] private float _modeStartTimer = 0.0f;


        // Weapon updates
        public static event Action<NewWeaponManager> OnHeldGunsChange;
        public static event Action<Weapon> OnSwapWeapon;
        public static event Action<Weapon> OnWeaponStatusChange;
        // Reload UI updates
        public static event Action<bool> ReloadIsHappening;
        public static event Action<float> ReloadProgress;
        // Special updates
        public static event Action<SpecialAttack> UsedSpecial;








        public List<IWeapon.GunID> HeldWeaponsIDsList()
        {
            List<IWeapon.GunID> weaponIDs = new List<IWeapon.GunID>();
            foreach (var weaponID in _heldWeaponsIDs)
            {
                weaponIDs.Add(weaponID);
            }
            return weaponIDs;
        }


        public void Init(List<IWeapon.GunID> weaponsToLoad)
        {
            _heldWeapons.Clear();
            StartCoroutine(InitGunsSequence(weaponsToLoad));
        }
        private IEnumerator InitGunsSequence(List<IWeapon.GunID> weaponsToLoad)
        {
            foreach (IWeapon.GunID weaponID in weaponsToLoad)
            {
                AddWeaponByID(weaponID);
                yield return null;
            }
            SwapWeapon(0);
            OnHeldGunsChange?.Invoke(this);
            //Debug.Log("Weapons updated from RunData");    
        }

        private void AddWeaponByID(IWeapon.GunID weaponID)
        {
            _heldWeaponsIDs.Add(weaponID);
            Weapon weapon = null;
            switch (weaponID)
            {
                default:
                    Debug.Log("GunID not recognized! Adding Glockerson");
                    weapon = _glockersonPrefab;
                    break;
                case IWeapon.GunID.Glockerson:
                    weapon = _glockersonPrefab;
                    break;
                case IWeapon.GunID.DoubleShot:
                    weapon = _doubleShotPrefab;
                    break;
                case IWeapon.GunID.TrustyRevolver:
                    weapon = _trustyRevolverPrefab;
                    break;
                case IWeapon.GunID.CloseEncounter:
                    weapon = _closeEncounterPrefab;
                    break;
                case IWeapon.GunID.Deagle:
                    weapon = _deaglePrefab;
                    break;
                case IWeapon.GunID.Fractal:
                    weapon = _fractalPrefab;
                    break;
                case IWeapon.GunID.MoneyGun:
                    weapon = _moneyGunPrefab;
                    break;
            }

            var weaponInstance = Instantiate(weapon, transform.position, transform.rotation, transform);
            weaponInstance.Init(_playerRef);
            _heldWeapons.Add(weaponInstance);
        }




        public float ThisModeDurationRatio()
        {
            if (!_isInMode)
            {
                return 0.0f;
            }
            return (_timer - _modeStartTimer) / (_modeTimerEndsTarget - _modeStartTimer);
        }




        // Get / Set HeldWeapons
        public int HeldWeaponsCount()
        {
            return _heldWeapons.Count;
        }
        public Weapon GetWeaponBySlot(int slot)
        {
            return _heldWeapons[slot];
        }
        public void AddWeaponToHeld(Weapon weapon)
        {
            StartCoroutine(AddingWeaponSequence(weapon));
        }
        private IEnumerator AddingWeaponSequence(Weapon weapon)
        {
            _heldWeaponsIDs.Add(weapon.MyGunID);

            var weaponInstance = Instantiate(weapon, transform.position, transform.rotation, transform);
            _heldWeapons.Add(weaponInstance);
            yield return null;
            weaponInstance.Init(_playerRef);
            OnHeldGunsChange?.Invoke(this);
            yield return null;
            SwapWeapon(HeldWeaponsCount() - 1);
            Debug.Log("Added new weapon: " + weapon);
        }



        // Can do sth checks
        public bool IsCurrentWeaponAuto()
        {
            if (_currentWeapon.MyShooting == IWeapon.ShootingType.Auto)
                return true;
            return false;
        }
        public bool WeaponsBusy()
        {
            if (_state == State.Idle)
                return false;
            return true;
        }
        public bool WeaponHasAmmoForShot()
        {
            return _currentWeapon.HasAmmoForShot();
        }
        public bool CanWeaponReload()
        {
            return _currentWeapon.CanReload();
        }



        // Controller actions
        public void SwapWeaponToNext(bool toNextWeapon)
        {
            if (_state == State.Swapping)
                return;
            if (_heldWeapons.Count <= 1)
                return;

            if (_state == State.Reloading)
            {
                //Debug.Log("Swapping cancels reload");
                CancelReload();
            }

            if (toNextWeapon)
            {
                _selectedWeaponIndex++;
                if (_selectedWeaponIndex > _heldWeapons.Count - 1)
                {
                    _selectedWeaponIndex = 0;
                }
            }
            else
            {
                _selectedWeaponIndex--;
                if (_selectedWeaponIndex < 0)
                {
                    _selectedWeaponIndex = _heldWeapons.Count - 1;
                }
            }

            SwapWeapon(_selectedWeaponIndex);
        }
        public void SwapWeaponToIndex(int index)
        {
            if (_state == State.Swapping)
                return;

            if (index > _heldWeapons.Count)
            {
                Debug.Log("Index was out of range");
                return;
            }

            _selectedWeaponIndex = index;
            SwapWeapon(_selectedWeaponIndex);
        }
        public void CancelReload()
        {
            //Debug.Log("Reload Cancelled");
            _actionsLockedTimerTarget = _timer - 1.0f;
            ReloadIsHappening?.Invoke(false);
        }
        public void SwapWeapon(int index)
        {
            _selectedWeaponIndex = index;
            _audioSource.PlayOneShot(_swapWeaponsAudio, 0.5f);
            _actionsLockedTimerTarget = _timer + _swapWeaponDuration;

            foreach (Weapon weap in _heldWeapons)
            {
                weap.gameObject.SetActive(false);
            }
            _currentWeapon = _heldWeapons[_selectedWeaponIndex];
            _currentWeapon.gameObject.SetActive(true);

            _state = State.Swapping;
            OnSwapWeapon?.Invoke(_currentWeapon);
            OnWeaponStatusChange?.Invoke(_currentWeapon);
        }



        // Weapon actions
        public void Shoot()
        {
            if (_state != State.Idle)
            {
                //Debug.Log("Shoot called when state was not Idle even though check should have been made, returning");
                return;
            }

            _actionsLockedTimerTarget = _timer + _currentWeapon.CurrentTimeBetweenShots()/*_shootingDuration*/;
            _audioSource.PlayOneShot(_currentWeapon.MyData.FireAudio);

            _state = State.Shooting;
            _currentWeapon.Fire();
            OnWeaponStatusChange?.Invoke(_currentWeapon);
        }
        public void StartReload()
        {
            if (_state != State.Idle)
            {
                return;
            }

            //_audioSource.PlayOneShot(_currentWeapon.ReloadingAudio);
            _actionsLockedTimerTarget = _timer + _currentWeapon.CurrentReloadDuration() /*_reloadingDuration*/;
            _reloadStarted = _timer;
            _state = State.Reloading;
            ReloadIsHappening?.Invoke(true);
        }
        public void UseSpecial()
        {
            var special = CurrentSpecial;
            special.UseSpecial();
            UsedSpecial?.Invoke(special);
        }



        // Behavior
        private void OnEnable()
        {
            EnterMode.OnEnterMode += OnEnteredMode;
            EnterMode.OnExitMode += OnExitedMode;

            UIEvents.OnUpdateAllHUD += OnFullHUDUpdate;

            EnemyManager.OnNewWave += OnNewWave;
        }
        private void OnDisable()
        {
            EnterMode.OnEnterMode -= OnEnteredMode;
            EnterMode.OnExitMode -= OnExitedMode;

            UIEvents.OnUpdateAllHUD -= OnFullHUDUpdate;

            EnemyManager.OnNewWave -= OnNewWave;
        }

        private void OnNewWave(int index)
        {
            foreach (Weapon weapon in _heldWeapons)
            {
                if (weapon.MyAmmo == IWeapon.AmmoType.Bullets)
                {
                    weapon.Reload();
                    //Debug.Log("Weapon reloaded on New Wave called");
                }
            }
        }


        private void OnFullHUDUpdate()
        {
            //OnHeldGunsChange?.Invoke(this);
        }
        // Mode
        private void OnEnteredMode(EnterMode specialMode)
        {
            EnterMode.Mode mode = specialMode.ThisMode;

            switch (mode)
            {
                default:
                    Debug.Log("Entered mode but ModeType not recognized!");
                    break;

                case EnterMode.Mode.SlowTime:
                    //if (specialMode is not SlowDownTime)
                    //{
                    //    Debug.Log("EnterMode " + specialMode + " is set as Mode:SlowTime but is not SlowDownTime on cast");
                    //    break;
                    //}
                    SlowDownTime slowTime = specialMode as SlowDownTime;
                    PlayerEvents.SlowDownTimeStarts(slowTime.SlowTimeTo/*, slowTime.UseDuration*/);
                    break;

                case EnterMode.Mode.Invulnerability:
                    //if (specialMode is not Invulnerability)
                    //{
                    //    Debug.Log("EnterMode " + specialMode + " is set as Mode:Invulnerability but is not Invulnerability on cast");
                    //    break;
                    //}
                    _playerRef.PlayerHP.InvulnerabilityMode(specialMode.UseDuration);
                    break;

                case EnterMode.Mode.DoubleUp:
                    //if (specialMode is not DoubleUp)
                    //{
                    //    Debug.Log("EnterMode " + specialMode + " is set as Mode:DoubleUp but is not DoubleUp on cast");
                    //    break;
                    //}
                    break;

                case EnterMode.Mode.BoostStats:
                    //if (specialMode is not BoostStatsMode)
                    //{
                    //    Debug.Log("EnterMode " + specialMode + " is set as Mode:BoostStats but is not BoostStats on cast");
                    //}
                    break;
            }

            _usedMode = specialMode;
            _modeStartTimer = _timer;
            _modeTimerEndsTarget = _timer + _usedMode.UseDuration;
            _isInMode = true;
            _currentWeapon.ToggleUseModeStats(true);
        }
        private void OnExitedMode(EnterMode specialMode)
        {
            EnterMode.Mode mode = specialMode.ThisMode;

            switch (mode)
            {
                default:
                    Debug.Log("Exited mode but ModeType not recognized!");
                    break;

                case EnterMode.Mode.SlowTime:
                    PlayerEvents.SlowDownTimeStops();
                    break;

                case EnterMode.Mode.Invulnerability:
                    //Debug.Log("Invulnerability ended called, real timer is in PlayerHP, test");
                    break;

                case EnterMode.Mode.DoubleUp:
                    break;

                case EnterMode.Mode.BoostStats:
                    break;
            }

            _isInMode = false;
            _currentWeapon.ToggleUseModeStats(false);
        }



        private void Awake()
        {
            _state = State.Idle;
            _timer = 0.0f;
            foreach (var weapon in _heldWeapons)
            {
                weapon.Init(_playerRef);
            }
            _selectedWeaponIndex = 0;

            SwapWeapon(_selectedWeaponIndex);
        }

        private void Update()
        {

            _currentWeapon.CalculateAmmo();
            _timer = GameManager.Instance.IngameUnscaledTimer;

            if (_isInMode)
            {
                if (_timer > _modeTimerEndsTarget)
                {
                    _usedMode.ExitThisMode();
                }
            }

            switch (_state)
            {
                case State.Idle:
                    break;

                case State.Shooting:
                    {
                        if (_timer > _actionsLockedTimerTarget)
                        {
                            _state = State.Idle;
                        }
                        break;
                    }

                case State.Reloading:
                    {
                        ReloadProgress?.Invoke((_timer - _reloadStarted) / _currentWeapon.CurrentReloadDuration());

                        if (_timer > _actionsLockedTimerTarget)
                        {
                            _currentWeapon.Reload();

                            _audioSource.PlayOneShot(_currentWeapon.MyData.ReloadedAudio);
                            _state = State.Idle;
                            ReloadIsHappening?.Invoke(false);
                            //OnWeaponStatusChange?.Invoke(_currentWeapon);
                        }
                        break;
                    }

                case State.Swapping:
                    {
                        if (_timer > _actionsLockedTimerTarget)
                        {
                            _currentWeapon.gameObject.SetActive(true);
                            _state = State.Idle;
                        }
                        break;
                    }
            }

            OnWeaponStatusChange?.Invoke(_currentWeapon);
        }

    }
}