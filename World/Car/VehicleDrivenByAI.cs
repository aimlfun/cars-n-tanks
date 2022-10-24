using CarsAndTanks.Learn;
using CarsAndTanks.AI;
using CarsAndTanks.Settings;
using CarsAndTanks.World.Telemetry;
using CarsAndTanks.World.UX.VehicleRenderers;

namespace CarsAndTanks.World.Car;

/// <summary>
/// Magic AI driven vehicle.
/// </summary>
internal class VehicleDrivenByAI
{
    /// <summary>
    /// Reasons the car may have been eliminated (or not).
    /// </summary>
    internal enum EliminationReasons { notEliminated, collided, backwardsGate, gateMutate, moves100, userPunish };

    /// <summary>
    /// The telemetry data for the current lap.
    /// </summary>
    internal List<TelemetryData> TelemetryData = new();

    /// <summary>
    /// Last set of inputs from the neural network.
    /// </summary>
    internal VehicleInputState carInputState = new();

    /// <summary>
    /// If set to true, there is no AI / learning
    /// </summary>
    internal static bool c_hardCodedBrain = false;

    /// <summary>
    /// The unique ID of this car in the Cars[] dictionary, used to map to AI brain (has same ID).
    /// </summary>
    internal int id = 0;

    /// <summary>
    /// True of this car has lapped the track twice.
    /// </summary>
    internal bool HasLappedTwice = false;

    /// <summary>
    /// Lap the car is currently on.
    /// </summary>
    internal int lap = 0;

    /// <summary>
    /// Represents the car model which can vary (from basic physics to full physics).
    /// </summary>
    internal readonly Vehicle CarImplementation = new CarUsingBasicPhysics();

    #region GATES
    /// <summary>
    /// Each time it passes a gate this flag is briefly set. We do so for visual confirmation (flash) so it is possible to tell
    /// that the gate-detect is working (aside from telemetry)
    /// </summary>
    internal bool HasHitGate = false;

    /// <summary>
    /// Checkpoint (gate) number on the course.
    /// </summary>
    internal int CurrentGate = 0;

    /// <summary>
    /// Last gate car was at. -1 = before any gates.
    /// </summary>
    internal int LastGatePassed = 0;
    #endregion

    #region CAR ELIMINATION

    /// <summary>
    /// See HasBeenEliminated.
    /// </summary>
    private bool hasBeenEliminated = false;

    /// <summary>
    /// Why the car was eliminated (changes color etc).
    /// </summary>
    private EliminationReasons reasonCarWasEliminated = EliminationReasons.notEliminated;

    /// <summary>
    /// Returns the reason why the car was eliminated.
    /// </summary>
    internal EliminationReasons EliminatedReason
    {
        get { return reasonCarWasEliminated; }
    }

    /// <summary>
    /// When true, this car will no longer move. 
    /// Reasons are (1) the car has crashed into the grass 
    /// (2) the car decided to misbehave (go backwards).
    /// </summary>
    internal bool HasBeenEliminated
    {
        get { return hasBeenEliminated; }
    }

    /// <summary>
    /// > 0 we see a "ghost" of the car briefly where it got removed.
    /// </summary>
    internal int EliminatedFadeCount = 0;

    /// <summary>
    /// The start heading for the car, based on the track painted.
    /// </summary>
    internal static int Heading;

    /// <summary>
    /// Marks a car as eliminated (doesn't move, will get replaced by a better performing car during next mutate).
    /// </summary>
    /// <param name="reason"></param>
    internal void Eliminate(EliminationReasons reason)
    {
        if (hasBeenEliminated) return; // no action necessary.

        hasBeenEliminated = true;

        EliminatedFadeCount = Config.s_settings.Display.HowLongToShowGhostOfEliminatedCars; // timer, so we see a "ghost" of the car briefly where it got removed

        reasonCarWasEliminated = reason; // track the reason, so we can trouble-shoot 
    }
    #endregion

    /// <summary>
    /// Determines how to paint and hit-detect.
    /// </summary>
    internal VehicleRendererBase Renderer;

