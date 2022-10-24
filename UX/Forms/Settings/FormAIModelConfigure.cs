using CarsAndTanks.AI;
using CarsAndTanks.AI.UX;
using CarsAndTanks.Settings;
using CarsAndTanks.Utilities;

namespace CarsAndTanks.UX.Forms.Settings;

public partial class FormAIModelConfigure : Form
{
    /// <summary>
    /// Simplify access to AI config.
    /// </summary>
    private readonly ConfigAI AIconf = Config.s_settings.AI;

    /// <summary>
    /// Neural network object for rending in PictureBox.
    /// </summary>
    private NeuralNetwork n;


    public FormAIModelConfigure()
    {
        InitializeComponent();

        NeuralNetworkVisualiser.Reset(); // this ensures it paints the background

        // a neural network to appear in the visualiser
        n = new NeuralNetwork(-1, AIconf.Layers, false);

        if (Config.s_settings.Display.VehicleShape == ConfigDisplay.VehicleSupportedShapes.Tank)
        {
            labelOutput1.Text = "TrackL";
            labelOutput2.Text = "TrackR";
        }
        else
        {
            labelOutput1.Text = "Steering";
            labelOutput2.Text = "Throttle";
        }

        Config.s_settings.ConfigChangedTheCarsAndNeuralNetworkAreInvalid = false;
    }

    /// <summary>
    /// OnLoad event sets all the UI inputs.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FormAIBrainSettings_Load(object sender, EventArgs e)
    {
        numericUpDownINPUT.Value = AIconf.SamplePoints;

        DisplayNeurons(); // shows the settings visually

        // this populates the neural network "inputs" ignoring input and output node.
        InitialiseNeuralNetworkLayersINPUTs();

        // setup remaining inputs and attach a handler for when it changes

        InitialiseModulationINPUTs();

        numericUpDownPopulation.Value = AIconf.NumberOfAICarsToCreate < numericUpDownPopulation.Minimum ? numericUpDownPopulation.Minimum : AIconf.NumberOfAICarsToCreate;
        numericUpDownPopulation.ValueChanged += NumericUpDownPopulation_ValueChanged;

        numericUpDownMutateAfter.Value = AIconf.CarMovesBeforeFirstMutation;
        numericUpDownMutateAfter.ValueChanged += NumericUpDownMutateAfter_ValueChanged;

        numericUpDownMutationStrength.Value = (decimal)AIconf.MutationStrength;
        numericUpDownMutationStrength.ValueChanged += NumericUpDownMutationStrength_ValueChanged;

        numericUpDownMutationChance.Value = (decimal)AIconf.MutationChance;
        numericUpDownMutationChance.ValueChanged += NumericUpDownMutationChance_ValueChanged;

        numericUpDownThresholdIncrease.Value = AIconf.PercentageIncreaseBetweenMutations;
        numericUpDownThresholdIncrease.ValueChanged += NumericUpDownThresholdIncrease_ValueChanged;
    }

    /// <summary>
    /// Modulation controls reduction or maplification of the neueral network output. This copies
    /// the existing values into the inputs.
    /// </summary>
    private void InitialiseModulationINPUTs()
    {
        numericUpDownSteering.Value = (decimal)AIconf.OutputModulation[ConfigAI.c_steeringOrThrottle2Neuron];
        numericUpDownSteering.ValueChanged += NumericUpDownModulation_ValueChanged;

        numericUpDownThrottle.Value = (decimal)AIconf.OutputModulation[ConfigAI.c_throttleNeuron];
        numericUpDownThrottle.ValueChanged += NumericUpDownModulation_ValueChanged;

        numericUpDownBrake.Value = (decimal)AIconf.OutputModulation[ConfigAI.c_brakeNeuron];
        numericUpDownBrake.ValueChanged += NumericUpDownModulation_ValueChanged;

        numericUpDownEBrake.Value = (decimal)AIconf.OutputModulation[ConfigAI.c_eBrakeNeuron];
        numericUpDownEBrake.ValueChanged += NumericUpDownModulation_ValueChanged;
    }

    /// <summary>
    /// Copy layer numbers for neural network into INPUTs.
    /// </summary>
    private void InitialiseNeuralNetworkLayersINPUTs()
    {
        int numberOfLayersExcludingInputOutput = AIconf.Layers.Length - 2;

        if (numberOfLayersExcludingInputOutput > 0) numericUpDownAINeuronsLayer1.Value = AIconf.Layers[1];
        if (numberOfLayersExcludingInputOutput > 1) numericUpDownAINeuronsLayer2.Value = AIconf.Layers[2];
        if (numberOfLayersExcludingInputOutput > 2) numericUpDownAINeuronsLayer3.Value = AIconf.Layers[3];
        if (numberOfLayersExcludingInputOutput > 3) numericUpDownAINeuronsLayer4.Value = AIconf.Layers[4];
        if (numberOfLayersExcludingInputOutput > 4) numericUpDownAINeuronsLayer5.Value = AIconf.Layers[5];
        if (numberOfLayersExcludingInputOutput > 5) numericUpDownAINeuronsLayer6.Value = AIconf.Layers[6];
        if (numberOfLayersExcludingInputOutput > 6) numericUpDownAINeuronsLayer7.Value = AIconf.Layers[7];

        numericUpDownAINeuronsLayer1.ValueChanged += NumericUpDownAINeuronsLayer_ValueChanged;
        numericUpDownAINeuronsLayer2.ValueChanged += NumericUpDownAINeuronsLayer_ValueChanged;
        numericUpDownAINeuronsLayer3.ValueChanged += NumericUpDownAINeuronsLayer_ValueChanged;
        numericUpDownAINeuronsLayer4.ValueChanged += NumericUpDownAINeuronsLayer_ValueChanged;
        numericUpDownAINeuronsLayer5.ValueChanged += NumericUpDownAINeuronsLayer_ValueChanged;
        numericUpDownAINeuronsLayer6.ValueChanged += NumericUpDownAINeuronsLayer_ValueChanged;
        numericUpDownAINeuronsLayer7.ValueChanged += NumericUpDownAINeuronsLayer_ValueChanged;
    }

