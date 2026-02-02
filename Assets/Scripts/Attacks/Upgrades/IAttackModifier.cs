using System;
using Vector3 = UnityEngine.Vector3;

public interface IAttackModifier
{
    void ModifyExecute(Attack attack, ref Action<Vector3, Vector3> original);

    // int ModifyDamage(Attack attack, int originalValue);
    // float ModifyCooldown(Attack attack, float originalValue);
    // float ModifyTravelSpeed(Attack attack, float originalValue);
    // float ModifyLifetime(Attack attack, float originalValue);
    // bool ModifyPiercing(Attack attack, bool originalValue);
}

/*
- Attacks should now store a list of modifiers
- foreach modifier
- a
*/