    /// <summary>
    /// true - car has lapped at least once.
    /// </summary>
    internal bool HasLapped;

    /// <summary>
    /// Constructor.
    /// </summary>
    internal VehicleDrivenByAI(int newId)
    {
        Renderer = Config.s_settings.Display.VehicleShape switch
        {
            ConfigDisplay.VehicleSupportedShapes.Blob => new BlobRenderer(),
            ConfigDisplay.VehicleSupportedShapes.GoKart => new GokartRenderer(),
            ConfigDisplay.VehicleSupportedShapes.Tank => new TankRenderer(),
            _ => throw new ArgumentOutOfRangeException(nameof(Config.s_settings.Display.VehicleShape)),
        };

        // we support basic and real physics. Basic physics was the 1.0, get AI working, real physics increases the challenges.
        CarImplementation = Config.s_settings.World.UsingRealWorldPhysics ? new CarUsingRealPhysics() : new CarUsingBasicPhysics();

        // this is the data used to link up the neural network / monitor / manipulate
        id = newId;

        // reset the car to the start point for this track and point it in the correct direction (intended flow of track).
        CarImplementation.LocationOnTrack = new PointF(LearningAndRaceManager.s_startPoint.X, LearningAndRaceManager.s_startPoint.Y);
        CarImplementation.AngleVehicleIsPointingInDegrees = Heading;
    }

    /// <summary>
    /// Merges inputs [0..n] LIDAR + other sensors into one array to feed into the neural network.
    /// </summary>
    /// <param name="LIDAR"></param>
    /// <param name="otherSensors"></param>
    /// <returns></returns>
    private static double[] CombineNNInputs(double[] LIDAR, double[] otherSensors)
    {
        List<double> inputs = new();

        inputs.AddRange(LIDAR);
        inputs.AddRange(otherSensors);

        return inputs.ToArray();
    }

    /// <summary>
    /// Called at a constant interval to simulate car moving.
    /// </summary>
    internal void Move(out bool lapped)
    {
        lapped = false;

        // if the car has collided with the grass it is stuck, we don't move it or call its NN.
        if (HasBeenEliminated) return;

        ApplyAIoutputAndMoveTheCar();

        // track what the car was doing (intent and position/forces/acceleration etc)
        if (CarImplementation.LapTelemetryData is not null) TelemetryData.Add(CarImplementation.LapTelemetryData);

        // having moved, did the AI muck it up and drive onto the grass? If so, that's this car out of action.
        if (Renderer.Collided(this))
        {
            Eliminate(EliminationReasons.collided); // stop car as it hit the grass
            return;
        }

        if (c_hardCodedBrain) return; // gates are irrelevant, it's not learning

        // the car hasn't crashed, so let's see if it reached a gate....

        HasHitGate = false;

        // When it crosses a gate, we update the gate for the car
        // From this "progress" indicator, we judge the fitness/performance of the network for that car.
        if (LearningAndRaceManager.CarIsAtGate(this, out int gate, out lapped))
        {
            CurrentGate = gate;
            HasHitGate = true;

            // track for cars that regress (backwards thru gates) so we can eliminate them. This is done by pretending it crashed.
            if (LastGatePassed > CurrentGate || CurrentGate - 30 > LastGatePassed)
            {
                CurrentGate = -2; // dump it at the bottom for mutation, as punishment for going the wrong way
                Eliminate(EliminationReasons.backwardsGate);
            }
            else
                LastGatePassed = CurrentGate;
        }
    }

