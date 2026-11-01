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
                Vector2 position = new Vector2(startX + i * horizontalSpace, startY + j * verticalSpace);

                Instantiate(nodePrefab, position, Quaternion.identity, transform);
            }
        }


        List<Vector3> linePositions = new List<Vector3>();

        for (int layer = 0; layer < layers.Count - 1; layer++)
        {
            for (int node = 0; node < layers[layer]; node++)
            {
                Vector2 currentNodePos = GetNodePosition(layer, node, layers);

                for (int i = layers[layer + 1] - 1; i >= 0; i--)
                {
                    Vector2 otherNodePos = GetNodePosition(layer + 1, i, layers);

                    linePositions.Add(currentNodePos);
                    linePositions.Add(otherNodePos);
                }
            }
        }



        lineRenderer.positionCount = linePositions.Count;

        lineRenderer.SetPositions(linePositions.ToArray());
    }

    private Vector2 GetNodePosition(int layerIndex, int nodeIndex, List<int> layers) //TODO: Make this much cleaner and more performant when we have a proper layer struct/class
    {
        float startX = -(layers.Count - 1f) * horizontalSpace / 2f;
        float startY = -(layers[layerIndex] - 1f) * verticalSpace / 2f;

        return new Vector2(startX + layerIndex * horizontalSpace, startY + nodeIndex * verticalSpace);
    }
}