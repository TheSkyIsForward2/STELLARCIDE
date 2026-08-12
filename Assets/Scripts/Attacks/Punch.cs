using System.Collections;
using UnityEngine;

public class Punch : Attack
{
    /// <summary>
    /// Instantiate a quick melee Attack. This one just damages all enemies in front of the player 
    /// in a close proximity. Call Punch.Execute() to actually perform the attack. To access its 
    /// cooldown call Punch.IsReady()
    /// </summary>
    /// <param name="owner">Gameobject that will perform the punch</param>
    /// <param name="damage">Damage value and type</param>
    /// <param name="cooldown">Time in seconds before another attack</param>
    public Punch(GameObject owner,
                  Damage damage,
                  float cooldown,
                  float travelSpeed=0) : base(owner, damage, cooldown)
    {
        TravelSpeed = travelSpeed;
        AttackType = Type.UNARMED_MELEE;
        if (Owner.transform.Find("MechVisual"))
        {
            if (Owner.transform.Find("MechVisual").TryGetComponent<Animator>(out Animator a))
            {
                Animator = a;  
            }
            AnimationName = "Punch";
        }
        playerRB = Owner.GetComponent<Rigidbody2D>();
    }

    /// <summary>Actually punches (verb)</summary>
    /// <param name="origin">Nothing... just necessary for override</param>
    /// <param name="target">target.x is the total width of the punch, target.y 
    ///     is the total range </param>
    /// <returns></returns>
    public override IEnumerator Execute(Vector3 origin, Vector3 target)
    {
        if (Animator) {
            Animator.SetTrigger("executePunch");
        }

        // small lunge forward
        playerRB.AddForce(Owner.transform.right * TravelSpeed, ForceMode2D.Impulse);
        
        LastExecute = Time.time;
        yield return new WaitForSeconds(0.30f);

        AudioManager.Instance.PlayPunchingSFX();

        // knocking back enemies
        foreach (Entity entity in DamageArea(range: (float)target.x, width: (float)target.y))
        {
            if (entity.healthController.team == HealthOwner.Team.ENEMY)
            {
                CoroutineManager.Instance.Run(entity.KnockBack(
                    origin: entity.transform.position,
                    strength: 3));
            }
        }

        LastExecute = Time.time;
        yield return new WaitWhile(AnimatorIsPlaying);

        // lol it works
        if (Doubling)
        {
            if (Animator) {
                Animator.SetTrigger("executePunch");
            }

            LastExecute = Time.time;
            yield return new WaitForSeconds(0.20f);

            AudioManager.Instance.PlayPunchingSFX();
            DamageArea(range: (float)target.x, width: (float)target.y);

            LastExecute = Time.time;
            yield return new WaitWhile(AnimatorIsPlaying);
        }
    }
}