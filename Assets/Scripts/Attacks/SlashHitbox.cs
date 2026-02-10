using UnityEngine;

public class SlashHitbox : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"enter triggered {collision.gameObject.name}");
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log($"exit triggered {collision.gameObject.name}");
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log($"stay triggered {collision.gameObject.name}");
    }
}