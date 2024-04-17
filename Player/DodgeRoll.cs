using System;
using UnityEngine;

public class DodgeRoll : MonoBehaviour
{

    [Header("Resource system")]
    [SerializeField] private float _dodgeMeter = 20.0f;
    [SerializeField] private float _dodgeMeterMax = 20.0f;
    [SerializeField] private float _dodgeCost = 10.0f;
    [SerializeField] private float _regenPerSec = 5.0f;

    [Header("Movement")]
    [SerializeField] private AnimationCurve _moveForwardCurve;
    [SerializeField] private float _dodgeDuration = 0.5f;
    [SerializeField] private float _dashDistance = 3.0f;
    [SerializeField] private LayerMask _undashableLayers;

    [Header("Debug")]
    [SerializeField] private Vector3 _dodgeStartPos;
    [SerializeField] private Vector3 _dodgeEndPos;
    [SerializeField] private Vector3 _currentDashDirection;
    public bool IsDodging = false;

    [Header("Timer")]
    [SerializeField] private float _timer = 0.0f;
    [SerializeField] private float _minTimeBetweenDodges = 0.3f;
    [SerializeField] private float _endOfLastDodge = -10.0f;
    [SerializeField] private float _dodgeSequenceStart = 0.0f;


    private float _bar1Fill = 1.0f;
    private float _bar2Fill = 1.0f;
    public float Bar1Fill => _bar1Fill;
    public float Bar2Fill => _bar2Fill;

    public static event Action<DodgeRoll> PlayerDodgeBarProgress;




    private void OnEnable()
    {
        UIEvents.OnUpdateAllHUD += UpdateUI;
    }
    private void OnDisable()
    {
        UIEvents.OnUpdateAllHUD -= UpdateUI;
    }





    public bool CanDodge()
    {

        if (IsDodging)
        {
            //Debug.Log("Cant dash: Already Dashing");
            return false;
        }

        if( _dodgeMeter < _dodgeCost)
        {
            //Debug.Log("Cant dash: No charges");
            return false;
        }

        if (_timer < _endOfLastDodge + _minTimeBetweenDodges)
        {
            //Debug.Log("Cant dash: Too soon");
            return false;
        }

        //Debug.Log("Can dash");
        return true;
    }

