using UnityEngine;

public class MissionBorder : MonoBehaviour
{
    private Coroutine coroutine;

    void OnDisable()
    {
        StopAllCoroutines();
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }

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
        if (!isActiveAndEnabled)
        {
            return;
        }

        if (collider.CompareTag("Player"))
        {
            if (GameManager.Instance.textPrompt == null) {return;}
            coroutine = StartCoroutine(GameManager.Instance.textPrompt.StartCountDown());
        }
    }
}
