using System;
using MapScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class SelectorButton : MonoBehaviour
{
    public string nodeId;
    public bool isPlayerAdjacent = false;
    public SelectorMapGenerator mapGenerator;
    public GraphUIRenderer graphUIRenderer;

    private void Start()
    {
        graphUIRenderer =  FindAnyObjectByType<GraphUIRenderer>();
    }

    public void OnClick()
    {
        if (!isPlayerAdjacent) return;

        graphUIRenderer.MissionChange(gameObject, nodeId);
    }

    public void Selected()
    {
        // TODO needs code here that changes to the selected node's map scene.
        mapGenerator.ChangePlayerLocation(nodeId);
        SceneManager.LoadScene("Scenes/MapTestScene");
    }
}
