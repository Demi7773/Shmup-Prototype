using ReworkedWeapons;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrowingWeaponsDisplay : MonoBehaviour
{

    private NewWeaponManager _weapons;

    [SerializeField] private GameObject _displayPanel;
    [SerializeField] private Image _displayImagePrefab;
    [SerializeField] private List<Image> _displayImages = new List<Image>();

    [SerializeField] private float _heightPerImage = 60.0f;
    [SerializeField] private float _rectHeightOffset = 10.0f;
    [SerializeField] private float _finalRectHeight = 0.0f;

    [SerializeField] private float _rectWidth = 70.0f;

    [SerializeField] private RectTransform _selectionHighlight;
    [SerializeField] private float _finalSelectionHighlightYPos = 0.0f;
    [SerializeField] private float _defaultSelectionY = 0.0f;
    [SerializeField] private float _selectionHighlightXPos = 0.0f;




    private void OnEnable()
    {
        NewWeaponManager.OnHeldGunsChange += OnHeldGunsChanged;
        NewWeaponManager.OnSwapWeapon += OnSelectedWeaponChanged;
    }
    private void OnDisable()
    {
        NewWeaponManager.OnHeldGunsChange -= OnHeldGunsChanged;
        NewWeaponManager.OnSwapWeapon -= OnSelectedWeaponChanged;

    }


    private void OnHeldGunsChanged(NewWeaponManager weapons)
    {
        _weapons = weapons;

        int displayImgs = _displayImages.Count;
        for (int i = displayImgs; i > 0; i--)
        {
            _displayImages[i - 1].gameObject.SetActive(false);
            //Destroy(_displayImages[i - 1]);
        }
        _displayImages.Clear();

        int heldWeapons = _weapons.HeldWeaponsCount();
        _finalRectHeight = _rectHeightOffset + (heldWeapons * _heightPerImage);
        _displayPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(_rectWidth, _finalRectHeight);


        for (int i = heldWeapons; i > 0; i--)
        {
            Image image = Instantiate(_displayImagePrefab, _displayPanel.transform);
            Weapon weapon = _weapons.GetWeaponBySlot(i - 1);
            Sprite sprite = weapon.Sprite;
            image.sprite = sprite;
            _displayImages.Add(image);
            //Debug.Log("GrowingWeapons set up a WeaponDisplay, i: " + i + ", weapon: " + weapon.name + ", sprite: " + sprite.name);
        }
        OnSelectedWeaponChanged(_weapons.CurrentWeapon);
    }

    private void OnSelectedWeaponChanged(Weapon weapon)
    {
        _finalSelectionHighlightYPos = SelectionHighlightYPosInContext();
        _selectionHighlight.position = new Vector2(_selectionHighlightXPos, _finalSelectionHighlightYPos);
    }

    private float SelectionHighlightYPosInContext()
    {
        if (_weapons == null)
        {
            Debug.Log("Weapon Controller null, returning _defaultSelectionY");
            return _defaultSelectionY;
        }
        int selectedIndex = _weapons.SelectedWeaponIndex;
        float y = 150f + (_rectHeightOffset / 2f) + (selectedIndex + 0.5f) * _heightPerImage;
        return y;
    }

}
