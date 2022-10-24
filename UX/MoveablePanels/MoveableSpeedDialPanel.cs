using AICarTrack;
using AICarTrack.World.UX.Dials;
using CarsAndTanks.Settings;

namespace UX.MoveablePanels;

/// <summary>
/// Used for the "speedo" panel. It differs in that this specific one shows a digital and analogue speedo.
/// </summary>
internal class MoveableSpeedDialPanel : MoveableSemiTransparentPanel
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="pb"></param>
    /// <param name="r"></param>
    internal MoveableSpeedDialPanel(Panel pb, Rectangle r) : base(pb, r)
    {
    }

    /// <summary>
    /// Overriden. Used to draw the contents of the panel. (draws a speedo).
    /// </summary>
    /// <param name="g"></param>
    internal override void Draw(Graphics g)
    {
        if (!Visible) return;

        if (LearningAndRaceManager.s_currentBestCarId < 0) return;

        base.Draw(g);

        VehicleDrivenByAI car = LearningAndRaceManager.s_cars[LearningAndRaceManager.s_currentBestCarId];

        Bitmap b = DashboardSpeedometer.DrawNeedle(new Point(260 / 2, 260 / 2), 95,
                              Config.s_settings.World.UsingRealWorldPhysics ? car.CarImplementation.Speed : car.CarImplementation.Speed * 40);

        g.DrawImage(b, Location.X, Location.Y);
    }

    /// <summary>
    /// Save the location (invoked on mouse-up).
    /// </summary>
    protected override void Save()
    {
        Config.s_settings.Display.LocationOfMoveableSpeedPane = Location;
        Config.Save();
    }

}
