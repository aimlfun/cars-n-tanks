using AICarTrack;
using AICarTrack.Settings;
using AICarTrack.World;
using AICarTrack.World.UX.VehicleRenderers;
using CarsAndTanks.Settings;

namespace UX.MoveablePanels;

/// <summary>
/// Used for the "telemetry" panel. It differs in that this specific one invokes the "telemetry" visualisation.
/// </summary>
internal class MoveableTelemetryPanel : MoveableSemiTransparentPanel
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="pb"></param>
    /// <param name="r"></param>
    internal MoveableTelemetryPanel(Panel pb, Rectangle r) : base(pb, r)
    {
    }

    /// <summary>
    /// Overriden. Used to draw the contents of the panel. (a car or tank with indicators of wheels/tracks).
    /// </summary>
    /// <param name="g"></param>
    internal override void Draw(Graphics g)
    {
        if (!Visible) return;

        if (LearningAndRaceManager.s_currentBestCarId < 0) return;

        // adjust the size before drawing the alpha panel
        PanelSize.Height = Config.s_settings.Display.VehicleShape == ConfigDisplay.VehicleSupportedShapes.Tank ? 320 : 230;

        base.Draw(g);

        VehicleDrivenByAI car = LearningAndRaceManager.s_cars[LearningAndRaceManager.s_currentBestCarId];


        if (Config.s_settings.Display.VehicleShape != ConfigDisplay.VehicleSupportedShapes.Tank
            && Config.s_settings.World.UsingRealWorldPhysics && car.CarImplementation is CarUsingRealPhysics)
        {
            Bitmap b = GoKartTelemetry.ShowGoKartSteeringAndSlide(car, 150, 250);
            g.DrawImage(b, Location.X, Location.Y);
        }
        else
        {
            if (Config.s_settings.Display.VehicleShape == ConfigDisplay.VehicleSupportedShapes.Tank)
            {
                Bitmap b = TankTelemetry.TankTelemetryAsBitmap(car, 150, 300);
                g.DrawImage(b, Location.X, Location.Y);
            }
        }
    }

    /// <summary>
    /// Save the location (invoked on mouse-up).
    /// </summary>
    protected override void Save()
    {
        Config.s_settings.Display.LocationOfMoveableTelemetryPane = Location;
        Config.Save();
    }
}