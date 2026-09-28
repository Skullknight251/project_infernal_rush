using UnityEngine;

public interface IShieldBlockable
{
    bool CanPierceShield { get; }

    void OnBlockedByShield();
}
