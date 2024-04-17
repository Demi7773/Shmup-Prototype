using TMPro;
using UnityEngine;

public class HealToFull : Interactible
{

    //[SerializeField] private bool _isReusable = false;
    //[SerializeField] private bool _hasBeenUsed = false;

    private PlayerReferences _playerRef;
    private Transform _playerTransform;
    private Camera _cam;
    [SerializeField] private float _distanceToDisplayInfo = 2.5f;
    [SerializeField] private GameObject _uiPopUp;
    [SerializeField] private TextMeshProUGUI _text = null;





    public override void Interact(PlayerReferences playerRef)
    {
        if (_playerRef == null)
        {
            SetPlayerRef(playerRef);
        }
        PlayerHP playerHP = _playerRef.PlayerHP;
        if (playerHP.IsFullHP())
        {
            //Debug.Log("Already full HP");
            return;
        }

        playerHP.Heal(playerHP.MaxHP);
        //Debug.Log("Player used HealToFull");

        //if (_isReusable)
        //    return;
        //_hasBeenUsed = true;
        //Debug.Log("Single use heal consumed");
    }


    private void Awake()
    {
        _cam = Camera.main;
        _text.SetText("Press E to Drink from the Rejuvenating fountain and heal up");
    }
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
        _playerRef = playerRef;
        _playerTransform = _playerRef.PlayerTransform;
    }




    private void Update()
    {

        if (_playerTransform == null)
        {
            return;
        }

        if (Vector3.Distance(transform.position, _playerTransform.position) > _distanceToDisplayInfo)
        {
            _uiPopUp.SetActive(false);
            return;
        }
        else
        {
            _uiPopUp.SetActive(true);
        }

        _uiPopUp.transform.rotation = Quaternion.LookRotation(transform.position - _cam.transform.position, _cam.transform.up);
    }

}
