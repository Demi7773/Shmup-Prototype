using UnityEngine;

public class ProjectileBurst : SpecialAttack
{

    [SerializeField] protected GuidedProjectilesController _projectileController;


    public override float CooldownProgress()
    {
        return (_timer - _endOfUse) / (_nextCanUseTarget - _endOfUse);
    }



    public void Init()
    {
        _projectileController.Init();
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
        _endOfUse = _timer;
        _nextCanUseTarget = _timer + _useCooldown;
        _projectileController.Fire();
    }

}
