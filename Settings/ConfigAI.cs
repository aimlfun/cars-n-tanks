using CarsAndTanks.Vision;

namespace CarsAndTanks.Settings;

/// <summary>
/// Settings that relate to AI behaviour.
/// </summary>
internal class ConfigAI
{
    /// <summary>
    /// This determines which output "neuron" is controlling the steering.
    /// </summary>
    internal const int c_steeringOrThrottle2Neuron = 0;

    /// <summary>
    /// This determines which output "neuron" is controlling the speed.
    /// </summary>
    internal const int c_throttleNeuron = 1;

    /// This determines which output "neuron" is controlling the brakes.
    /// </summary>
    internal const int c_brakeNeuron = 2;

    /// <summary>
    /// This determines which output "neuron" is controlling the e-brakes.
    /// </summary>
    internal const int c_eBrakeNeuron = 3;

    /// <summary>
    /// c_*Neuron are 0..3, but if their modulation is 0, we don't create a neuron for it.
    /// Reference this as [c_*] => neuron index.
    /// </summary>
    internal readonly Dictionary<int, int> NeuronMapTypeToOutput = new();

    /// <summary>
    /// Enable different vision systems.
    /// </summary>
    internal IVision VisionSystem = new MonoLIDAR();

    /// <summary>
    /// Determines the likelihood of mutation.
    /// </summary>
    public int MutationChance { get; set; } = 50;

    /// <summary>
    /// Determines how much change any mutation will cause.
    /// </summary>
    public float MutationStrength { get; set; } = 0.5f;

    /// <summary>
    /// Learning requires us to create a number of cars, and mutate the worst 50%. 
    /// This happens repeatedly, resulting in a more fitting NN being selected.
    /// </summary>
    public int NumberOfAICarsToCreate { get; set; } = 28;

    /// <summary>
    /// After this amount of MOVES has elapsed, a mutation occurs.
    /// </summary>
    public int CarMovesBeforeFirstMutation { get; set; } = 1000; // moves

    /// <summary>
    /// This enables the learning to increase the time so that cars get longer after
    /// each mutation. The idea being that they reach further, and weed out those that
    /// will eventually fail.
    /// </summary>
    public int PercentageIncreaseBetweenMutations { get; set; } = 10; // 0..100%

    /// <summary>
    /// Initializing network to the right size.
    /// Experiment with it to see what happens... { 1,2,2 } | { 5, 25, 20, 50, 2 }  { 5, 2, 2 } | { 5, 20, 10,  2 }
    /// First=INPUT
    /// Last=OUTPUT
    /// Use the UI to configure it.
    /// </summary>
    public int[] Layers { get; set; } = new int[3] { 2, 2, 2 };

    /// <summary>
    /// Used to "amplify" or "reduce" NN output.
    /// Speed Amplifier: Generally keep as 1, but if more than 1, this amplifies the output of the neural network.
    /// Steering Amplifier: if more than 1, this amplifies the rotational output of the neural network.
    /// i.e if set to 5 instead of turning 1 degree, it turns 5 degrees.
    /// </summary>
    private float[] outputModulation = new float[] { 50F, 50F, 0, 0 };

    /// <summary>
    /// Used to "amplify" or "reduce" NN output.
    /// Speed Amplifier: Generally keep as 1, but if more than 1, this amplifies the output of the neural network.
    /// Steering Amplifier: if more than 1, this amplifies the rotational output of the neural network.
    /// i.e if set to 5 instead of turning 1 degree, it turns 5 degrees.
    /// </summary>
    public float[] OutputModulation
    {
        get { return outputModulation; }
        set
        {
            outputModulation = value;

            // if the whole thing changes, we need to update the map of outputs to the neurons
            SetInternalOutMap();
        }
    }

    /// <summary>
    /// Counts neurons required.
    /// Output modulations of zero will result in "*0" and therefore no effect. Therefore
    /// we don't include that as a neuron.
    /// </summary>
    internal int CountOfOutputNeuronsRequiredBasedOnModulation
    {
        get
        {
            int cnt = 0;

            foreach (float f in OutputModulation)
            {
                if (f != 0) ++cnt;
            }

            return cnt;
        }
    }

