using System;
using UnityEngine;

public class LossGraph : MonoBehaviour
{
    [SerializeField] private float width = 2f;
    //TODO: Do something to allow very flat graphs to stay somewhere
    //TODO: in the middle of the graph window instead of drifting
    //TODO: towards the top.
    [SerializeField] private float height = 4f;
    [SerializeField] private int capacity = 10000;
    [SerializeField] private LineRenderer line;
    [SerializeField] private Vector3[] points;
    [SerializeField] private bool to;

    void Awake()
    {
        points = new Vector3[capacity];
        line.SetPositions(points);
        line.positionCount = capacity;
    }

    void Update()
    {
        if (to)
        {
            to = false;
            AddPoint(UnityEngine.Random.Range(0.5f, 1f));
        }
    }

    public void AddPoint(float loss)
    {
        float increment = width / capacity;
        float highestPoint = float.MinValue;

        for (int i = capacity - 1; i > 0; i--)
        {
            int reverseIndex = capacity - i;

            points[i].x = reverseIndex * increment;
            points[i].y = points[i - 1].y;
            if (points[i].y > highestPoint) highestPoint = points[i].y;
        }

        points[0].x = (capacity - 1) * increment;
        points[0].y = loss * height;

        if (points[0].y > highestPoint) highestPoint = points[0].y;

        transform.localScale = new Vector3(1, height / highestPoint, 1);

        line.SetPositions(points);
    }
}