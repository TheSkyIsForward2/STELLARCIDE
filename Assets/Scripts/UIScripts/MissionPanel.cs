using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionPanel : MonoBehaviour
{
    public string currentNodeID;
    public TMP_Text description;
    public TMP_Text scoreMult;
    private SelectorMapGenerator mapGenerator;

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
        SceneManager.LoadScene("Mission");
    }
}