    /// <summary>
    /// Creates a map of logical outputs to neurons (or -1 if no neuron required).
    /// </summary>
    internal void SetInternalOutMap()
    {
        int physicalNeuronNumber = 0;

        NeuronMapTypeToOutput.Clear();

        if (OutputModulation[c_steeringOrThrottle2Neuron] != 0) NeuronMapTypeToOutput.Add(c_steeringOrThrottle2Neuron, physicalNeuronNumber++); else NeuronMapTypeToOutput.Add(c_steeringOrThrottle2Neuron, -1);
        if (OutputModulation[c_throttleNeuron] != 0) NeuronMapTypeToOutput.Add(c_throttleNeuron, physicalNeuronNumber++); else NeuronMapTypeToOutput.Add(c_throttleNeuron, -1);
        if (OutputModulation[c_brakeNeuron] != 0) NeuronMapTypeToOutput.Add(c_brakeNeuron, physicalNeuronNumber++); else NeuronMapTypeToOutput.Add(c_brakeNeuron, -1);
        if (OutputModulation[c_eBrakeNeuron] != 0) NeuronMapTypeToOutput.Add(c_eBrakeNeuron, physicalNeuronNumber++); else NeuronMapTypeToOutput.Add(c_eBrakeNeuron, -1);
    }

    /// <summary>
    /// See FieldOfVisionStartInDegrees.
    /// </summary>
    private int _fieldOfVisionStartInDegrees = -45; // degrees

    /// <summary>
    ///     -45  0  45
    ///  -90 _ \ | / _ 90   <-- relative to direction of car, hence + angle car is pointing.
    ///   ^ this
    /// </summary>
    public int FieldOfVisionStartInDegrees
    {
        get { return _fieldOfVisionStartInDegrees; }
        set
        {
            if (value > FieldOfVisionStopInDegrees) FieldOfVisionStopInDegrees = value;

            _fieldOfVisionStartInDegrees = value;
        }
    }

    /// <summary>
    /// See FieldOfVisionStopInDegrees.
    /// </summary>
    private int _fieldOfVisionStopInDegrees = 45; // degrees

    /// <summary>
    ///     -45  0  45
    ///  -90 _ \ | / _ 90   <-- relative to direction of car, hence + angle car is pointing.
    ///                ^ this
    /// </summary>
    public int FieldOfVisionStopInDegrees
    {
        get { return _fieldOfVisionStopInDegrees; }

        set
        {
            if (value < FieldOfVisionStartInDegrees) FieldOfVisionStartInDegrees = value;

            _fieldOfVisionStopInDegrees = value;
        }
    }

    /// <summary>
    /// Do we check for 5 e.g. -90,-45,0,+45,+90, or just -45,0,45? etc.
    /// It will divide the field of view by this amount. 
    //              (3)  
    ///     (2) -45  0  45 (4)
    /// (1)  -90 _ \ | / _ 90  (5)  <-- # sample points = 5.
    /// </summary>
    public int SamplePoints { get; set; } = 7; //7;

    /// <summary>
    /// Account for "additional" sensors.
    /// </summary>
    public int OtherSensorCount { get; set; } = 0;

    /// <summary>
    /// See DepthOfVisionInPixels.
    /// </summary>
    private int _depthOfVisionInPixels = 70; // px

    /// <summary>
    /// ##########
    /// 
    ///    ¦    }
    ///    ¦    } how far the AI looks ahead
    ///    ¦    }
    ///   (o)  car
    /// </summary>
    public int DepthOfVisionInPixels
    {
        get { return _depthOfVisionInPixels; }

        set
        {
            if (value < 1) throw new ArgumentOutOfRangeException(nameof(value));

            _depthOfVisionInPixels = value;
        }
    }

    /// <summary>
    /// Subtracts the 2 angles and divides by sample point.
    /// </summary>
    internal float VisionAngleInDegrees
    {
        get
        {
            return SamplePoints == 1 ? 0 : (float)(FieldOfVisionStopInDegrees - FieldOfVisionStartInDegrees) / (SamplePoints - 1);
        }
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    internal ConfigAI()
    {
        SetInternalOutMap();
    }
}
