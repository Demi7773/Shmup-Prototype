using UnityEngine;

public class SlowDownTime : EnterMode
{

    [SerializeField] protected float _slowTimeTo = 0.5f;
    public float SlowTimeTo => _slowTimeTo;

}
