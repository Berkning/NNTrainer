using System;
using System.Collections;
using UnityEngine;

public class Trainer : MonoBehaviour
{
    [SerializeField] protected Network network;
    [SerializeField] private LossFunction lossFunction;
    [SerializeField] private Optimizer optimizer;
    [SerializeField] private LossGraph lossGraph;
    [SerializeField] private int batchSize = 1;
    [SerializeField] private int epochs = 1;
    [SerializeField] private bool useSGD;
    private double accumulatedLoss = 0d;
    private int lossAccumulationCount = 0;

    public double GetAndResetLoss()
    {
        double loss = accumulatedLoss / lossAccumulationCount;

        accumulatedLoss = 0d;
        lossAccumulationCount = 0;

        return loss;
    }

    public void StartTraining(DataSet dataSet)
    {
        if (dataSet.inputWidth != network.layers[0].size)
        {
            Console.WriteLine("DataSet doesn't fit network input. Expected: " + network.layers[0].size + " Got: " + dataSet.inputWidth);
            return;
        }

        if (dataSet.outputWidth != network.sizeOfOutputLayer)
        {
            Console.WriteLine("DataSet doesn't fit network output. Expected: " + network.sizeOfOutputLayer + " Got: " + dataSet.outputWidth);
            return;
        }


        StartCoroutine(TrainingLoop(dataSet));
    }

    private IEnumerator TrainingLoop(DataSet dataSet)
    {
        for (int e = 0; e < epochs; e++)
        {
            for (int d = 0; d < dataSet.Length; d++)
            {
                double[] input = new double[dataSet.inputWidth]; //TODO: Just get a span from dataset
                for (int i = 0; i < dataSet.inputWidth; i++)
                {
                    input[i] = dataSet[d, i];
                }

                double[] label = new double[dataSet.outputWidth]; //TODO: Just get a span from dataset
                for (int i = 0; i < dataSet.outputWidth; i++)
                {
                    label[i] = dataSet[d, i + dataSet.inputWidth];
                }

                network.Evaluate(input);
                CalculateGradients(label);
                if (useSGD) ApplyGradients(1);
            }

            if (!useSGD) ApplyGradients(dataSet.Length);

            lossGraph.AddPoint((float)GetAndResetLoss());

            VisualizeNetwork();

            yield return null;
        }
    }

    protected virtual void VisualizeNetwork() { }

    public void ApplyGradients(int batchSize) //TODO: remove and make call directly
    {
        optimizer.ApplyGradients(network, batchSize);
    }

    public void CalculateGradients(double[] targets)
    {
        double loss = 0d;
        Span<double> errorDelta = stackalloc double[targets.Length]; //TODO: Don't allocate at all, just store errorDeltas in layer
        errorDelta.Clear();

        int lastLayerIndex = network.layers.Length - 1;
        Layer lastLayer = network.layers[lastLayerIndex];

        for (int i = 0; i < lastLayer.sizeOfNextLayer; i++)
        {
            loss += lossFunction.GetLoss(lastLayer.activations[i], targets[i]);

            //How much does the loss change with respect to the pre-activation values of each neuron in the layer
            errorDelta[i] = lossFunction.GetGradient(lastLayer.activations[i], targets[i]) * lastLayer.activationFunction.GetGradient(lastLayer.results[i]);
        }

        accumulatedLoss += loss;
        lossAccumulationCount++;

        PropogateError(errorDelta, lastLayerIndex);
    }

    //The layer at layerIndex-1 holds the activations for this layer, but the layer at layerIndex hold the weights and biases connecting this layer to the next (The output at first call)
    private void PropogateError(Span<double> lastErrorDelta, int layerIndex)
    {
        Layer layer = network.layers[layerIndex];

        Span<double> errorDelta = stackalloc double[layer.size]; //TODO: Don't allocate at all, just store errorDeltas in layer
        errorDelta.Clear();

        for (int i = 0; i < layer.size; i++)
        {
            for (int w = 0; w < layer.sizeOfNextLayer; w++)
            {
                layer.weightGradients[i, w].gradient += lastErrorDelta[w] * layer.input[i];

                errorDelta[i] += layer.weights[i, w] * lastErrorDelta[w];
            }

            if (layerIndex > 0) errorDelta[i] *= network.layers[layerIndex - 1].activationFunction.GetGradient(network.layers[layerIndex - 1].results[i]);
        }

        for (int b = 0; b < layer.sizeOfNextLayer; b++)
        {
            layer.biasGradients[b].gradient += lastErrorDelta[b];
        }


        if (layerIndex == 0) return;

        PropogateError(errorDelta, layerIndex - 1);
    }
}

public class DataSet
{
    //Stores the data in an order such that
    //each data 'point' contains the doubles
    //that make up the input to the network,
    //or the 'features', immediately followed
    //by the doubles that make up the expected
    //output from the network, or the 'label'
    private double[] data;

    public int Length;

    //How wide the networks input layer is
    public int inputWidth;

    //How wide the networks output layer is
    public int outputWidth;

    //The total width of a single point of
    //data, or the outputWidth + inputWidth
    public int dataPointWidth;


    public double this[int dataPointIndex, int index] => data[dataPointIndex * dataPointWidth + index];



    public DataSet(double[] _data, int _inputWidth, int _outputWidth)
    {
        data = _data;
        inputWidth = _inputWidth;
        outputWidth = _outputWidth;
        dataPointWidth = inputWidth + outputWidth;
        Length = data.Length / dataPointWidth;
    }
}