    /// <summary>
    /// Determines how many moves before mutation starts.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NumericUpDownMutateAfter_ValueChanged(object? sender, EventArgs e)
    {
        AIconf.CarMovesBeforeFirstMutation = (int)numericUpDownMutateAfter.Value;
    }

    /// <summary>
    /// Updates mutation strength from INPUT.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NumericUpDownMutationStrength_ValueChanged(object? sender, EventArgs e)
    {
        AIconf.MutationStrength = (float)numericUpDownMutationStrength.Value;
    }

    /// <summary>
    /// Updates mutation chance from INPUT.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NumericUpDownMutationChance_ValueChanged(object? sender, EventArgs e)
    {
        AIconf.MutationChance = (int)numericUpDownMutationChance.Value;
    }

    /// <summary>
    /// Updates mutation increase % from INPUT.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NumericUpDownThresholdIncrease_ValueChanged(object? sender, EventArgs e)
    {
        AIconf.PercentageIncreaseBetweenMutations = (int)numericUpDownThresholdIncrease.Value;
    }

    /// <summary>
    /// Updates car # from INPUT.
    /// Number of cars (50-100) ensures random mutations make one that helps.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NumericUpDownPopulation_ValueChanged(object? sender, EventArgs e)
    {
        Config.s_settings.ConfigChangedTheCarsAndNeuralNetworkAreInvalid = true;

        AIconf.NumberOfAICarsToCreate = (int)numericUpDownPopulation.Value;
    }

    /// <summary>
    /// Update the modulation based on inputs.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NumericUpDownModulation_ValueChanged(object? sender, EventArgs e)
    {
        AIconf.OutputModulation[ConfigAI.c_steeringOrThrottle2Neuron] = (float)numericUpDownSteering.Value;
        AIconf.OutputModulation[ConfigAI.c_throttleNeuron] = (float)numericUpDownThrottle.Value;
        AIconf.OutputModulation[ConfigAI.c_brakeNeuron] = (float)numericUpDownBrake.Value;
        AIconf.OutputModulation[ConfigAI.c_eBrakeNeuron] = (float)numericUpDownEBrake.Value;

        AIconf.SetInternalOutMap();

        Config.s_settings.ConfigChangedTheCarsAndNeuralNetworkAreInvalid = true;

        NumericUpDownAINeuronsLayer_ValueChanged(sender, e); // forces repaint (as number of outputs may have changed).
    }

    /// <summary>
    /// Neuron layer counts are copied into an int array.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NumericUpDownAINeuronsLayer_ValueChanged(object? sender, EventArgs e)
    {
        List<int> layers = new()
        {
            AIconf.Layers[0]
        };

        if (numericUpDownAINeuronsLayer1.Value > 0) layers.Add((int)numericUpDownAINeuronsLayer1.Value);
        if (numericUpDownAINeuronsLayer2.Value > 0) layers.Add((int)numericUpDownAINeuronsLayer2.Value);
        if (numericUpDownAINeuronsLayer3.Value > 0) layers.Add((int)numericUpDownAINeuronsLayer3.Value);
        if (numericUpDownAINeuronsLayer4.Value > 0) layers.Add((int)numericUpDownAINeuronsLayer4.Value);
        if (numericUpDownAINeuronsLayer5.Value > 0) layers.Add((int)numericUpDownAINeuronsLayer5.Value);
        if (numericUpDownAINeuronsLayer6.Value > 0) layers.Add((int)numericUpDownAINeuronsLayer6.Value);
        if (numericUpDownAINeuronsLayer7.Value > 0) layers.Add((int)numericUpDownAINeuronsLayer7.Value);

        layers.Add(AIconf.CountOfOutputNeuronsRequiredBasedOnModulation);

        AIconf.Layers = layers.ToArray();
        n = new NeuralNetwork(n.Id - 1, AIconf.Layers, false);

        Config.s_settings.ConfigChangedTheCarsAndNeuralNetworkAreInvalid = true;
        DisplayNeurons();
    }

    /// <summary>
    /// Update the UI based on the settings.
    /// </summary>
    private void DisplayNeurons()
    {
        // sized big enough for circle map + neural network (that varies in width based on the largest neuron layer)
        Bitmap visualImage = new(90 + Math.Max(pictureBoxWorldRepresentation.Width, 100 + NeuralNetworkVisualiser.MaxLayerWidth(n) * (NeuralNetworkVisualiser.s_maxDiameter + 2) + NeuralNetworkVisualiser.s_maxDiameter + 10),
                                      Math.Max(pictureBoxWorldRepresentation.Height, 180 + n.Layers.Length * (NeuralNetworkVisualiser.s_maxDiameter + 2)));

        using Graphics graphicsOfVisualImage = Graphics.FromImage(visualImage);

        // this dashboard image is ruined if the graphics are blocky, so we set high quality at expense of performance
        graphicsOfVisualImage.ToHighQuality();

        NeuralNetworkVisualiser.DrawNeuralFiringAsColouredBlobs(n, visualImage, graphicsOfVisualImage, -170);

        if (pictureBoxWorldRepresentation.Image is not null) pictureBoxWorldRepresentation.Image.Dispose();

        pictureBoxWorldRepresentation.Image = visualImage;
    }

    /// <summary>
    /// [Save] settings clicked.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ButtonSave_Click(object sender, EventArgs e)
    {
        Config.Save();
    }
}