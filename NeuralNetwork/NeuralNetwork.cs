using System.Security.Cryptography;
using System.Text;

namespace AICarTrack;

/// <summary>
/// Implementation of a feedforward neural network, for use with the cars.
///
///    LAYERS  NEURONS
///      |      |   |
///     \|/    \|/ \|/
///     
///           {VISION}
///           
///     INPUT: (O) (O)   } 0..1 for vision, "1" means wall is close | "0" wall is not near. Computed:  1-(distance to wall pixel / depth of pixels to check)
///             |\ /|
///             | * |
///             |/ \|
///    HIDDEN: (O) (O)   } can be multiple neurons (1...n)
///      (1)    |\ /|
///             | * |
///             |/ \|
///              ...     } can be multiple layers (1...n)
///    HIDDEN: (O) (O)   
///      (n)    |\ /|
///             | * |
///             |/ \|
///    OUTPUT: (O) (O)
///    
///        {STEER} {THROTTLE}    <-- we use to control the car
///         -1..1     -1..1
///         
///   A neuron is simply:
///      output = SUM( weight * input ) + bias
///                "weight" amplifies or reduces the input it receives from a neuron that feeds into it. It is from the conceptual dendrite.
///                "bias" is how much is added to the neuron output. (think fires when it reaches a threshold, this lowers the need for the
///                neuron to fire for the output to be "on" full.
/// </summary>
public class NeuralNetwork
{
    /// <summary>
    /// Callback function, that visualises the network (monitor)
    /// </summary>
    /// <param name="network"></param>
    internal delegate void MonitorCallback(NeuralNetwork network);

    /// </summary>
    /// Tracks the neural networks.
    /// <summary>
    internal static Dictionary<int, NeuralNetwork> s_networks = new();

    /// <summary>
    /// The "id" (index) of the brain, should also align to the "id" of the car it is attached.
    /// </summary>
    internal int Id;

    /// <summary>
    /// How many layers of neurons (3+). Do not do 2 or 1.
    /// 2 => input connected to output.
    /// 1 => input is output, and feed forward will crash.
    /// </summary>
    internal readonly int[] Layers;

    /// <summary>
    /// The neurons.
    /// [layer][neuron]
    /// </summary>
    internal double[][] Neurons;

    /// <summary>
    /// NN Biases. Either improves or lowers the chance of this neuron fully firing.
    /// [layer][neuron]
    /// </summary>
    internal double[][] Biases;

    /// <summary>
    /// NN weights. Reduces or amplifies the output for the relationship between neurons in each layer
    /// [layer][neuron][neuron]
    /// </summary>
    internal double[][][] Weights;

    /// <summary>
    /// Indicator for how fit this NN is for the purpose.
    /// </summary>
    internal float Fitness = 0;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="_id">Unique ID of the neuron.</param>
    /// <param name="layerDefinition">Defines size of the layers.</param>
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Init*() set the fields.
    internal NeuralNetwork(int _id, int[] layerDefinition, bool addToList = true)
#pragma warning restore CS8618
    {
        // (1) INPUT (2) HIDDEN (3) OUTPUT. Less than 3 would be INPUT->OUTPUT; hardly "AI" but actually works
        if (layerDefinition.Length < 2) throw new ArgumentException(nameof(layerDefinition));

        Id = _id; // used to reference this network

        // copy layerDefinition to Layers; although for cars this must not change.     
        Layers = new int[layerDefinition.Length];

        for (int layer = 0; layer < layerDefinition.Length; layer++)
        {
            Layers[layer] = layerDefinition[layer];
        }

        // if layerDefinition is [2,3,2] then...
        // 
        // Neurons :      (o) (o)    <-2  INPUT
        //              (o) (o) (o)  <-3
        //                (o) (o)    <-2  OUTPUT
        //

        InitialiseNeurons();
        InitialiseBiases();
        InitialiseWeights();

        // track all the neurons we created
        if (addToList)
        {
            if (!s_networks.ContainsKey(Id)) s_networks.Add(Id, this); else s_networks[Id] = this;
        }
    }

