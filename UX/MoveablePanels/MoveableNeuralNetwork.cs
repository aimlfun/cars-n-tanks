using CarsAndTanks.Learn;
using CarsAndTanks.AI;
using CarsAndTanks.AI.UX;
using CarsAndTanks.Settings;
using CarsAndTanks.World.Car;

namespace CarsAndTanks.UX.MoveablePanels;

/// <summary>
/// Used for the "neural network" panel. It differs in that this specific one shows the neurons firing.
/// </summary>
internal class MoveableNeuralNetwork : MoveableSemiTransparentPanel
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="pb"></param>
    /// <param name="r"></param>
    internal MoveableNeuralNetwork(Panel pb, Rectangle r) : base(pb, r)
    {
    }

    /// <summary>
    /// Overriden. Used to draw the contents of the panel. (Animated neurons).
    /// </summary>
    /// <param name="g"></param>
    internal override void Draw(Graphics g)
    {
        if (!Visible) return;

        if (LearningAndRaceManager.s_currentBestCarId < 0) return;

        VehicleDrivenByAI car = LearningAndRaceManager.s_cars[LearningAndRaceManager.s_currentBestCarId];
        NeuralNetwork n = NeuralNetwork.s_networks[car.id];

        PanelSize.Width = 20 + NeuralNetworkVisualiser.MaxLayerWidth(n) * (NeuralNetworkVisualiser.s_maxDiameter + 2) + NeuralNetworkVisualiser.s_maxDiameter + 10;
        PanelSize.Height = n.Layers.Length * (NeuralNetworkVisualiser.s_maxDiameter + 2) + 80;

        base.Draw(g);

        Bitmap b = NeuralNetworkVisualiser.Render(NeuralNetwork.s_networks[car.id]);
        g.DrawImage(b, Location.X, Location.Y);
    }

    /// <summary>
    /// Save the location (invoked on mouse-up).
    /// </summary>
    protected override void Save()
    {
        Config.s_settings.Display.LocationOfMoveableNeuralNetworkPane = Location;
        Config.Save();
    }

}
