using System;
using UnityEngine;

public class LossGraph : MonoBehaviour
{
    [SerializeField] private float width = 2f;
    [SerializeField] private float height = 2f;
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

        for (int i = capacity - 1; i > 0; i--)
        {
            int reverseIndex = capacity - i;

            points[i].x = reverseIndex * increment;
            points[i].y = points[i - 1].y;
        }

        points[0].x = (capacity - 1) * increment;
        points[0].y = loss * height;

        line.SetPositions(points);
    }
}