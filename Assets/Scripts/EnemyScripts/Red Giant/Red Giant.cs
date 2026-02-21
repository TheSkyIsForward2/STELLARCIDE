using System.Collections.Generic;
using UnityEngine;

public class RedGiant : MonoBehaviour
{

    [Header("Body Info")]
    public int SegmentCount = 0;
    public GameObject HeadPrefab;
    public GameObject BodyPrefab;
    public GameObject TailPrefab;

    private float bodySpacing = 0.01f;

    private List<GameObject> bodyParts = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Create head
        GameObject head = Instantiate(HeadPrefab, transform.position, transform.rotation, transform);
        head.GetComponent<MarkerManager>().ClearMarkerList();
        bodyParts.Add(head);

        CreateBody();
    }

    float countUp = 0;
    private void CreateBody()
    {
        MarkerManager markM = bodyParts[bodyParts.Count - 1].GetComponent<MarkerManager>();
        if (countUp == 0)
        {
            markM.ClearMarkerList();
        }

        countUp += Time.deltaTime;
        if (countUp >= bodySpacing)
        {
            GameObject body = Instantiate(BodyPrefab, transform.position,
                transform.rotation, transform);
            body.GetComponent<MarkerManager>().ClearMarkerList();
            bodyParts.Add(body);
            countUp = 0;
        }
    }

    private void FixedUpdate()
    {
        if (bodyParts.Count < SegmentCount + 1)
        {
            CreateBody();
        }
        if (bodyParts.Count > 1)
        {
            for (int i = 1; i < bodyParts.Count; i++)
            {
                MarkerManager markM = bodyParts[i - 1].GetComponent<MarkerManager>();
                bodyParts[i].transform.position = markM.markerList[0].position;
                bodyParts[i].transform.rotation = markM.markerList[0].rotation;
                markM.markerList.RemoveAt(0);
            }
        }
    }
}
