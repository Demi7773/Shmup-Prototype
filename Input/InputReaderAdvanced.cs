using Demi;
using ReworkedWeapons;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

//[CreateAssetMenu(fileName = "InputReaderAdvanced")]
public class InputReaderAdvanced : MonoBehaviour, GameInput.IGameplayActions, GameInput.IUIActions
{

    private GameInput _gameInput;

    [SerializeField] private bool _holdDownToShoot = false;

    //public static InputReaderAdvanced Instance;

    //public event Action<Vector2> MoveEvent;
    //public event Action<Vector2> LookEvent;
    //public event Action<Vector2> SwapWeaponEvent;
    //public event Action ShootEvent;
    //public event Action ReloadEvent;
    //public event Action SpecialAttackEvent;
    //public event Action InteractEvent;
    //public event Action JumpEvent;
    //public event Action JumpCancelEvent;

    public event Action PauseEvent;
    public event Action ResumeEvent;



    public Vector2 LookInputValue;
    public Vector2 MoveInputValue;
    public Vector2 SwapInputValue;

    public GameplayInputs LastInput;
    public enum GameplayInputs
    {
        None,
        Interact,
        Dodge,
        SwapWeapon,
        Shoot,
        SpecialAttack,
        Reload
    }

    private float _timer = 0.0f;
    [SerializeField] private float _saveInputsFor = 0.2f;



    private void OnEnable()
    {
        //if (Instance == null)
        //{
        //    Instance = this;
        //    Debug.Log("InputReaderAdvanced Instance set");
        //}
        //else
        //{
        //    Debug.Log("InputReaderAdvanced Instance already exists, Destroying this!");
        //    Destroy(gameObject);
        //}

        if (_gameInput == null)
        {
            _gameInput = new GameInput();

            _gameInput.Gameplay.SetCallbacks(this);
            _gameInput.UI.SetCallbacks(this);

            //SetGameplay();
        }

        NewWeaponManager.OnSwapWeapon += OnSwapWeapon;
    }

    private void OnDisable()
    {
        //StopAllCoroutines();

        NewWeaponManager.OnSwapWeapon -= OnSwapWeapon;
    }




        // Pausing
    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            PauseEvent?.Invoke();
            SetUI();
        }
    }
    public void OnResume(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            ResumeEvent?.Invoke();
            SetGameplay();
        }
    }
    public void SetGameplay()
    {
        _gameInput.UI.Disable();
        _gameInput.Gameplay.Enable();
        //Debug.Log("Input set to Gameplay");
    }
    public void SetUI()
    {
        _gameInput.Gameplay.Disable();
        _gameInput.UI.Enable();

        //StopAllCoroutines();
        //Debug.Log("Input set to UI");
    }


        // Weapon Fire Types
    private void OnSwapWeapon(Weapon weapon)
    {
        switch (weapon.MyShooting)
        {
            case IWeapon.ShootingType.SingleFire:
                _holdDownToShoot = false;
                break;
            case IWeapon.ShootingType.SemiAuto:
                _holdDownToShoot = false;
                break;
            case IWeapon.ShootingType.Auto:
                //Debug.Log("InputReader fire type set to Auto");
                _holdDownToShoot = true;
                break;
            case IWeapon.ShootingType.HoldToCharge:
                _holdDownToShoot = true;
                break;
        }
    }


        // Handle Look and Move inputs
    public void OnLook(InputAction.CallbackContext context)
    {
        //LookEvent?.Invoke(context.ReadValue<Vector2>());
        LookInputValue = context.ReadValue<Vector2>();
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        //MoveEvent?.Invoke(context.ReadValue<Vector2>());
        MoveInputValue = context.ReadValue<Vector2>();
    }


        // Handle Action Inputs
    public void OnShoot(InputAction.CallbackContext context)
    {

        if (_holdDownToShoot)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                LastInput = GameplayInputs.Shoot;
                _timer = 0.0f;
            }
            if (context.phase == InputActionPhase.Canceled)
            {
                LastInput = GameplayInputs.None;
                _timer = 0.0f;
            }
            return;
        }

        if (context.phase == InputActionPhase.Performed)
        {
            LastInput = GameplayInputs.Shoot;
            _timer = 0.0f;
        }
    }

    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            LastInput = GameplayInputs.Reload;
            _timer = 0.0f;
        }
    }

    public void OnSpecialAttack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            LastInput = GameplayInputs.SpecialAttack;
            _timer = 0.0f;
        }
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            LastInput = GameplayInputs.Dodge;
            _timer = 0.0f;
        }
    }


    public void OnSwapWeapon(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            SwapInputValue = context.ReadValue<Vector2>();
            LastInput = GameplayInputs.SwapWeapon;
            _timer = 0.0f;
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            LastInput = GameplayInputs.Interact;
            _timer = 0.0f;
        }
    }



    public void ClearInputs()
    {
        LastInput = GameplayInputs.None;
        _timer = 0.0f;
    }

    private void Update()
    {
        if (LastInput == GameplayInputs.None)
        {
            return;
        }

        if (LastInput == GameplayInputs.Shoot)
        {
            if (_holdDownToShoot)
            {
                return;
            }
        }

        _timer += Time.deltaTime;
        if (_timer > _saveInputsFor)
        {
            //Debug.Log("Last input " + LastInput + " expired");
            LastInput = GameplayInputs.None;
            _timer = 0.0f;
        }
    }

}
