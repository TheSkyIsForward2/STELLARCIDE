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

    public override IEnumerator Execute(Vector3 origin, Vector3 target)
    {
        if (Animator) {Animator.SetTrigger("executeSlash");}
        
        LastExecute = Time.time;
        yield return new WaitForSeconds(0.43f); // ikik magic numbers but whatever
        DamageArea(3,3);

        LastExecute = Time.time;
        yield return new WaitForSeconds(0.227f);
        DamageArea(3,3);

        LastExecute = Time.time;
        yield return new WaitWhile(AnimatorIsPlaying);
    }

}