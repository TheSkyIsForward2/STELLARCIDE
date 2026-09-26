using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class MissionPanel : MonoBehaviour
{
    public string currentNodeID;
    public TMP_Text description;
    public TMP_Text scoreMult;
    private SelectorMapGenerator mapGenerator;

    private string[] mapPool = {"BasicMission", "HalfNHalf", "Central", "AsteroidHeavy", "AsteroidLight"};

    private void Start()
    {
        mapGenerator = FindAnyObjectByType<SelectorMapGenerator>();
    }

    // TODO use grammars to put descriptions of nodes into the map generator
    public void GenerateMissionDescription()
    {
        
    }

    public void MissionSelected()
    {
        GameManager.Instance.difficultySum += mapGenerator.graph.Nodes[currentNodeID].Difficulty;
        mapGenerator.ChangePlayerLocation(currentNodeID);
        Random rand = new Random();
        SceneManager.LoadScene(mapPool[rand.Next(0,4)]);
    }

    public void TutorialFinish()
    {
        GameManager.Instance.difficultySum += mapGenerator.graph.Nodes[currentNodeID].Difficulty;
        mapGenerator.ChangePlayerLocation(currentNodeID);
        PlayerPrefs.SetString("TutorialFinished", "yes");
        Random rand = new Random();
        SceneManager.LoadScene(mapPool[rand.Next(0,4)]);
    }
}
