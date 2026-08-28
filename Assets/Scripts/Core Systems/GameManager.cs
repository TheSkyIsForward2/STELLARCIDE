using UIScripts.Leaderboard;
using UnityEngine;

public class GameManager
{
    static GameManager thisInstance;

    public GameObject Player;
    public ProjectileManager ProjectileManager;
    public SpawnerManager SpawnerManager;
    public bool GameActive;
    public XMLManager xmlManager;
    public ScoreManager scoreManager;

    public static GameManager Instance
    {
        get { return thisInstance ??= new GameManager(); }
    }

}