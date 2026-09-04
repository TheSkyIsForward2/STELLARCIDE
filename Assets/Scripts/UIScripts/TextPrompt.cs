using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TextPrompt : MonoBehaviour
{
    private TextMeshProUGUI textLabel;
    public int returnTime = 10;
    private int counter;

    public bool isVisible 
    {
        set
        { 
            textLabel.enabled = value; 
            if (!value) {counter = returnTime;}
        }
        get
        { 
            return textLabel.enabled;
        }
    }

    void Awake()
    {
        textLabel = GetComponent<TextMeshProUGUI>();
        textLabel.text = $"you shouldnt see this B==D";
        isVisible = false;
        counter = returnTime;

        GameManager.Instance.textPrompt = this;
    }

    public IEnumerator StartCountDown()
    {
        isVisible = true;
        while (counter > 0)
        {
            textLabel.text = $"RETURN TO MISSION ZONE IN {counter--}";
            yield return new WaitForSeconds(1);
        }
        GameManager.Instance.Player.GetComponent<HealthOwner>().TakeDOT(
            -1,
            new Damage(20,Damage.Type.PHYSICAL),
            1
        );
    }
}