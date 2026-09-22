using UnityEngine;

public class MissionBorder : MonoBehaviour
{
    private Coroutine coroutine;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            GameManager.Instance.textPrompt.isVisible = false;
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            coroutine = StartCoroutine(GameManager.Instance.textPrompt.StartCountDown());
        }
    }
}
