using System.Collections.Generic;
using UnityEngine;

public class NetworkVisualizer : MonoBehaviour
{
    public List<int> networkLayers = new List<int>();
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float verticalSpace;
    [SerializeField] private float horizontalSpace;

    void Start()
    {
        SpawnNetwork(networkLayers);
    }

    public void SpawnNetwork(List<int> layers)
    {
        float startX = -(layers.Count - 1f) * horizontalSpace / 2f;

        for (int i = 0; i < layers.Count; i++)
        {
            float startY = -(layers[i] - 1f) * verticalSpace / 2f;

            for (int j = 0; j < layers[i]; j++)
            {
                Instantiate(nodePrefab, new Vector2(startX + i * horizontalSpace, startY + j * verticalSpace), Quaternion.identity, transform);
            }
        }
    }
}