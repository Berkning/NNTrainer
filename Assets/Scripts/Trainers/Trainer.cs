using System;
using System.Collections;
using UnityEngine;

public class Trainer : MonoBehaviour
{
    [SerializeField] protected Network network;
    [SerializeField, Range(0f, 1f)] protected float validationPercent;
    [SerializeField] private LossFunction lossFunction;
    [SerializeField] private Optimizer optimizer;
    [SerializeField] private LossGraph trainingLossGraph;
    [SerializeField] private LossGraph validationLossGraph;
    [SerializeField] private int batchSize = 1;
    [SerializeField] private int epochs = 1;
    private double accumulatedLoss = 0d;
    private int lossAccumulationCount = 0;

    public double GetAndResetLoss()
    {
        double loss = accumulatedLoss / lossAccumulationCount;

        accumulatedLoss = 0d;
        lossAccumulationCount = 0;

        return loss;
    }

    public double GetValidationLoss(DataSet dataSet)
    {
        if (dataSet.ValidationLength == 0) return 0d;

        double loss = 0d;
        double[] label = new double[dataSet.outputWidth];
        double[] input = new double[dataSet.inputWidth];

        Layer lastLayer = network.layers[network.layers.Length - 1];

        for (int d = dataSet.TrainingLength; d < dataSet.TotalLength; d++)
        {
            //Get the input for the current dataPoint
            for (int i = 0; i < dataSet.inputWidth; i++)
            {
                input[i] = dataSet[d, i];
            }

            //Get label for the current dataPoint
            for (int i = 0; i < dataSet.outputWidth; i++)
            {
                label[i] = dataSet[d, i + dataSet.inputWidth];
            }

            network.Evaluate(input);

            //Get individual loss for each output neuron
            for (int i = 0; i < network.sizeOfOutputLayer; i++)
            {
                loss += lossFunction.GetLoss(lastLayer.activations[i], label[i]);
            }
        }

        return loss / dataSet.ValidationLength;
    }

    public void StartTraining(DataSet dataSet)
    {
        if (dataSet.inputWidth != network.layers[0].size)
        {
            Debug.Log("DataSet doesn't fit network input. Expected: " + network.layers[0].size + " Got: " + dataSet.inputWidth);
            return;
        }

        if (dataSet.outputWidth != network.sizeOfOutputLayer)
        {
            Debug.Log("DataSet doesn't fit network output. Expected: " + network.sizeOfOutputLayer + " Got: " + dataSet.outputWidth);
            return;
        }


        StartCoroutine(TrainingLoop(dataSet));
    }

    private IEnumerator TrainingLoop(DataSet dataSet)
    {
        for (int e = 0; e < epochs; e++)
        {
            int batchCount = 0;

            for (int d = 0; d < dataSet.TrainingLength; d++)
            {
                batchCount++;

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

                //BatchCount will ofc never be bigger, but
                //it can be changed during runtime, which
                //will cause edge cases
                if (batchCount >= batchSize)
                {
                    ApplyGradients(batchCount);
                    batchCount = 0;
                }
            }

            if (batchCount > 0) ApplyGradients(batchCount);

            trainingLossGraph.AddPoint((float)GetAndResetLoss());
            validationLossGraph.AddPoint((float)GetValidationLoss(dataSet));

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

[System.Serializable]
public class DataSet
{
    //Stores the data in an order such that
    //each data 'point' contains the doubles
    //that make up the input to the network,
    //or the 'features', immediately followed
    //by the doubles that make up the expected
    //output from the network, or the 'label'
    [SerializeField] private double[] data;

    //The amount of entries dedicated to training
    public int TrainingLength;
    //The amount of entries dedicated to validation
    public int ValidationLength;
    //The total amount of entries
    public int TotalLength;

    //How wide the networks input layer is
    public int inputWidth;

    //How wide the networks output layer is
    public int outputWidth;

    //The total width of a single point of
    //data, or the outputWidth + inputWidth
    public int dataPointWidth;


    public double this[int dataPointIndex, int index] => data[dataPointIndex * dataPointWidth + index];



    public DataSet(double[] _data, int _inputWidth, int _outputWidth, float validationPercent)
    {
        data = _data;
        inputWidth = _inputWidth;
        outputWidth = _outputWidth;
        dataPointWidth = inputWidth + outputWidth;
        TotalLength = data.Length / dataPointWidth;

        ValidationLength = Mathf.RoundToInt(validationPercent * TotalLength);
        TrainingLength = TotalLength - ValidationLength;

        Debug.Log("Training Entries: " + TrainingLength + "   Validation Entries: " + ValidationLength);
    }
}