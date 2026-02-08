using System.Collections;
using UnityEngine;

public class Slash : Attack
{
    /// <summary>
    /// An armed melee attack. Performs several instances of damage at close range.
    /// Uses colliders to deal damage.
    /// </summary>
    /// <param name="owner">Gameobject that will perform the punch</param>
    /// <param name="damage">Damage value and type</param>
    /// <param name="cooldown">Time in seconds before another attack</param>
    public Slash(GameObject owner,
                  Damage damage,
                  float cooldown) : base(owner, damage, cooldown)
    {
        AttackType = Type.ARMED_MELEE;
        if (Owner.transform.Find("MechVisual").TryGetComponent<Animator>(out Animator a))
        {
            Animator = a;  
        }
        AnimationName = "Slash";
    }

    /// <summary>Actually punches (verb)</summary>
    /// <param name="origin">Nothing... just necessary for override</param>
    /// <param name="target">Slash deals damage using a collider</param>
    /// <returns></returns>
    public override IEnumerator Execute(Vector3 origin, Vector3 target)
    {
        if (Animator) Animator.SetTrigger("executeSlash");
            
        LastExecute = Time.time;
        yield return new WaitWhile(AnimatorIsPlaying);

        LastExecute = Time.time;
        yield return new WaitForEndOfFrame();
    }
}