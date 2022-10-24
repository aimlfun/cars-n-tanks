using CarsAndTanks.Settings;
using CarsAndTanks.Utilities;
using CarsAndTanks.World;

namespace CarsAndTanks.Vision;

/// <summary>
/// Simple vision implementation of a LIDAR.
/// </summary>
internal class MonoLIDAR : IVision
{
    /// <summary>
    /// Parallel.ForEach causes issues as we
    /// </summary>
    private readonly object parallelForEachLock = new();

    /// <summary>
    /// LIDAR lines are drawn with this
    /// </summary>
    private readonly static Pen s_penToDrawLidar = new(Color.FromArgb(200, 255, 0, 0));

    /// <summary>
    /// Used to remember where to draw the LIDAR.
    /// </summary>
    private PointF locationOfOutput = new();

    /// <summary>
    /// Used to draw the LIDAR, contains the values given to the neural network. Correspond to "distances" from colliding (1=close).
    /// </summary>
    readonly Dictionary<int, double[]> lastOutputFromSensor = new();

    /// <summary>
    /// The AI car needs to know how far it is from the grass, after all we're training it to stay on the
    /// track.
    /// 
    /// We do this as a LIDAR approach minus the fancy/expensive laser. 
    /// 
    /// We check pixels in the config defined directions.
    /// 
    /// Typically that would be forwards, diagonally forward to inform it as it turns, and to the sides so it knows how
    /// close it is to the grass.
    /// 
    /// The car could be moving in any direction, we have to compute LIDAR hits with the forward line pointing
    /// where the car is going.
    /// 
    ///     \  |  /
    ///      \ | /        __       __
    ///  _____\|/_____     |\  /|\  /|
    ///   +--------+         \  |  /        .¦
    ///   |   AI   |          ) | (      .:¦¦¦
    ///   +--------+          angle      speed
    /// </summary>
    /// <returns>Array of proximities (each 0..1F).</returns>
    /// <param name="AngleLookingInDegrees"></param>
    /// <param name="location"></param>
    /// <returns>Proximities 0..1 of what it sees</returns>
    /// <exception cref="NotImplementedException"></exception>
    public double[] VisionSensorOutput(int id, double AngleLookingInDegrees, PointF location)
    {
        ConfigAI AIconfig = Config.s_settings.AI;

        // e.g 
        // input to the neural network
        //   _ \ | / _   
        //   0 1 2 3 4 
        //        
        double[] LIDAROutput = new double[AIconfig.SamplePoints];

        //   _ \ | / _   
        //   0 1 2 3 4
        //   ^ this
        float LIDARAngleToCheckInDegrees = AIconfig.FieldOfVisionStartInDegrees;

        //   _ \ | / _   
        //   0 1 2 3 4
        //   [-] this
        float LIDARVisionAngleInDegrees = AIconfig.VisionAngleInDegrees;

        int radiusOfCarInPX = Config.s_settings.Display.VehicleShape == ConfigDisplay.VehicleSupportedShapes.Blob ? Config.s_settings.World.DiameterOfCarInPixels / 2 : (WorldManager.s_width - 20 * WorldManager.s_width / 42) / 2; // we process in radius, not diameter, so calc once here

        int searchDistanceInPixels = AIconfig.DepthOfVisionInPixels + radiusOfCarInPX;

        for (int LIDARangleIndex = 0; LIDARangleIndex < AIconfig.SamplePoints; LIDARangleIndex++)
        {
            //     -45  0  45
            //  -90 _ \ | / _ 90   <-- relative to direction of car, hence + angle car is pointing
            double LIDARangleToCheckInRadians = MathUtils.DegreesInRadians(AngleLookingInDegrees + LIDARAngleToCheckInDegrees);

            // calculate ONCE per angle, not per radius.
            double cos = Math.Cos(LIDARangleToCheckInRadians);
            double sin = Math.Sin(LIDARangleToCheckInRadians);

            float howCloseToGrassCarIsForThisAngle = 0;

            // We don't want the car driving on the grass, so there is little advantage of checking pixels very close to
            // the car. To avoid accidents, we need to check radiating outwards from the vehicle and find the first square
            // of grass in that direction. i.e. we don't care if there is grass 30 pixels away if there is grass right next to the car.

            // Given the car is a blob of 5px radius, the first 5 pixels are the car itself. We'd need to start
            // checking at least 5 pixels away.
            //         .5.        px
            //  |..15..(o)..15..|

            // Based on config, we can look ahead. But be mindful, every pixel we check takes time.
            for (int currentLIDARscanningDistanceRadius = radiusOfCarInPX;
                     currentLIDARscanningDistanceRadius < searchDistanceInPixels;
                     currentLIDARscanningDistanceRadius += 2) // no need to check at 1 pixel resolution
            {
                // simple maths, think circle. We are picking a point at an arbitrary angle at an arbitrary distance from a center point.
                // r = LIDARscanningDistance, angle = LIDARangleToCheckInDegrees

                // we need to convert that into a relative horizontal / vertical position, then add that to the cars location
                // X = r cos angle | y = r sin angle
                double positionOnTrackBeingScannedX = Math.Round(cos * currentLIDARscanningDistanceRadius);
                double positionOnTrackBeingScannedY = Math.Round(sin * currentLIDARscanningDistanceRadius);

                // do we see grass on that pixel?
                if (WorldManager.PixelIsGrass(location.X + positionOnTrackBeingScannedX,
                                              location.Y + positionOnTrackBeingScannedY))
                {
                    howCloseToGrassCarIsForThisAngle = currentLIDARscanningDistanceRadius;
                    break; // we've found the closest pixel in this direction
                }
            }

            // at this point we have proximity of grass in a single direction

            // >0 means there is grass within the LIDAR radius
            if (howCloseToGrassCarIsForThisAngle > 0)
            {
                howCloseToGrassCarIsForThisAngle -= radiusOfCarInPX;

                // the range is 20..30, so we subtract 20 to bring it in the 0-10 range.
                howCloseToGrassCarIsForThisAngle /= AIconfig.DepthOfVisionInPixels;
                howCloseToGrassCarIsForThisAngle.Clamp(0, 1);

                // the neural network cares about 0..1 for inputs so we scale but
                // but we also need to invert so that "1" needs to mean REALLY close (neuron fires), "0" means no grass
                LIDAROutput[LIDARangleIndex] = 1 - howCloseToGrassCarIsForThisAngle;
            }
            else
            {
                LIDAROutput[LIDARangleIndex] = 0; // no grass within this direction
            }

            //   _ \ | / _         _ \ | / _   
            //   0 1 2 3 4         0 1 2 3 4
            //  [-] from this       [-] to this
            LIDARAngleToCheckInDegrees += LIDARVisionAngleInDegrees;
        }

        // store the location for drawing later
        locationOfOutput.X = location.X;
        locationOfOutput.Y = location.Y;

        SetLIDAR(id, LIDAROutput);

        // an array of float values 0..1 indicating "1" grass is really close in that direction to "0" no grass.
        return LIDAROutput;
    }

