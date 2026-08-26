using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class SelectorButton : MonoBehaviour
{
    public string nodeId;
    public bool isPlayerAdjacent = false;
    public SelectorMapGenerator mapGenerator;

    public void OnClick()
    {
        if (!isPlayerAdjacent) return;

        mapGenerator.ChangePlayerLocation(nodeId);
    }

    public void Selected()
    {
        // TODO needs code here that changes to the selected node's map scene.
        SceneManager.LoadScene("Scenes/MapTestScene");
    }
}
