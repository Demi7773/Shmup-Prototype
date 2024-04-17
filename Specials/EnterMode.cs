using System;
using UnityEngine;

public class EnterMode : SpecialAttack
{

    [Space(30)]
    [Header("EnterMode")]
    [Header("Setup")]
    [SerializeField] protected float _useDuration = 10.0f;
    public float UseDuration => _useDuration;

    [Header("Debug")]
    [SerializeField] protected bool _isModeActive = false;
    public bool IsModeActive => _isModeActive;


    public static event Action<EnterMode> OnEnterMode;
    public static event Action<EnterMode> OnExitMode;


    public Mode ThisMode;
    public enum Mode
    {
        SlowTime,
        Akimbo,
        Invulnerability,
        DoubleUp,
        BoostStats
    }



    public override float CooldownProgress()
    {
        return (_timer - _endOfUse) / (_nextCanUseTarget - _endOfUse);
    }




    protected override void Update()
    {
        base.Update();

        if (_isModeActive)
        {
            if (_timer > _endOfUse)
            {
                ExitThisMode();
            }
        }
    }



    public override bool CanUseSpecial()
    {
        if (_timer > _nextCanUseTarget)
            return true;
        return false;
    }
    public override void UseSpecial()
    {
        _lastUsed = _timer;
        _endOfUse = _timer + _useDuration;
        _nextCanUseTarget = _endOfUse + _useCooldown;
        _isModeActive = true;
        Debug.Log("Mode: " + name + " entered, next CanUse: " + _nextCanUseTarget);
        EnterThisMode();
    }



    public virtual void EnterThisMode()
    {
        OnEnterMode?.Invoke(this);
    }
    public virtual void ExitThisMode()
    {
        _isModeActive = false;
        Debug.Log("Mode: " + name + " Exit");
        OnExitMode?.Invoke(this);
    }

}
