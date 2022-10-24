#define plain_grass
using AICarTrack;
using AICarTrack.Settings;
using CarsAndTanks.Settings;
using System.Drawing.Drawing2D;
using System.Text;
using UX.MoveablePanels;

namespace CarsAndTanks.World;

/// <summary>
/// Manages the "world" from a visual standpoint.
/// </summary>
internal static class WorldManager
{
    internal static Bitmap s_carBitmap = new("UX/Resources/car.png");

    /// <summary>
    /// List of standard colors, used to color the cars.
    /// </summary>
    internal static readonly List<Color> s_colors = Utils.ColorStructToList();

    /// <summary>
    /// Contains the center of the polygon based track (for vertex gate computation).
    /// </summary>
    internal static PointF s_centerPointOfTrack;

    /// <summary>
    /// 
    /// </summary>
    internal static int s_width = -1;

    /// <summary>
    /// 
    /// </summary>
    internal static int s_height = -1;

    /// <summary>
    /// Loads a static go-kart image, resizes and makes it transparent.
    /// </summary>
    internal static void LoadAndSizeCar()
    {
        if (s_carBitmap != null) s_carBitmap.Dispose();

        string imageName = "car.png";

        if (Config.s_settings.Display.VehicleShape == ConfigDisplay.VehicleSupportedShapes.Tank) imageName = "tank.png";

        s_carBitmap = new($"UX/Resources/{imageName}");
        s_carBitmap.MakeTransparent(s_carBitmap.GetPixel(0, 0));
        s_carBitmap = (Bitmap)ImageUtils.ResizeImage(s_carBitmap, Config.s_settings.World.DiameterOfCarInPixels + 40, Config.s_settings.World.DiameterOfCarInPixels + 40);

        s_width = s_carBitmap.Width;
        s_height = s_carBitmap.Height;
    }

    /// <summary>
    /// Initialises the world.
    /// </summary>
    internal static void Initialise()
    {
        s_centerPointOfTrack = new PointF(Config.s_settings.World.PolygonRadiusOfGeneratedTracksInPixels + Config.s_settings.World.OffsetOfGeneratedTrackFromEdgeInPixels,
                                          Config.s_settings.World.PolygonRadiusOfGeneratedTracksInPixels + Config.s_settings.World.OffsetOfGeneratedTrackFromEdgeInPixels);
        LoadAndSizeCar();
    }

    /// <summary>
    /// Determines whether pixel at (x,y) is grass.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>true - pixel at (x,y) is grass | false - pixel is not grass.</returns>
    internal static bool PixelIsGrass(int x, int y)
    {
        return TrackAndBackgroundCache.GetPixel(x, y) == 255;
    }

    /// <summary>
    /// Determines whether pixel at (x,y) is grass.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>true - pixel at (x,y) is grass | false - pixel is not grass.</returns>
    internal static bool PixelIsGrass(double x, double y)
    {
        return TrackAndBackgroundCache.GetPixel((int)(x + 0.5F), (int)(y + 0.5F)) == 255;
    }

    /// <summary>
    /// Identifies based on pixel color whether it is grass or not.
    /// </summary>
    /// <param name="pixelColor"></param>
    /// <returns>true - color is grass | false - color is not grass.</returns>
    internal static bool IsGrass(Color pixelColor)
    {
        return pixelColor.R == 255;
    }

    /// <summary>
    /// Draw gates with labels
    /// 
    ///   |           |
    ///   o-25--------o 25 
    ///   |           |
    ///
    /// It's useful whilst debugging to be able to see the lines that make up our "gates".
    /// Cars do flash when they travel over the gate, but there is/was plenty of room for
    /// error given the gates require perpendicular lines draw at fixed length along a track segment.
    /// </summary>
    /// <param name="graphics"></param>
    private static void LabelGatesAroundTrack(Graphics graphics)
    {
        for (int gate = 0; gate < LearningAndRaceManager.s_gatesHalf1.Length; gate++)
        {
            // draw a line between the 2 gate points
            PointF pt1 = LearningAndRaceManager.s_gatesHalf1[gate];
            PointF pt2 = LearningAndRaceManager.s_gatesHalf2[gate];

            //   o-25--------o 25 
            //    [---------] line
            graphics.DrawLine(Config.s_settings.Display.PenForDrawingTrackFitnessGates, pt1, pt2);

            //   o-25--------o 25 
            //   ^           ^
            graphics.FillEllipse(new SolidBrush(Color.White), pt1.X - 3, pt1.Y - 3, 5, 5);
            graphics.FillEllipse(new SolidBrush(Color.White), pt2.X - 3, pt2.Y - 3, 5, 5);

            // label both ends of the gate
            string text = gate.ToString();

            //   o-25--------o 25 
            //      ^          ^
            graphics.DrawString(text, Config.s_settings.Display.fontForCarLabel, Config.s_settings.Display.brushGate, pt1.X + 5, pt1.Y);
            graphics.DrawString(text, Config.s_settings.Display.fontForCarLabel, Config.s_settings.Display.brushGate, pt2.X + 5, pt2.Y);
        }
    }

