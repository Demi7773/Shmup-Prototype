using ReworkedWeapons;
using UnityEngine;

public class NewPlayerController : MonoBehaviour
{

    [Header("Dependencies")]
    [SerializeField] private PlayerReferences _playerReferences;
    private InputReaderAdvanced _input; /*=> _playerReferences.Input;*/
    private PlayerSensors _sensors; /*=> _playerReferences.PlayerSensors;*/
    private NewWeaponManager _weapons;
    private DodgeRoll _dodge; /* => _playerReferences.Dodge;*/
    //private WeaponController _weapons; /*=> _playerReferences.WeaponController;*/

    [Header("Look")]
    private Camera _cam;
    [SerializeField] private Texture2D _aimCursorTexture;
    [SerializeField] private float _camZOffsetForLook = 40.0f;
    [SerializeField] private Vector3 _aimPos;
    public Vector3 AimPos => _aimPos;

    [Header("Stats")]
    [SerializeField] private float _speed = 5.0f;
    [SerializeField] private LayerMask _terrainLayers;

    [Header("Status")]
    public bool CanMove = true;





    private void Awake()
    {
        _cam = Camera.main;
        Cursor.SetCursor(_aimCursorTexture, /*Vector2.zero*/ new Vector2(16, 16), CursorMode.Auto);

        _input = _playerReferences.Input;
        _sensors = _playerReferences.PlayerSensors;
        _weapons = _playerReferences.WeaponManager;
        _dodge = _playerReferences.Dodge;
    }

    private void Update()
    {

        if (_input == null)
        {
            Debug.Log("PlayerInput null");
            return;
        }

        Look();
        if (_dodge.IsDodging)
        {
            return;
        }

        Vector2 moveInput = _input.MoveInputValue;
        if (CanMove)
        {
            Move(moveInput);
        }

        HandleActiveInputActions(moveInput);
    }

    private void Move(Vector2 moveInput)
    {
        Vector3 dir = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        Ray ray = new Ray(transform.position, dir);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, 1.0f, _terrainLayers))
        {
            //Debug.Log("Raycast hit Terrain, dir: " + dir); 
            dir += hitInfo.normal;
            //Debug.Log("New dir: " + dir);
        }
        transform.position += dir * (_speed * Time.deltaTime);
    }

    private void Look()
    {
        Vector3 v3 = new Vector3(_input.LookInputValue.x, _input.LookInputValue.y, _camZOffsetForLook);
        Vector3 lookPos = _cam.ScreenToWorldPoint(v3);
        //Debug.Log("LookPos: " + lookPos);
        _aimPos = lookPos;
        Vector3 lookDirection = _aimPos - transform.position;
        transform.rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
    }





    private void HandleActiveInputActions(Vector2 moveInput)
    {
        InputReaderAdvanced.GameplayInputs input = _input.LastInput;
        switch (input)
        {
            case InputReaderAdvanced.GameplayInputs.None:
            {
                break;
            }

            case InputReaderAdvanced.GameplayInputs.Shoot:
            {
                if (_weapons.WeaponsBusy())
                {
                    break;
                }

                if (_weapons.WeaponHasAmmoForShot())
                {
                    HandleShoot();
                    break;
                }

                if (_weapons.CanWeaponReload())
                {
                    HandleReload();
                }
                break;
            }

            case InputReaderAdvanced.GameplayInputs.Reload:
            {
                if (_weapons.WeaponsBusy())
                {
                    break;
                }
                HandleReload();
                break;
            }

            case InputReaderAdvanced.GameplayInputs.Dodge:
            {
                if (_dodge.CanDodge())
                {
                    if (moveInput == Vector2.zero)
                    {
                        HandleDodge(transform.forward);
                        //Debug.Log("Dodge with no move input, dashing facing direction instead");
                        break;
                    }

                    Vector3 direction = new Vector3(moveInput.x, transform.position.y, moveInput.y).normalized;
                    HandleDodge(direction);
                    //Debug.Log("Dodge input direction: " + direction);
                }
                break;
            }

            case InputReaderAdvanced.GameplayInputs.SwapWeapon:
                {
                    HandleSwapWeapon();
                    break;
                }

            case InputReaderAdvanced.GameplayInputs.Interact:
                {
                    HandleInteract();
                    break;
                }

            case InputReaderAdvanced.GameplayInputs.SpecialAttack:
                {
                    HandleSpecialAttack();
                    break;
                }
        }
    }

    private void HandleSpecialAttack()
    {
        if (_weapons.CurrentSpecial.CanUseSpecial())
        {
            _weapons.UseSpecial();

            // Add condtional clause to check if shoot input should be cleared
            _input.ClearInputs();
        }
    }

    private void HandleInteract()
    {
        Interactible interactible = _sensors.ClosestInteractible;
        if (interactible != null)
        {
            interactible.Interact(_playerReferences);
            _input.ClearInputs();
        }
    }

    private void HandleSwapWeapon()
    {
        SpecialAttack special = _weapons.CurrentSpecial;
        if (special.MySpecialBehavior == ISpecialAttack.SpecialBehavior.Mode)
        {
            EnterMode specialMode = special as EnterMode;
            if (specialMode == null)
            {
                Debug.Log("WeaponType is Mode but EnterMode is null on cast");
                return;
            }

            if (specialMode.IsModeActive)
            {
                Debug.Log("Tried switching weapons while mode active, returning");
                return;
            }
        }

        bool isNext = true;
        //Debug.Log("Swap Vector2 Scroll value read: " + _input.SwapInputValue);
        if (_input.SwapInputValue.y < 0.0f)
        {
            isNext = false;
        }
        _weapons.SwapWeaponToNext(isNext);
        _input.ClearInputs();
    }

    private void HandleReload()
    {
        if (_weapons.CanWeaponReload())
        {
            _input.ClearInputs();
            _weapons.StartReload();
        }
    }

    private void HandleShoot()
    {
        if (!_weapons.IsCurrentWeaponAuto())
        {
            _input.ClearInputs();
        }
        _weapons.Shoot();
    }

    private void HandleDodge(Vector3 direction)
    {
        _dodge.StartDodging(direction);
    }

}
