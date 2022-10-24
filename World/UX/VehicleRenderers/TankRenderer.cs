using CarsAndTanks.Utilities;
using CarsAndTanks.World;
using CarsAndTanks.World.Car;

namespace CarsAndTanks.World.UX.VehicleRenderers;

/// <summary>
/// Implementation of car as a tank.
/// </summary>
internal class TankRenderer : VehicleRendererBase
{
    internal override PointF[] RawHitTestPoints(VehicleDrivenByAI car)
    {
        float width = WorldManager.s_width - 15 * WorldManager.s_width / 42;
        float height = WorldManager.s_height - 5 * WorldManager.s_height / 42;

        /*              X   X   X
         *             p3  p13   p1
         *   p5 +---------------+ X 
         *      |               |
         *      |        +      | X p12
         *      |               |
         *   p6 +---------------+ X
         *             p4  p24  p2
         *              X   X   X
         */

        PointF p1 = new(height / 2 - height / 24 - 2 - 14 + 2, width / 2 - width / 9 - 15 + 3);
        PointF p3 = new(0, p1.Y);
        PointF p13 = new((p1.X + p3.X) / 2, p1.Y);

        PointF p2 = new(height / 2 - height / 24 - 2 - 14 + 2, -width / 2 + width / 9 + 11 - 3);
        PointF p4 = new(0, p2.Y);
        PointF p24 = new((p2.X + p4.X) / 2, p4.Y);

        PointF p12 = new(p1.X + 1, (p1.Y + p2.Y) / 2);

        PointF p5 = new(-height / 2 + height / 24 + 15 - 2, width / 2 - 5 - 16 + 3);
        PointF p6 = new(-height / 2 + height / 24 + 15 - 2, -width / 2 + 5 - 2 + 14 - 3);

        return new PointF[] { p1, p12, p2, p24, p4, p6, p5, p3, p13 };
    }

    /// <summary>
    /// Draws the Go Kart.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="car"></param>
    internal override void Draw(Graphics g, VehicleDrivenByAI car)
    {
        Bitmap b = ImageUtils.RotateBitmapWithColoredBackground(WorldManager.s_carBitmap, car.CarImplementation.AngleVehicleIsPointingInDegrees);

        g.DrawImage(b,
                new Point((int)car.CarImplementation.LocationOnTrack.X - b.Width / 2,
                          (int)car.CarImplementation.LocationOnTrack.Y - b.Height / 2));
    }
}
