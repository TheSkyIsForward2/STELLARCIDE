using System;
using Vector3 = UnityEngine.Vector3;

public abstract class AttackModifier : IAttackModifier
{
    AttackModifier()
    {
        
    }

    public virtual void ModifyExecute(Attack attack, ref Action<Vector3, Vector3> original){}
}