using System.Collections;
using UnityEngine;

public class Slash : Attack
{
    /// <summary>
    /// Instantiate a quick melee Attack. This one just damages all enemies in front of the player 
    /// in a close proximity. Call Slash.Execute() to actually perform the attack. To access its 
    /// cooldown call Slash.IsReady()
    /// </summary>
    /// <param name="owner">Gameobject that will perform the punch</param>
    /// <param name="damage">Damage value and type</param>
    /// <param name="cooldown">Time in seconds before another attack</param>
    public Slash(GameObject owner,
                  Damage damage,
                  float cooldown) : base(owner, damage, cooldown)
    {
        AttackType = Type.UNARMED_MELEE;
    }

    public override IEnumerator Execute(Vector3 origin, Vector3 target)
    {
        // instantiate two line / rect at +-120d
        // -120 goes first and up to 30d
        // then 120 goes to -30
        // every entity it touches is dealt damage

        LastExecute = Time.time;
        yield return new WaitForEndOfFrame();
    }
}