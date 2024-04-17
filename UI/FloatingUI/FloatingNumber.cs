using TMPro;
using UnityEngine;

public class FloatingNumber : MonoBehaviour
{

    [Header("Dependencies")]
    [SerializeField] private TextMeshProUGUI _displayTmp;
    [Space(20)]
    [Header("Settings")]
    [SerializeField] private AnimationCurve _xCurve;
    [SerializeField] private AnimationCurve _zCurve;
    [SerializeField] private AnimationCurve _fadeAlphaCurve;
    [SerializeField] private float _duration = 0.5f;
    [SerializeField] private float _xRange = 1.5f;

    public enum Context
    {
        Damage,
        ShieldDamage,
        Heal
    }
    [Space(20)]
    [Header("Debug")]
    [SerializeField] private float _startingX, _startingY, _startingZ;
    [SerializeField] private float _endingX, _endingZ;
    [SerializeField] private Context _context;
    [SerializeField] private Vector3 _startPos;
    [SerializeField] private Vector3 _endPos;
    [SerializeField] private float _startTime = 0.0f;
    [SerializeField] private float _endTime = 0.0f;




    public void Init(Context context, Vector3 startPos, float amount)
    {
        _context = context;
        _displayTmp.color = _context switch
        {
            Context.Damage => Color.red,
            Context.ShieldDamage => Color.blue,
            Context.Heal => Color.green,
            _ => Color.red,
        };

        _displayTmp.text = ((int) amount).ToString();
        //_displayTmp.text = amount.ToString("{0:0}");

        _startTime = GameManager.Instance.IngameUnscaledTimer;
        _endTime = _startTime + _duration;


        float xOffset = Random.Range(-_xRange, _xRange);
        float zOffset = Random.Range(1.5f, 3.0f);
        transform.position = startPos;

        _startingX = startPos.x;
        _startingY = startPos.y;
        _startingZ = startPos.z;

        _endingX = _startingX + xOffset;
        _endingZ = _startingZ + zOffset;

        //_startPos = startPos;
        //_endPos = _startPos + new Vector3(xOffset, 0f, 2f);

        gameObject.SetActive(true);
        //Debug.Log("FloatingNumber Init with trajectory: " + _startPos + " to " + _endPos);
    }



    private void Update()
    {
        float timer = GameManager.Instance.IngameUnscaledTimer;
        if (timer > _endTime)
        {
            _displayTmp.text = string.Empty;
            gameObject.SetActive(false);
            return;
        }

        float progress = (timer - _startTime) / _duration;
        float alphaCurveProgress = _fadeAlphaCurve.Evaluate(progress);

        float xCurveProgress = _xCurve.Evaluate(progress);
        float zCurveProgress = _zCurve.Evaluate(progress);

        float currentX = Mathf.Lerp(_startingX, _endingX, xCurveProgress);
        float currentZ = Mathf.Lerp(_startingZ, _endingZ, zCurveProgress);

        Vector3 newPos = new Vector3(currentX, _startingY, currentZ);

        transform.position = newPos;
        _displayTmp.alpha = alphaCurveProgress;
    }

}
