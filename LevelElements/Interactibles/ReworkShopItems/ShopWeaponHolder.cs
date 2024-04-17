using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ReworkedWeapons
{
    public class ShopWeaponHolder : Interactible
    {

        [Header("Setup")]
        [Header("Weapon")]
        [SerializeField] private WeaponData _heldWeaponData;
        private int _itemPrice;
        [Space(20)]
        [Header("UI")]
        [SerializeField] private float _distanceToDisplayInfo = 2.5f;

        private PlayerReferences _playerRef;
        private Transform _playerTransform;
        private PlayerMoney _playerMoney;
        private NewWeaponManager _playerWeapons;
        private Camera _cam;


        [SerializeField] private RectTransform _itemDisplayCanvas;
        [SerializeField] private Transform _itemModelHolder;
        [SerializeField] private bool _isInStock = false;
        public bool IsInStock => _isInStock;
        public WeaponData HeldWeaponData => _heldWeaponData;

        [SerializeField] private TextMeshProUGUI _nameText = null;
        [SerializeField] private TextMeshProUGUI _descriptionText = null;
        [SerializeField] private Image _displayImage = null;
        [SerializeField] private TextMeshProUGUI _itemPriceText = null;
        [SerializeField] private Image _priceBG = null;



        // dodat display za ima / nema para i out of stock




        // Interactions
        public override void Interact(PlayerReferences playerRef)
        {
            if (!_isInStock)
                return;

            if (_playerRef == null)
                SetPlayerRef(playerRef);

            if (!_playerMoney.CanAfford(_itemPrice))
                return;

            BuyHeldItem();
        }
        public void BuyHeldItem()
        {
            _playerMoney.LoseMoney(_itemPrice);
            _playerWeapons.AddWeaponToHeld(_heldWeaponData.WeaponInstance);
            _itemModelHolder.gameObject.SetActive(false);
            _itemDisplayCanvas.gameObject.SetActive(false);
            _isInStock = false;
        }



        // SpawnMeAt
        private void OnEnable()
        {
            PlayerReferences.NewPlayerReference += SetPlayerRef;
        }
        private void OnDisable()
        {
            PlayerReferences.NewPlayerReference -= SetPlayerRef;
        }
        private void SetPlayerRef(PlayerReferences playerRef)
        {
            _playerRef = GameManager.Instance.PlayerRef;
            _playerTransform = _playerRef.PlayerTransform;
            _playerMoney = _playerRef.PlayerMoney;
            _playerWeapons = _playerRef.WeaponManager;
        }
        private void Awake()
        {
            if (_heldWeaponData == null)
            {
                Debug.LogError("No heldWeaponData");
                return;
            }

            _cam = Camera.main;
            _itemPrice = _heldWeaponData.Price;
            Instantiate(_heldWeaponData.WeaponInstance, _itemModelHolder.position, _itemModelHolder.rotation, _itemModelHolder);
            _isInStock = true;
            SetDisplayedItemUI();
        }
        private void SetDisplayedItemUI()
        {
            IWeapon.GunID gunId = _heldWeaponData.WeaponInstance.MyGunID;

            string name = string.Empty;
            string description = string.Empty;

            switch (gunId)
            {
                default:
                    Debug.Log("IWeapon.GunID not recognized");
                    break;

                case IWeapon.GunID.Glockerson:
                    name = DisplayNames.GlockersonName;
                    description = DisplayNames.GlockersonDescription;
                    break;

                case IWeapon.GunID.DoubleShot:
                    name = DisplayNames.DoubleShotName;
                    description = DisplayNames.DoubleShotDescription;
                    break;

                case IWeapon.GunID.TrustyRevolver:
                    name = DisplayNames.TrustyRevolverName;
                    description = DisplayNames.TrustyRevolverDescription;
                    break;

                case IWeapon.GunID.CloseEncounter:
                    name = DisplayNames.CloseEncounterName;
                    description = DisplayNames.CloseEncounterDescription;
                    break;

                case IWeapon.GunID.Deagle:
                    name = DisplayNames.DeagleName;
                    description = DisplayNames.DeagleDescription;
                    break;

                case IWeapon.GunID.Fractal:
                    name = DisplayNames.FractalName;
                    description = DisplayNames.FractalDescription;
                    break;

                case IWeapon.GunID.MoneyGun:
                    name = DisplayNames.MoneyGunName;
                    description = DisplayNames.MoneyGunDescription;
                    break;
            }

            _nameText.SetText(name);
            _itemPriceText.SetText("$" + _heldWeaponData.Price);
            _displayImage.sprite = _heldWeaponData.Sprite;
            _descriptionText.SetText(description);
        }



        // Behavior
        private void Update()
        {

            if (!IsInStock)
                return;

            if (_playerRef == null)
            {
                if (GameManager.Instance.PlayerRef == null)
                {
                    return;
                }
                SetPlayerRef(GameManager.Instance.PlayerRef);
            }

            CanvasBehavior();
        }
        private void CanvasBehavior()
        {
            if (Vector3.Distance(transform.position, _playerTransform.position) > _distanceToDisplayInfo)
            {
                _itemDisplayCanvas.gameObject.SetActive(false);
                return;
            }

            if (_playerMoney.CanAfford(_itemPrice))
            {
                _priceBG.color = Color.green;
            }
            else
            {
                _priceBG.color = Color.red;
            }

            _itemDisplayCanvas.transform.rotation = Quaternion.LookRotation(transform.position - _cam.transform.position, _cam.transform.up);
            _itemDisplayCanvas.gameObject.SetActive(true);
        }

    }
}
