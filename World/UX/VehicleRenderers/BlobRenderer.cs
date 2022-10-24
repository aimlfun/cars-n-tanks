using AICarTrack.Settings;
using CarsAndTanks.Settings;
using CarsAndTanks.World;

namespace AICarTrack
{
    /// <summary>
    /// Draws a car as a blob.
    /// </summary>
    internal class BlobRenderer : VehicleRendererBase
    {
        /// <summary>
        /// For a blob, the hit points are a circle. This works out those points
        /// </summary>
        /// <param name="car"></param>
        /// <returns></returns>
        internal override PointF[] RawHitTestPoints(VehicleDrivenByAI car)
        {
            int radius = (Config.s_settings.World.DiameterOfCarInPixels / 2) + (Config.s_settings.World.DiameterOfCarInPixels % 1 == 1 ? 1 : 0); // +1 for odd, to make even

            double intervalInRadians = 2 * Math.PI / (10 + Config.s_settings.World.DiameterOfCarInPixels / 5); // every 10 degrees

            List<PointF> points = new();

            // compute points in a circle around the blob, 2*PI = 360
            for (double angleInRadians = 0; angleInRadians < 2 * Math.PI; angleInRadians += intervalInRadians)
            {
                points.Add(new PointF((int)Math.Round(Math.Cos(angleInRadians) * radius),
                                      (int)Math.Round(Math.Sin(angleInRadians) * radius)));
            }

            return points.ToArray(); // array of dots around the outside of the blob.
        }

        /// <summary>
        /// Draws the car as a blob.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="car"></param>
        internal override void Draw(Graphics g, VehicleDrivenByAI car)
        {
            ConfigDisplay displayConfig = Config.s_settings.Display;
            ConfigWorld worldConfig = Config.s_settings.World;

            // draw car (flash upon hitting gate can be disable thru config)

            Brush brushToPaintCar = (displayConfig.CarsAreRandomColours) ? new SolidBrush(WorldManager.s_colors[car.id]) : displayConfig.BrushCar;

            if (displayConfig.FlashCarAsItGoesOverAGate && car.HasHitGate) brushToPaintCar = displayConfig.BrushCarAtGate;

            // car is drawn as a coloured blob (circle)
            g.FillEllipse(brushToPaintCar,
                          car.CarImplementation.LocationOnTrack.X - worldConfig.DiameterOfCarInPixels / 2,
                          car.CarImplementation.LocationOnTrack.Y - worldConfig.DiameterOfCarInPixels / 2,
                          worldConfig.DiameterOfCarInPixels,
                          worldConfig.DiameterOfCarInPixels);

            if (displayConfig.CarsAreRandomColours && brushToPaintCar != displayConfig.BrushCarAtGate) brushToPaintCar.Dispose(); // we created it for the colors, we should dispose of it
        }
    }
}
