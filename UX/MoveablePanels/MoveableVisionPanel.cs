using AICarTrack;
using AICarTrack.Vision.UX;
using CarsAndTanks.Settings;

namespace UX.MoveablePanels;

/// <summary>
/// Used for the "vision" panel. It differs in that this specific one invokes the vision "visualiser".
/// </summary>
internal class MoveableVisionPanel : MoveableSemiTransparentPanel
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="pb"></param>
    /// <param name="r"></param>
    internal MoveableVisionPanel(Panel pb, Rectangle r) : base(pb, r)
    {
    }

    /// <summary>
    /// Overriden. Used to draw the contents of the panel. (What the vision lidar sees).
    /// </summary>
    /// <param name="g"></param>
    internal override void Draw(Graphics g)
    {
        if (!Visible) return;

        if (LearningAndRaceManager.s_currentBestCarId < 0) return;

        base.Draw(g);

        VehicleDrivenByAI car = LearningAndRaceManager.s_cars[LearningAndRaceManager.s_currentBestCarId];

        Bitmap b = VisionVisualiser.Render(NeuralNetwork.s_networks[car.id]);
        g.DrawImage(b, Location.X, Location.Y);
    }

    /// <summary>
    /// Save the location (invoked on mouse-up).
    /// </summary>
    protected override void Save()
    {
        Config.s_settings.Display.LocationOfMoveableVisionPane = Location;
        Config.Save();
    }
}
