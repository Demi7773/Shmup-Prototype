using System.Collections.Generic;
using UnityEngine;

public class FloatingNumbersCanvas : MonoBehaviour
{

    [Header("Dependencies")]
    [SerializeField] private List<FloatingNumber> _startingList = new List<FloatingNumber>();

    [Space(20)]
    [Header("Debug")]
    [SerializeField] private Camera _cam;
    [SerializeField] private int _lastDeployedIndex = 0;




    private void OnEnable()
    {
        //OnNewPlayerCam += OnNewPlayerCam;
        //EnemyBehavior.OnRequestFloatingNumber += RequestFloatingNumber;
    }
    private void OnDisable()
    {
        //OnNewPlayerCam -= OnNewPlayerCam;
        //EnemyBehavior.OnRequestFloatingNumber += RequestFloatingNumber;
    }

    private void OnNewPlayerCam(Camera cam)
    {
        _cam = cam;
    }



    public void Init()
    {
        _cam = Camera.main;
        foreach (FloatingNumber number in _startingList)
        {
            number.gameObject.SetActive(false);
            //_availableFloatingNumbers.Enqueue(number);
        }

        _lastDeployedIndex = 0;
        gameObject.SetActive(true);
        //Debug.Log("EnemyHPUIAdvanced SpawnMeAt complete");
    }



    public void RequestFloatingNumber(Transform target, Vector3 startingOffset, FloatingNumber.Context context, float value)
    {
        var floatingNumber = _startingList[_lastDeployedIndex];
        _lastDeployedIndex++;

        Vector3 startingPos = target.position + startingOffset;
        Vector3 screenSpacePos = _cam.WorldToScreenPoint(startingPos);
        floatingNumber.Init(context, screenSpacePos, value);
    }


}