    /// <summary>
    /// Draws the LIDAR onto the image provided. Unlike the live LIDAR, we are drawing with the car facing upwards.
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="visualImage"></param>
    public void DrawSensorToImage(int id, Graphics graphics, Bitmap visualImage, PointF visualCenterPoint)
    {
        if (lastOutputFromSensor is null) throw new Exception("this should come from the LIDAR");

        ConfigAI AIconfig = Config.s_settings.AI;

        //      .  
        //       \ | .    
        //      __(o).     "(o)" is the car. lines vary in length depending on proximity to wall 

        float LIDARAngleToCheckInDegrees = AIconfig.FieldOfVisionStartInDegrees;
        float LIDARVisionAngleInDegrees = AIconfig.VisionAngleInDegrees;

        s_penToDrawLidar.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;

        int radiusOfCarInPX = Config.s_settings.Display.VehicleShape == ConfigDisplay.VehicleSupportedShapes.Blob
                ? Config.s_settings.World.DiameterOfCarInPixels / 2
                : (WorldManager.s_carBitmap.Width - 20 * WorldManager.s_carBitmap.Width / 42) / 2; // we process in radius, not diameter, so calc once here

        for (int LIDARangleIndex = 0; LIDARangleIndex < AIconfig.SamplePoints; LIDARangleIndex++)
        {
            double inputInverted = 1 - lastOutputFromSensor[id][LIDARangleIndex];

            //     -45  0  45
            //  -90 _ \ | / _ 90   <-- relative to direction of car, hence + angle car is pointing
            double LIDARangleToCheckInRadians = MathUtils.DegreesInRadians(LIDARAngleToCheckInDegrees) - /* 90 degrees */ Math.PI / 2;

            // size of line to draw
            double sizeOfLine = inputInverted * AIconfig.DepthOfVisionInPixels + radiusOfCarInPX;

            // we need to convert that into a relative horizontal / vertical position, then add that to the cars location
            // X = r cos angle | y = r sin angle
            float x = (float)Math.Round(Math.Cos(LIDARangleToCheckInRadians) * sizeOfLine) + visualCenterPoint.X;
            float y = (float)Math.Round(Math.Sin(LIDARangleToCheckInRadians) * sizeOfLine) + visualCenterPoint.Y;

            graphics.DrawLine(s_penToDrawLidar, visualCenterPoint, new PointF(x, y));

            LIDARAngleToCheckInDegrees += LIDARVisionAngleInDegrees;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="sensor"></param>
    public void SetLIDAR(int id, double[] sensor)
    {
        lock (parallelForEachLock)
        {
            if (!lastOutputFromSensor.ContainsKey(id)) lastOutputFromSensor.Add(id, sensor); else lastOutputFromSensor[id] = sensor;
        }
    }
}