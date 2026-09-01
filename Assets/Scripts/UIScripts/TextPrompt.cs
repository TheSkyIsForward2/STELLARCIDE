using TMPro;
using UnityEngine;
using System.Collections;

public class TextPrompt : MonoBehaviour
{
    private TextMeshProUGUI textLabel;
    public int returnTime = 10;

    [Tooltip("DEBUG, DONT TOUCH")]
    [SerializeField] private int counter;

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
        // TODO: start a coroutine to slowly dmg player then break;
    }
}