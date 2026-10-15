using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public int enemiesDefeated = 0;
    public int redDwarfDefeated = 0;
    public int yellowDwarfDefeated = 0;
    public int redGiantDefeated = 0;
    public int totalMapsCleared = 0;

    private void Start()
    {
        GameManager.Instance.scoreManager = this;
        EventBus.Instance.OnEnemyDead += EnemyDefeated;
    }

    public void EnemyDefeated(GameObject gb)
    {
        // determine type of enemy killed and add it
        // Red Dwarf
        if (gb.TryGetComponent<RedDwarf>(out RedDwarf rd))
            redDwarfDefeated++;
        // Yellow Dwarf
        if (gb.TryGetComponent<YellowDwarf>(out YellowDwarf yd))
            yellowDwarfDefeated++;
        // Red Giant (Head is health owner entity)
        if (gb.TryGetComponent<Head>(out Head rg))
            redGiantDefeated++;
        // default
        enemiesDefeated++;
        
    }
    
    
    
}
