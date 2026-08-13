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
    public StateController sc;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="origin">Position of the source of knockback</param>
    /// <param name="strength"></param>
    /// <returns></returns>
    public IEnumerator KnockBack(Vector3 origin, float strength)
    {
        Debug.Log("do some knockback");
        if (sc)
        {
            Debug.Log("do something");
            sc.locked = true;
        }

        float elapsedTime=0;
        Vector3 directionVector = transform.position - origin;

        while (elapsedTime < strength)
        {
            transform.position = Vector3.Lerp(transform.position, 
                origin + directionVector, 
                elapsedTime/strength
            );
            Debug.Log("knocking back");
            elapsedTime += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
        transform.position = origin + directionVector;

        if (sc)
        {
            sc.locked = false;
        }

        yield return new WaitForEndOfFrame();
    }
}