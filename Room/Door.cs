using UnityEngine;

public class Door : MonoBehaviour
{

    [SerializeField] private bool _isOpened = false;
    //[SerializeField] private Collider _collider;
    [SerializeField] private GameObject _door;



    private void Awake()
    {
        //_collider = GetComponent<Collider>();
    }

    public void SetOpened(bool isOpened)
    {
        _isOpened = isOpened;
        _door.SetActive(isOpened);
        //_collider.enabled = _isOpened;

        if (isOpened)
        {
            Debug.Log("Door opened");
            return;
        }
        Debug.Log("Door closed");

    }

}
