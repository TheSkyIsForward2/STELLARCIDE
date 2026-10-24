using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Stores an entity's team type and HP. Come to definition to find
/// all teams available and maybe add more. Also stores the entity
/// owner itself
/// </summary>
public class HealthOwner : Component
{
    public enum Team
    {
        PLAYER,
        ENEMY,
        DESTRUCTIBLE
    }
    public Team team;
    public int hp;
    public int maxHP;
    public GameObject owner;

    /// <param name="hp"> Integer value describing hitpoints </param>
    /// <param name="team"> Can be from {Team.PLAYER, Team.ENEMY, Team.DESTRUCTIBLE} </param>
    /// <param name="owner"> GameObject uses this component</param>
    public HealthOwner(int hp, Team team, GameObject owner)
    {
        this.hp = hp;
        maxHP = hp;
        this.team = team;
        this.owner = owner;
    }

    /// <summary>
    /// Subtract damage.amount from the owner's hp. Can also heal through negative numbers.
    /// Entity will die when health reaches 0 and will not overheal.
    /// </summary>
    /// <param name="damage"> struct Damage(int amount, Type type) </param>
    /// <param name="attackDirection"> Vector2 of incoming attack's direction for particles,
    /// leave as Vector2.zero if non-standard attack</param>
    /// <returns> True if the healthowner dies as a result of this damage </returns>
    public bool TakeDamage(Damage damage, Vector2 attackDirection)
    {
        // HANDLE dmg & heal on negatives & account for overhealth
        if (hp - damage.Amount > maxHP)
        {
            hp = maxHP;
        }
        else
        {
            AudioManager.Instance.PlayPlayerTakeDamageSFX();

            #region  spawn particle

            // check for incoming direction
            if (!attackDirection.Equals(Vector2.zero))
            {
                // Red stars damage particle
                if (owner.TryGetComponent<RedDwarf>(out RedDwarf rD))
                {
                    if (rD != null)
                    {
                        GameManager.Instance.ParticleManager.SpawnParticle(owner.transform, 0, attackDirection);
                    }
                }
                else if (owner.TryGetComponent<RedGiant>(out RedGiant rG))
                {
                    if (rG != null)
                    {
                        GameManager.Instance.ParticleManager.SpawnParticle(owner.transform, 0, attackDirection);
                    }
                }
                // Yellow stars damage particle
                else if (owner.TryGetComponent<YellowDwarf>(out YellowDwarf yD))
                {
                    if (yD != null)
                    {
                        GameManager.Instance.ParticleManager.SpawnParticle(owner.transform, 1, attackDirection);
                    }
                }
            
            
                // TODO create player damage particle(s)
            }
            
            #endregion
            
            // Debug.Log($"[DAMAGE] something on team {team} took {damage.Amount} of {damage.type} damage");
            hp -= damage.Amount;
        }

        if (team == Team.PLAYER)
        {
            EventBus.Instance.HitPlayer();
        }
        if (hp > 0) return false;

        // HANDLE death 
        hp = 0;

        if (team == Team.PLAYER)
        {
            SceneManager.LoadScene("Leaderboard");
            return true;
        }

        if (team == Team.ENEMY)
        {
            EventBus.Instance.EnemyDead(owner);
        }
        
        Destroy(owner);
        return true;
    }

    // TODO: for mission walls (maybe)
    /// <summary>
    /// Make this HealthOwner / entity take damage over time
    /// </summary>
    /// <param name="duration"> Total time in seconds that this DOT lasts. If value<0, will go on indefinitely></param>
    /// <param name="totalDamage"> struct Damage(int amount, Type type) </param>
    /// /// <param name="interval"> Time in seconds between each tick of damage. Default to 0.5 if value<0</param>
    /// <returns></returns>
    public bool TakeDOT(float duration, Damage totalDamage, float interval = 0.5f)
    {
        if (interval <= 0) { interval = 0.5f; }

        if (duration == 0)
        {
            return TakeDamage(totalDamage, Vector2.zero);
        }
        else if (duration < 0)
        {
            CoroutineManager.Instance.Run(SlowPainfulDeath(totalDamage,interval));
        }
        else
        {
            if (interval > duration) { return TakeDamage(totalDamage, Vector2.zero); }

            float dmgPerTick = totalDamage.Amount / (duration / interval);
            CoroutineManager.Instance.Run(DoTickDamage(
                duration, 
                new Damage(Mathf.RoundToInt(dmgPerTick), totalDamage.type), 
                interval
            ));
        }
        
        return false;
    }

    /// <summary>
    /// When increasing an entity's max hp by [int amount], their current hp amount relative to their new max hp
    /// stays the same.
    /// </summary>
    /// <param name="amount"> A flat integer amount to increase the entity's maximum hit points by </param>
    public void IncreaseMaxHP(int amount)
    {
        float ratio = hp / maxHP;
        maxHP += amount;
        hp = Mathf.RoundToInt(ratio * maxHP);
    }

    #region Helper Funks
    IEnumerator DoTickDamage(float duration, Damage dmg, float interval)
    {
        float _duration = duration;
        while ( _duration >= 0)
        {
            if (TakeDamage(dmg, Vector2.zero)) yield break;
            _duration -= interval;
            yield return new WaitForSeconds(interval);
        } 
        yield return new WaitForEndOfFrame();
    }

    IEnumerator SlowPainfulDeath(Damage dmg, float interval)
    {
        while (true)
        {
            if (TakeDamage(dmg, Vector2.zero)) yield break;
            yield return new WaitForSeconds(interval);
        }
    }
    #endregion
}