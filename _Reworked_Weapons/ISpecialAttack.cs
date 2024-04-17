

public interface ISpecialAttack
{

    public enum SpecialBehavior
    {
        SingleUse,
        Mode
    }

    public bool CanUseSpecial();
    // in parameter SpecialAttack because subscribes to event from NewWeaponManager -> PlayerEvents
    public void UseSpecial();

}