    /// <summary>
    /// Create empty storage array for the neurons in the network.
    /// </summary>
    private void InitialiseNeurons()
    {
        List<double[]> neuronsList = new();

        // if layerDefinition is [2,3,2] ..   float[]
        // Neurons :      (o) (o)    <-2  ... [ 0, 0 ]
        //              (o) (o) (o)  <-3  ... [ 0, 0, 0 ]
        //                (o) (o)    <-2  ... [ 0, 0 ]
        //

        for (int layer = 0; layer < Layers.Length; layer++)
        {
            neuronsList.Add(new double[Layers[layer]]);
        }

        Neurons = neuronsList.ToArray();
    }

    /// <summary>
    /// Generate a random number between -0.5...+0.5.
    /// </summary>
    /// <returns></returns>
    private float RandomFloatBetweenMinusHalfToPlusHalf()
    {
        return (float)(RandomNumberGenerator.GetInt32(0, 1000) - 500) / 1000;
    }

    /// <summary>
    /// initializes and populates biases.
    /// </summary>
    private void InitialiseBiases()
    {
        List<double[]> biasList = new();

        // for each layer of neurons, we have to set biases.
        for (int layer = 0; layer < Layers.Length; layer++)
        {
            double[] bias = new double[Layers[layer]];

            for (int biasLayer = 0; biasLayer < Layers[layer]; biasLayer++)
            {
                bias[biasLayer] = 0;
            }

            biasList.Add(bias);
        }

        Biases = biasList.ToArray();
    }

    /// <summary>
    /// Initializes random array for the weights being held in the network.
    /// </summary>
    private void InitialiseWeights()
    {
        List<double[][]> weightsList = new(); // used to construct weights, as dynamic arrays aren't supported

        for (int layer = 1; layer < Layers.Length; layer++)
        {
            List<double[]> layerWeightsList = new();

            int neuronsInPreviousLayer = Layers[layer - 1];

            for (int neuronIndexInLayer = 0; neuronIndexInLayer < Neurons[layer].Length; neuronIndexInLayer++)
            {
                double[] neuronWeights = new double[neuronsInPreviousLayer];

                for (int neuronIndexInPreviousLayer = 0; neuronIndexInPreviousLayer < neuronsInPreviousLayer; neuronIndexInPreviousLayer++)
                {
                    neuronWeights[neuronIndexInPreviousLayer] = RandomFloatBetweenMinusHalfToPlusHalf();
                }

                layerWeightsList.Add(neuronWeights);
            }

            weightsList.Add(layerWeightsList.ToArray());
        }

        Weights = weightsList.ToArray();
    }

    /// <summary>
    /// Feed forward, inputs >==> outputs.
    /// 
    ///     input       input
    ///         |          |
    ///         v[0] w[0]  v[1] w[1]              w = weight
    /// l0    ( 0 )      ( 1 )                    v = value
    ///         |    \  /  |                      b = bias
    ///         |     /    |     
    ///         |   /   \  |
    /// l1    ( 0 )      ( 1 )
    ///         |          |
    ///         |     b(1) |                      l0 node 0                    l0 node 1            bias of l1 node 1
    ///    b(0) |          v[1] = Activate( w[l0][1][0] * v[l0][0] +  w[l0][1][1] * v[l0][1]   +   b[l1][1] ) 
    ///         |                  l0 node 0                l0 node 1                     bias of l1 node 0
    ///         v[0] = Activate( w[l0][0][0] * v[l0][0] +  w[l0][0][1] * v[l0][1]   +   b[l1][0] )
    /// 
    /// </summary>
    /// <param name="inputs"></param>
    /// <returns></returns>
    internal double[] FeedForward(double[] inputs)
    {
        // put the INPUT values into layer 0 neurons
        for (int i = 0; i < inputs.Length; i++)
        {
            Neurons[0][i] = inputs[i];
        }

        // we start on layer 1 as we are computing values from prior layers (layer 0 is inputs)
        for (int layer = 1; layer < Layers.Length; layer++)
        {
            for (int neuronIndexForLayer = 0; neuronIndexForLayer < Neurons[layer].Length; neuronIndexForLayer++)
            {
                // sum of outputs from the previous layer
                double value = 0F;

                for (int neuronIndexInPreviousLayer = 0; neuronIndexInPreviousLayer < Neurons[layer - 1].Length; neuronIndexInPreviousLayer++)
                {
                    // remember: the "weight" amplifies or reduces, so we take the output of the prior neuron and "amplify/reduce" it's output here
                    value += Weights[layer - 1][neuronIndexForLayer][neuronIndexInPreviousLayer] * Neurons[layer - 1][neuronIndexInPreviousLayer];
                }

                // any neuron fires or not based on the input. The point of a bias is to move the activation up or down.
                // e.g. the value could be 0.3, adding a bias of 0.5 takes it to 0.8. You might think why not just use the weights to achieve this
                // but remember weights are individual per prior layer neurons, the bias affects the SUM() of them.

                Neurons[layer][neuronIndexForLayer] = Activate(value + Biases[layer][neuronIndexForLayer]);
            }
        }

        return Neurons[^1]; // final* layer contains OUTPUT
    }

