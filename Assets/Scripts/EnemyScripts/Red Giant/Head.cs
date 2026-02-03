using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class Head : MonoBehaviour
{
    public Transform Player;

    [Header("Body Info")]
    public int SegmentCount = 0;
    public Body BodyPrefab;
    public Tail TailPrefab;
    // Hold references to each part
    private List<Body> BodySegments = new List<Body>();
    private Tail TailSegment;

    private void Start()
    {
        Transform prevTransform = transform;
        for (int i = 1; i < SegmentCount + 1; i++)
        {
            Body body = Instantiate(BodyPrefab, new Vector2(transform.position.x, transform.position.y + (i)), transform.rotation);
            body.target = prevTransform;
            BodySegments.Add(body);
            prevTransform = body.transform;
        }
        TailSegment = Instantiate(TailPrefab, new Vector2(transform.position.x, transform.position.y + (SegmentCount + 1)), transform.rotation);
        TailSegment.target = prevTransform;
    }


}