    /// <summary>
    /// Read the sensors, provide to the "brains" (neural network) and take action based on
    /// the output.
    /// </summary>
    private void ApplyAIoutputAndMoveTheCar()
    {
        var AIconfig = Config.s_settings.AI;

        double[] visionSensor = Config.s_settings.AI.VisionSystem.VisionSensorOutput(id, CarImplementation.AngleVehicleIsPointingInDegrees, CarImplementation.LocationOnTrack); /// input is the distance sensors of how soon we'll impact

        // rather than have a rigid fixed inputs, we've built this to be expandable.
        // The NN doesn't care how many inputs we have.
        double[] outputFromNeuralNetwork;

        if (!c_hardCodedBrain)
        {
            double[] otherSensors = Array.Empty<double>(); // Speed/100, AngleCarIsPointingInDegrees/360 // TBD, current speed, current direction, etc

            double[] neuralNetworkInput = CombineNNInputs(visionSensor, otherSensors);

            // ask the neural to use the input and decide what to do with the car
            outputFromNeuralNetwork = NeuralNetwork.s_networks[id].FeedForward(neuralNetworkInput); // process inputs

            // e.g. 
            // outputFromNeuralNetwork[0] -> how much to rotate the car      -1..+1 from TANH
            // outputFromNeuralNetwork[1] -> the relative speed of the car   -1..+1 from TANH. To minimise backwards travel, we add 0.9 and divide by 2 (1/10th speed backwards).
        }
        else
        {
            outputFromNeuralNetwork = new double[2];

            // visionSensor[0] = lidar 45 degrees top-right, distance

            /* steering */
            outputFromNeuralNetwork[0] = Math.Tanh(0.20730000291951 * Math.Tanh(0.008449995890259743 * visionSensor[0] - 0.4309000142675359) +
                                                     -0.5545999999158084 * Math.Tanh(-0.6085499841719866 * visionSensor[0] + +0.10859999456442893)
                                                     - 0.026499994099140167);

            /* speed */
            outputFromNeuralNetwork[1] = Math.Tanh(0.11979999393224716 * Math.Tanh(0.008449995890259743 * visionSensor[0] - 0.4309000142675359) +
                                                    0.01675000146497041 * Math.Tanh(-0.6085499841719866 * visionSensor[0] + 0.10859999456442893)
                                                    + 0.05970000018351129);
        }

        carInputState = new(
                AIconfig.NeuronMapTypeToOutput[ConfigAI.c_steeringOrThrottle2Neuron] != -1 ? outputFromNeuralNetwork[AIconfig.NeuronMapTypeToOutput[ConfigAI.c_steeringOrThrottle2Neuron]] * AIconfig.OutputModulation[ConfigAI.c_steeringOrThrottle2Neuron] : 0,
                AIconfig.NeuronMapTypeToOutput[ConfigAI.c_throttleNeuron] != -1 ? outputFromNeuralNetwork[AIconfig.NeuronMapTypeToOutput[ConfigAI.c_throttleNeuron]] * AIconfig.OutputModulation[ConfigAI.c_throttleNeuron] : 0.8F,
                AIconfig.NeuronMapTypeToOutput[ConfigAI.c_brakeNeuron] != -1 ? outputFromNeuralNetwork[AIconfig.NeuronMapTypeToOutput[ConfigAI.c_brakeNeuron]] * AIconfig.OutputModulation[ConfigAI.c_brakeNeuron] : 0,
                AIconfig.NeuronMapTypeToOutput[ConfigAI.c_eBrakeNeuron] != -1 ? outputFromNeuralNetwork[AIconfig.NeuronMapTypeToOutput[ConfigAI.c_eBrakeNeuron]] * AIconfig.OutputModulation[ConfigAI.c_eBrakeNeuron] : 0,
                AIconfig.NeuronMapTypeToOutput[ConfigAI.c_steeringOrThrottle2Neuron] != -1 ? outputFromNeuralNetwork[AIconfig.NeuronMapTypeToOutput[ConfigAI.c_steeringOrThrottle2Neuron]] * AIconfig.OutputModulation[ConfigAI.c_steeringOrThrottle2Neuron] : 0
        );

        CarImplementation.ApplyPhysics(carInputState);
    }

    /// <summary>
    /// Updates fitness of network for sorting. The higher the gate it passed, the better the NN achieved.
    /// </summary>
    internal void UpdateFitness()
    {
        // The more gates it goes through the better car. However we want to encourage the minimum moves because that equates to quicker.
        NeuralNetwork.s_networks[id].Fitness = CurrentGate * 10000 - LearningAndRaceManager.NumberOfMovesMadeByCars; // the further around, the more gates i.e. better AI
    }
   }