    /// <summary>
    /// Draws the track onto the image, with labels for gates etc. 
    /// </summary>
    internal static void DrawTrack(Graphics graphics, Rectangle clipRect)
    {
        //if (TrackAndBackgroundCache.CachedImageOfTrack is null) return;

        // draw the "track" from an image we have cached for hit-detection etc.
        var currentMode = graphics.CompositingMode;

        graphics.CompositingMode = CompositingMode.SourceCopy;
        //graphics.Clip = new Region(clipRect);

        graphics.DrawImageUnscaled(TrackAndBackgroundCache.GetTrackFromCache(), 0, 0);
        //graphics.ResetClip();

        graphics.CompositingMode = currentMode;

        // show gates if turned on
        if (Config.s_settings.Display.DisplayGates) LabelGatesAroundTrack(graphics);
    }

    /// <summary>
    /// Draws the cars to the graphics (overlaid on top of the track).
    /// </summary>
    internal static Rectangle DrawCarsAndTrack(Graphics g, Rectangle clipRectangle)
    {
        Rectangle? rect = null;

        DrawTrack(g, clipRectangle);

        // no cars? nothing to draw
        if (LearningAndRaceManager.s_cars is null || LearningAndRaceManager.s_cars.Count == 0) return new Rectangle();

        if (Config.s_settings.Display.ShowTelemetry) OverlayTelemetry(g);

        StringBuilder debug = new(20);

        // draw each of the cars
        foreach (int id in LearningAndRaceManager.s_cars.Keys)
        {
            VehicleDrivenByAI car = LearningAndRaceManager.s_cars[id];

            Rectangle r = new((int)(car.CarImplementation.LocationOnTrack.X - Config.s_settings.World.DiameterOfCarInPixels - 7),
                                                       (int)(car.CarImplementation.LocationOnTrack.Y - Config.s_settings.World.DiameterOfCarInPixels + 7),
                                                       2 * (Config.s_settings.World.DiameterOfCarInPixels + 15),
                                                       2 * (Config.s_settings.World.DiameterOfCarInPixels + 15));

            rect ??= r; rect = Rectangle.Union((Rectangle)rect, r);

            car.Renderer.Render(g, car);
            debug.AppendLine($"{id}. gate: {car.CurrentGate.ToString()} {car.EliminatedReason.ToString().Replace("notEliminated", "")}");
        }

        ((MoveableRankingsPanel)FormMain.s_moveablePanels["ranking"]).SetData(debug.ToString());


        return Rectangle.Inflate((Rectangle)rect, 30, 30);
    }

    /// <summary>
    /// Draws the velocity and forces telemetry on top of everything.
    /// </summary>
    /// <param name="g"></param>
    private static void OverlayTelemetry(Graphics g)
    {
        if (LearningAndRaceManager.TelemetryOfBestCarAfterLaps is null || LearningAndRaceManager.TelemetryOfBestCarAfterLaps.Count == 0 || TrackAndBackgroundCache.CachedSillouetteOfTrack is null)
        {
            return;
        }

        List<PointF> lines = new();
        List<double> speed = new();

        var x = LearningAndRaceManager.TelemetryOfBestCarAfterLaps;

        foreach (var item in x)
        {
            lines.Add(item.LocationOnTrack);
            speed.Add(item.AbsoluteVelocityMSInWorldCoords);
        }

        PointF[] linesArray = lines.ToArray();

        // dash showing path
        using Pen p = new(Color.FromArgb(225, 155, 50, 255), 1);
        p.DashStyle = DashStyle.Dash;

        g.DrawLines(p, linesArray);

        // blue = direction car heading
        p.Color = Color.DeepSkyBlue;
        p.EndCap = LineCap.ArrowAnchor;
        p.DashStyle = DashStyle.Dash;
        int z = 0;

        using Pen p2 = new(Color.Red, 1);
        p2.DashStyle = DashStyle.Dot;
        p2.EndCap = LineCap.ArrowAnchor;

        foreach (var item in x)
        {
            if (++z % 15 != 0) continue; // don't do too many blobs close together.

            // direction of velocity
            float rx = (float)Math.Cos((float)item.HeadingOfCarInRadians) * (float)item.AbsoluteVelocityMSInWorldCoords / 2F;
            float ry = (float)Math.Sin((float)item.HeadingOfCarInRadians) * (float)item.AbsoluteVelocityMSInWorldCoords / 2F;

            PointF pt = new(item.LocationOnTrack.X + rx, item.LocationOnTrack.Y + ry);

            g.DrawLine(p, item.LocationOnTrack, pt);

            // perpendicular force
            float rx2 = (float)Math.Cos((float)item.HeadingOfCarInRadians - Math.PI / 2) * (float)item.TotalForceLocalCarCoordsLateralAkaSideways / 300F;
            float ry2 = (float)Math.Sin((float)item.HeadingOfCarInRadians - Math.PI / 2) * (float)item.TotalForceLocalCarCoordsLateralAkaSideways / 300F;

            PointF pt2 = new(item.LocationOnTrack.X + rx2, item.LocationOnTrack.Y + ry2);
            g.DrawLine(p2, item.LocationOnTrack, pt2);
        }
    }

}