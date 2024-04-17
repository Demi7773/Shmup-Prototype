using ReworkedWeapons;
using System;
using UnityEngine;

public static class PlayerEvents
{

    public static event Action<float/*, float*/> OnPlayerSlowsDownTime;
    public static event Action OnSlowDownTimeStops;
    public static event Action<Vector3, Vector3> OnPlayerDashes;
    public static event Action<SpecialAttack> OnPlayerUsesSpecial;
    //public static event Action<float> OnPlayerBecomesImmune;
    //public static event Action<float> OnPlayerBecomesSlowed;
    //public static event Action<Weapon> OnPlayerShootsWeapon;
    //public static event Action<Weapon> OnPlayerCompletesReload;



    //public static void PlayerShootsWeapon(Weapon weapon)
    //{
    //    OnPlayerShootsWeapon?.Invoke(weapon);
    //}
    //public static void PlayerCompletesReload(Weapon weapon)
    //{
    //    OnPlayerCompletesReload?.Invoke(weapon);
    //}
    public static void PlayerUsesSpecial(SpecialAttack special)
    {
        OnPlayerUsesSpecial?.Invoke(special);
    }

    public static void PlayerDashes(Vector3 startPos, Vector3 endPos)
    {
        OnPlayerDashes?.Invoke(startPos, endPos);
    }

    public static void SlowDownTimeStarts(float timeScale/*, float duration*/)
    {
        OnPlayerSlowsDownTime?.Invoke(timeScale/*, duration*/);
    }
    public static void SlowDownTimeStops()
    {
        OnSlowDownTimeStops?.Invoke();
    }

}