    /// <summary>
    /// 
    /// </summary>
    internal void Visualise()
    {
        callBackWhenAfterNeuronsFire?.Invoke(this);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    internal string Formula()
    {
        Dictionary<string, string> values = new();

        int neurons = 0;
        for (int layer = 0; layer < Layers.Length; layer++)
        {
            neurons += Neurons[layer].Length;
            if (neurons > 30) return "too big to output (exceed 30 neurons)";
        }

        for (int neuronIndexForLayer = 0; neuronIndexForLayer < Neurons[0].Length; neuronIndexForLayer++)
        {
            values.Add($"0-{neuronIndexForLayer}", $"visionSensor[{neuronIndexForLayer}]");
        }

        // we start on layer 1 as we are computing values from prior layers (layer 0 is inputs)
        for (int layer = 1; layer < Layers.Length; layer++)
        {
            List<string> dictionaryEntriesWeShouldRemove = new();

            for (int neuronIndexForLayer = 0; neuronIndexForLayer < Neurons[layer].Length; neuronIndexForLayer++)
            {
                // sum of outputs from the previous layer
                StringBuilder valueFormula = new(20);
                valueFormula.AppendLine("");

                for (int neuronIndexInPreviousLayer = 0; neuronIndexInPreviousLayer < Neurons[layer - 1].Length; neuronIndexInPreviousLayer++)
                {
                    // remember: the "weight" amplifies or reduces, so we take the output of the prior neuron and "amplify/reduce" it's output here
                    string weight = Weights[layer - 1][neuronIndexForLayer][neuronIndexInPreviousLayer].ToString();

                    string key = $"{layer - 1}-{neuronIndexInPreviousLayer}";
                    string neuronvalue = values[key];

                    if (!dictionaryEntriesWeShouldRemove.Contains(key)) dictionaryEntriesWeShouldRemove.Add(key);
                    // value += Weights[layer - 1][neuronIndexForLayer][neuronIndexInPreviousLayer] * Neurons[layer-1][neuronIndexInPreviousLayer];
                    //          ^ weight                                                                             ^ neuronValue
                    valueFormula.AppendLine($"(/* weight L{layer}.N{neuronIndexForLayer}-L{layer - 1}.N{neuronIndexInPreviousLayer} x value */ {weight}*{neuronvalue})+");
                }

                valueFormula.AppendLine("");
                string value = valueFormula.ToString().Trim('+');

                // Neurons[layer][neuronIndexForLayer] = Activate(value + Biases[layer][neuronIndexForLayer]);
                values.Add($"{layer}-{neuronIndexForLayer}", $"/* L{layer}.N{neuronIndexForLayer} -> */ Math.Tanh({value}+{Biases[layer][neuronIndexForLayer]})");
            }

            // reduce dictionary, as each iteration embeds the previous layer
            foreach (string key in dictionaryEntriesWeShouldRemove) values.Remove(key);
        }

        string result = ($"        outputFromNeuralNetwork = new double[{(Neurons[^1].Length > 1 ? 2 : 1)}];\n" +
                         $"        /* steering */ outputFromNeuralNetwork[0] = {values[(Layers.Length - 1).ToString() + "-0"]};\n\n").Replace("+-", "-");

        if (Neurons[^1].Length > 1)
            result += ($"        /* speed */ outputFromNeuralNetwork[1] = {values[(Layers.Length - 1).ToString() + "-1"]};").Replace("+-", "-");

        File.WriteAllText(@"c:\temp\formula.txt", result);
        return result;
    }

    /// <summary>
    /// Activate is TANH         1_       ___
    /// (hyperbolic tangent)     0_      /
    ///                         -1_  ___/
    ///                                | | |
    ///                     -infinity -2 0 2..infinity
    ///                               
    /// i.e. TANH flatters any value to between -1 and +1.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    internal static double Activate(double value)
    {
        return Math.Tanh(value);
    }

    /// <summary>
    /// A simple mutation function for any genetic implementations, ensuring it DOES mutate.
    /// </summary>
    /// <param name="pctChance"></param>
    /// <param name="val"></param>
    internal void Mutate(int pctChance, float val)
    {
        bool mutated = false;

        while (!mutated) // ensure SOMETHING changes, otherwise we'll get two identical cars.
        {
            for (int layerIndex = 0; layerIndex < Biases.Length; layerIndex++)
            {
                for (int neuronIndex = 0; neuronIndex < Biases[layerIndex].Length; neuronIndex++)
                {
                    if (RandomNumberGenerator.GetInt32(0, 100) <= pctChance)
                    {
                        mutated = true;
                        Biases[layerIndex][neuronIndex] += (float)(RandomNumberGenerator.GetInt32((int)(-val * 10000), (int)(val * 10000))) / 20000;
                    }
                }
            }

            for (int layerIndex = 0; layerIndex < Weights.Length; layerIndex++)
            {
                for (int neuronIndexForLayer = 0; neuronIndexForLayer < Weights[layerIndex].Length; neuronIndexForLayer++)
                {
                    for (int neuronIndexInPreviousLayer = 0; neuronIndexInPreviousLayer < Weights[layerIndex][neuronIndexForLayer].Length; neuronIndexInPreviousLayer++)
                    {
                        if (RandomNumberGenerator.GetInt32(0, 100) <= pctChance)
                        {
                            mutated = true;
                            Weights[layerIndex][neuronIndexForLayer][neuronIndexInPreviousLayer] += (float)(RandomNumberGenerator.GetInt32((int)(-val * 10000), (int)(val * 10000))) / 20000;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// This loads the biases and weights from within a file into the neural network.
    /// </summary>
    /// <param name="path"></param>
    internal bool Load(string path)
    {
        if (!File.Exists(path)) return false;

        string[] ListLines = File.ReadAllLines(path);

        int index = 0;

        Fitness = float.Parse(ListLines[index++]);

        try
        {
            for (int layerIndex = 0; layerIndex < Biases.Length; layerIndex++)
            {
                for (int neuronIndex = 0; neuronIndex < Biases[layerIndex].Length; neuronIndex++)
                {
                    Biases[layerIndex][neuronIndex] = double.Parse(ListLines[index++]);
                }
            }

            for (int layerIndex = 0; layerIndex < Weights.Length; layerIndex++)
            {
                for (int neuronIndexInLayer = 0; neuronIndexInLayer < Weights[layerIndex].Length; neuronIndexInLayer++)
                {
                    for (int neuronIndexInPreviousLayer = 0; neuronIndexInPreviousLayer < Weights[layerIndex][neuronIndexInLayer].Length; neuronIndexInPreviousLayer++)
                    {
                        Weights[layerIndex][neuronIndexInLayer][neuronIndexInPreviousLayer] = double.Parse(ListLines[index++]);
                    }
                }
            }
        }
        catch (Exception)
        {
            MessageBox.Show("Unable to load .AI files\nThe most likely reason is that the number of neurons does not match the saved AI file.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Saves the biases and weights within the network to a file.
    /// </summary>
    /// <param name="path"></param>
    internal void Save(string path)
    {
        using StreamWriter writer = new(path, false);

        writer.WriteLine(Fitness);

        // write the biases
        for (int layerIndex = 0; layerIndex < Biases.Length; layerIndex++)
        {
            for (int neuronIndex = 0; neuronIndex < Biases[layerIndex].Length; neuronIndex++)
            {
                writer.WriteLine(Biases[layerIndex][neuronIndex]);
            }
        }

        // write the weights
        for (int layerIndex = 0; layerIndex < Weights.Length; layerIndex++)
        {
            for (int neuronIndexInLayer = 0; neuronIndexInLayer < Weights[layerIndex].Length; neuronIndexInLayer++)
            {
                for (int neuronIndexInPreviousLayer = 0; neuronIndexInPreviousLayer < Weights[layerIndex][neuronIndexInLayer].Length; neuronIndexInPreviousLayer++)
                {
                    writer.WriteLine(Weights[layerIndex][neuronIndexInLayer][neuronIndexInPreviousLayer]);
                }
            }
        }

        writer.Close();
    }

    /// <summary>
    /// Sorts the network so fitter AI networks appear at the bottom.
    /// </summary>
    internal static void SortNetworkByFitness()
    {
        s_networks = s_networks.OrderBy(x => x.Value.Fitness).ToDictionary(x => x.Key, x => x.Value);
    }

    /// <summary>
    /// Sorts the network so fitter AI networks appear at the bottom.
    /// </summary>
    internal static void ReverseSortNetworkByFitness()
    {
        s_networks = s_networks.OrderByDescending(x => x.Value.Fitness).ToDictionary(x => x.Key, x => x.Value);
    }

    /// <summary>
    /// Copies from one NN to another
    /// </summary>
    /// <param name="neuralNetworkToCloneFrom"></param>
    /// <param name="neuralNetworkCloneTo"></param>
    internal static void CopyFromTo(NeuralNetwork neuralNetworkToCloneFrom, NeuralNetwork neuralNetworkCloneTo)
    {
        for (int layerIndex = 0; layerIndex < neuralNetworkToCloneFrom.Biases.Length; layerIndex++)
        {
            for (int neuronIndex = 0; neuronIndex < neuralNetworkToCloneFrom.Biases[layerIndex].Length; neuronIndex++)
            {
                neuralNetworkCloneTo.Biases[layerIndex][neuronIndex] = neuralNetworkToCloneFrom.Biases[layerIndex][neuronIndex];
            }
        }

        for (int layerIndex = 0; layerIndex < neuralNetworkToCloneFrom.Weights.Length; layerIndex++)
        {
            for (int neuronIndexInLayer = 0; neuronIndexInLayer < neuralNetworkToCloneFrom.Weights[layerIndex].Length; neuronIndexInLayer++)
            {
                for (int neuronIndexInPreviousLayer = 0; neuronIndexInPreviousLayer < neuralNetworkToCloneFrom.Weights[layerIndex][neuronIndexInLayer].Length; neuronIndexInPreviousLayer++)
                {
                    neuralNetworkCloneTo.Weights[layerIndex][neuronIndexInLayer][neuronIndexInPreviousLayer] = neuralNetworkToCloneFrom.Weights[layerIndex][neuronIndexInLayer][neuronIndexInPreviousLayer];
                }
            }
        }
    }

    #region MONITOR
    /// <summary>
    /// Invoke() when neural network is recalculated to allow us to monitor. 
    /// We intentionally have ONE callback and ONE network to monitor as the hit from visualisers is noticeable.
    /// </summary>
    private static MonitorCallback? callBackWhenAfterNeuronsFire;

    /// <summary>
    /// If a caller wants to be notified each time the NN is used, this is set to the neural network number.
    /// This could have been a pointer to a NN object, but previously this came unstuck when we instantiated
    /// new ones during mutations. By doing this, it points to the one of interest
    /// </summary>
    private static int IdOfNeuralNetworkBeingMonitored { get; set; } = -1;

    /// <summary>
    /// Returns which neural network we are monitoring.
    /// </summary>
    internal static int IdBeingMonitored
    {
        get { return IdOfNeuralNetworkBeingMonitored; }
    }

    /// <summary>
    /// Returns true if we are monitoring a neural network.
    /// </summary>
    internal static bool IsMontoring
    {
        get { return callBackWhenAfterNeuronsFire is not null; }
    }

    /// <summary>
    /// Allows a caller to be notified when this neural network changes.
    /// </summary>
    /// <param name="callBackFromMonitor">Method that will be called if id monitored changes.</param>
    /// <param name="id">Id of the neural network to monitor</param>
    internal static void Monitor(MonitorCallback callBackFromMonitor)
    {
        callBackWhenAfterNeuronsFire = callBackFromMonitor;
    }

    /// <summary>
    /// Turn off monitoring of NN.
    /// </summary>
    internal static void StopMonitor()
    {
        callBackWhenAfterNeuronsFire = null;
    }
    #endregion
}