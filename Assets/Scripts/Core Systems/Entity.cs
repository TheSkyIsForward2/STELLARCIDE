using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Class that PlayerHealth and EnemyHealth inherit from. Stores a HealthOwner which holds
/// the Entity's team, and max hp
/// </summary>
public abstract class Entity : MonoBehaviour
{
    [NonSerialized] public HealthOwner healthController;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="origin">Position of the source of knockback</param>
    /// <param name="strength"></param>
    /// <returns></returns>
    public IEnumerator KnockBack(Vector3 origin, float strength)
    {
        if (healthController == null)
        {
            yield break;
        }
        if (healthController.team == HealthOwner.Team.ENEMY)
        {
            // TODO: STANLEY change state to IDLE temporarily so they dont move while being knocked back
        }

        float elapsedTime=0;
        Vector3 directionVector = transform.position - origin;

        while (elapsedTime < strength)
        {
            transform.position = Vector3.Lerp(transform.position, 
                origin + directionVector, 
                elapsedTime/strength
            );
            elapsedTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        transform.position = origin + directionVector;
        yield return new WaitForEndOfFrame();
    }
}