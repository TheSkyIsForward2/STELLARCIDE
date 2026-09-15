using TMPro;
using UnityEngine;

public class MissionGoalUI : MonoBehaviour
{
    public TextMeshProUGUI objective;
    public TextMeshProUGUI counter;
    private void Awake()
    {
        GameManager.Instance.MissionGoalUI = this;
    }
}
