using UnityEngine;

public class RotateMeTowardsMainCam : MonoBehaviour
{

    private Camera _cam;



    private void Start()
    {
        _cam = Camera.main;
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - _cam.transform.position, _cam.transform.up);
    }

}
