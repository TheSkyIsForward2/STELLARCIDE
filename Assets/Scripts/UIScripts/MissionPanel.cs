using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionPanel : MonoBehaviour
{
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
        SceneManager.LoadScene("Mission");
    }
}
