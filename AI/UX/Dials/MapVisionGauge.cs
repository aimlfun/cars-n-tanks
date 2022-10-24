using CarsAndTanks.Learn;
using CarsAndTanks.AI;
using CarsAndTanks.Settings;
using CarsAndTanks.Utilities;
using CarsAndTanks.World.Car;
using CarsAndTanks.World.UX.Track;

namespace CarsAndTanks.AI.UX.Dials
{
    internal static class MapVisionGauge
    {
        /// <summary>
        /// We draw a car outline in black. 
        /// </summary>
        private static readonly Pen s_penForCarCircle = new(Color.GreenYellow);

        /// <summary>
        /// Draw a representation of the car, with sensor with world rotated around it.
        /// 
        /// Grab a square centered on the car.
        /// Cut out a circle area (losing outside of circle)
        /// Rotate the area so car is facing upwards.
        /// Superimpose sensor on top.
        /// </summary>
        /// <param name="network"></param>
        /// <param name="aiConfig"></param>
        /// <param name="visualImage"></param>
        /// <param name="graphicsOfVisualImage"></param>
        /// <param name="outputY"></param>
        internal static void DrawBlobWithLIDARAndRotatedTrack(NeuralNetwork network, Bitmap visualImage, Graphics graphicsOfVisualImage)
        {
            // this is the "radius" of the square around the car
            int radius = 90;

            var car = LearningAndRaceManager.s_cars[network.Id];

            // grab a square "size" pixels to the left,top,right,bottom from the scenery
            Image track = TrackAndBackgroundCache.CopySquareFromTrackImageAt((int)(car.CarImplementation.LocationOnTrack.X + 0.5F), (int)(car.CarImplementation.LocationOnTrack.Y + 0.5F), radius);

            // add a semi-transparent overlay of where other cars are
            track = OverlaysCarsOnLIDAR(track, network.Id, (int)(car.CarImplementation.LocationOnTrack.X + 0.5F), (int)(car.CarImplementation.LocationOnTrack.Y + 0.5F));

            // turn the square into a circle (black out the part outside of the circle)
            track = ImageUtils.CropToCircle(track, Color.Transparent);

            // rotate track as it would be if car was facing upwards
            track = ImageUtils.RotateBitmap(track, 270 - car.CarImplementation.AngleVehicleIsPointingInDegrees); // 270 is to rotate from horizontal to vertical

            graphicsOfVisualImage.DrawImageUnscaled(track, new Point(visualImage.Width / 2 - radius, visualImage.Height / 2 - radius));

            // draw the car
            int posX = visualImage.Width / 2; // size + 10;
            int posY = visualImage.Height / 2; // 20 + size;

            // vision system is responsible for drawing what it saw.
            Config.s_settings.AI.VisionSystem.DrawSensorToImage(network.Id, graphicsOfVisualImage, visualImage, new Point(posX, posY));

            PointF[] pointsArray = car.Renderer.RawHitTestPoints(car);

            List<PointF> rotatedUpfacing = new();
            PointF pOrigin = new(0, 0);

            for (int i = 0; i < pointsArray.Length; i++)
            {
                PointF p = MathUtils.RotatePointAboutOrigin(new PointF(pointsArray[i].X, pointsArray[i].Y),
                                                                   pOrigin,
                                                                   90);
                p.X = posX - p.X;
                p.Y = posY - p.Y;

                rotatedUpfacing.Add(p);
            }

            rotatedUpfacing.Add(rotatedUpfacing[0]);

            graphicsOfVisualImage.DrawLines(s_penForCarCircle, rotatedUpfacing.ToArray());
        }

        /// <summary>
        /// Rotates other cars wrt to this one, and draws if within the image "square".
        /// </summary>
        /// <param name="track"></param>
        /// <param name="centerCarId"></param>
        /// <param name="xCenter"></param>
        /// <param name="yCenter"></param>
        /// <returns></returns>
        private static Image OverlaysCarsOnLIDAR(Image track, int centerCarId, int xCenter, int yCenter)
        {
            using Graphics g = Graphics.FromImage(track);

            int width = track.Width;
            int height = track.Height;

            // semi-transparent, so we can see where other cars are (if they overlap)
            using SolidBrush carBrush = new(Color.FromArgb(100, 255, 0, 0));

            // for each car, test if it should be drawn (based on whether it is close enough to center car).
            foreach (int id in LearningAndRaceManager.s_cars.Keys)
            {
                VehicleDrivenByAI car = LearningAndRaceManager.s_cars[id];
                if (car.HasBeenEliminated || id == centerCarId) continue;

                // make our point wrt to main car
                int x = (int)(car.CarImplementation.LocationOnTrack.X - xCenter);
                int y = (int)(car.CarImplementation.LocationOnTrack.Y - yCenter);

                // -10 radius of car for test
                if (x < -width / 2 || x > width / 2 || y < -height / 2 || y > height / 2) continue;

                PointF[] hitTestPointsForCarArray = car.Renderer.RawHitTestPoints(car);

                List<PointF> rotatedUpfacing = new();
                PointF pOriginCar = new(0, 0);

                PointF pThisCar = new(x, y); // MathUtils.RotatePointAboutOrigin(new PointF(x, y), pOriginCar, 0);

                for (int collisionSensorIndex = 0; collisionSensorIndex < hitTestPointsForCarArray.Length; collisionSensorIndex++)
                {
                    // rotate car image about origin to match correct orientation
                    PointF p = MathUtils.RotatePointAboutOrigin(new PointF(hitTestPointsForCarArray[collisionSensorIndex].X, hitTestPointsForCarArray[collisionSensorIndex].Y),
                                                                       pOriginCar,
                                                                       car.CarImplementation.AngleVehicleIsPointingInDegrees);

                    rotatedUpfacing.Add(new PointF(width / 2 + pThisCar.X + p.X, height / 2 + pThisCar.Y + p.Y));
                }

                rotatedUpfacing.Add(rotatedUpfacing[0]);

                // draw the car as a filled in blob
                g.FillPolygon(carBrush, rotatedUpfacing.ToArray());

            }

            g.Flush();

            return track;
        }
    }
}