    public void StartDodging(Vector3 direction)
    {
        _currentDashDirection = direction;
        _dodgeStartPos = transform.position;


        _dodgeEndPos = CalculateEndPositionOutsideTerrain(_dodgeStartPos, _currentDashDirection, _dashDistance);



        _dodgeMeter -= _dodgeCost;
        _dodgeSequenceStart = _timer;
        _endOfLastDodge = _timer + _dodgeDuration;
        IsDodging = true;

        UpdateUI();
        PlayerEvents.PlayerDashes(_dodgeStartPos, _dodgeEndPos);
        //Debug.Log("Start of Dodge Sequence at " + _dodgeSequenceStart + " - StartPos: " + _dodgeStartPos + ", EndPos: " + _dodgeEndPos + ", dodgeMeter: " + _dodgeMeter);
    }
    private Vector3 CalculateEndPositionOutsideTerrain(Vector3 startPos, Vector3 trajectoryDirection, float distance)
    {

        Vector3 endPos = startPos + (trajectoryDirection * distance);
        //Debug.Log("Calculating dash, startPos: " + startPos + ", direction: " + trajectoryDirection + ", distance: " +  distance);
        //Debug.DrawLine(startPos, endPos, Color.red, 2.0f);

        Ray ray = new Ray(startPos, trajectoryDirection);     
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, distance, _undashableLayers))
        {
            Vector3 dir2 = trajectoryDirection + (hitInfo.normal * 0.5f);
            Vector3 endPos2 = startPos + (dir2 * distance);
            endPos = endPos2;

            //Debug.Log("RaycastHit1 at: " + hitInfo.point + ", adjusting trajectory to: " + dir2 + ", endPos: " + endPos);
            //Debug.DrawLine(startPos, endPos2, Color.red, 2.0f);

            Ray ray2 = new Ray(startPos, dir2);
            RaycastHit hitInfo2;
            if (Physics.Raycast(ray2, out hitInfo2, distance, _undashableLayers))
            {
                Vector3 dir3 = dir2 + (hitInfo2.normal * 0.5f);
                Vector3 endPos3 = startPos + (dir3 * distance);
                endPos = endPos3;

                //Debug.Log("RaycastHit2 at: " + hitInfo2.point + ", adjusting trajectory to: " + dir3 + ", endPos: " + endPos);
                //Debug.DrawLine(startPos, endPos3, Color.red, 2.0f);

                Ray ray3 = new Ray(startPos, dir3);
                RaycastHit hitInfo3;
                if (Physics.Raycast(ray3, out hitInfo3, distance, _undashableLayers))
                {
                    endPos = hitInfo.point;
                    Debug.Log("RaycastHit3 at: " + hitInfo3.point + ". Defaulting to 1st Raycast hit point: " + hitInfo.point);
                }
            }


            // Old attempt at System with Dash-through, doesnt work but parts might be reusable

            //Vector3 defaultEndPos = endPos;
            //Debug.Log("1st dash Raycast hit obstacle, checking endPos");

                //RaycastHit endHitInfo = new RaycastHit();
                //// if endPos is too close to collider, that hitInfo is used for new endPos
                //if (Physics.SphereCast(defaultEndPos, 0.0f, trajectoryDirection, out endHitInfo, _undashableLayers))
                //{
                //    Vector3 closestPos = endHitInfo.collider.ClosestPointOnBounds(defaultEndPos);
                //    //Vector3 closestPos = endHitInfo.collider.ClosestPoint(endPos);
                //    Debug.Log("2nd Dash Raycast hit obstacle, closest point on bounds: " + closestPos);

                //    RaycastHit adjustedEndHitInfo = new RaycastHit();
                //    if (Physics.SphereCast(closestPos, 0.0f, trajectoryDirection, out adjustedEndHitInfo, _undashableLayers))
                //    {
                //        closestPos = hitInfo.point;
                //        Debug.Log("3rd Dash Raycast hit obstacle: " + adjustedEndHitInfo.collider + " at: " + adjustedEndHitInfo.point + ", defaulting to 1st Raycast hit point: " + closestPos);
                //    }

                //    if (Vector3.Distance(closestPos, defaultEndPos) > 2.0f)
                //    {
                //        //Debug.Log("ClosestPos was too far from trajectory, set from " + closestPos + " to hit point: " + hitInfo.point);
                //        closestPos = hitInfo.point;
                //        Debug.Log("Adjusted end point too far from original, defaulting to 1st Raycast hit point: " + closestPos);
                //    }

                //    Vector3 nudgeDirection = (closestPos - defaultEndPos).normalized;
                //    endPos = closestPos + nudgeDirection;
                //    Debug.Log("Nudging final position by: " + nudgeDirection + ", final adjusted position: " + endPos);
                //    //Debug.Log("Raycast hit Undashable Terrain on Dash. StartPos: " + startPos + ", direction: " + trajectoryDirection + ", distance: " + distance +
                //    //", defaultEndPos: " + defaultEndPos + " -> adjusted endPos: " + endPos);
                //}
        }

        //Debug.Log("Final Dash Trajectory set: StartPos: " + startPos + ", EndPos: " + endPos);
        return endPos;
    }



    private void Awake()
    {
        //_thisLevelTimer = 0.0f;
        _dodgeMeter = _dodgeMeterMax;
        _endOfLastDodge = -10.0f;
        UpdateUI();
    }
    private void Start()
    {
        UpdateUI();
    }

    private void Update()
    {

        _timer = GameManager.Instance.IngameUnscaledTimer;

        if (IsDodging)
        {
            DodgeSequence();
            return;
        }

        if (_dodgeMeter >= _dodgeMeterMax)
        {
            return;
        }

        _dodgeMeter = Mathf.MoveTowards(_dodgeMeter, _dodgeMeterMax, _regenPerSec * Time.unscaledDeltaTime);
        UpdateUI();

    }

    private void DodgeSequence()
    {
        float timeSinceStart = _timer - _dodgeSequenceStart;
        float completion = timeSinceStart / _dodgeDuration;

        float posModifier = _moveForwardCurve.Evaluate(completion);

        transform.position = Vector3.Lerp(_dodgeStartPos, _dodgeEndPos, posModifier);
        //Debug.Log("DodgeSequence completion: " +  completion + ", scaled by curve: " + posModifier + ", pos: " + transform.position);

        if (_timer > _endOfLastDodge)
        {
            IsDodging = false;
            
            //Debug.Log("End of Dodge Sequence at " + _thisLevelTimer + " with position: " + transform.position);
        }
    }

    private void UpdateUI()
    {
        _bar1Fill = Mathf.Clamp01(_dodgeMeter / 10.0f);
        _bar2Fill = Mathf.Clamp01((_dodgeMeter - 10.0f) / (_dodgeMeterMax - 10.0f));
        PlayerDodgeBarProgress?.Invoke(this);
    }
}
