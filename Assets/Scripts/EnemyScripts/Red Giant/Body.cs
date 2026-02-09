using UnityEngine;

public class Body : MonoBehaviour
{
    public Transform target;
    public float followDistance = 0.5f;
    public float followSpeed = 5f;

    private Vector2 lastPos;
    public bool freeze = false;

    void Start()
    {
        lastPos = transform.position;
    }

    void Update()
    {
        if (!target || freeze) return;

        Vector2 dir = (Vector2)(target.position - transform.position);
        Vector2 desiredPos = (Vector2)target.position - dir.normalized * followDistance;

        Vector2 delta = desiredPos - lastPos;
        Vector2 forwardDir = dir.normalized;
        float forwardMovement = Vector2.Dot(delta, forwardDir);

        if (forwardMovement > 0f)
        {
            Vector2 move = forwardDir * forwardMovement;

            transform.position = Vector2.MoveTowards(transform.position, lastPos + move, followSpeed * Time.deltaTime);
        }

        transform.up = dir.normalized;

        lastPos = transform.position;
    }
}
