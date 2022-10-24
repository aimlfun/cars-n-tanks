using CarsAndTanks.AI;
using CarsAndTanks.AI.UX.Dials;
using CarsAndTanks.Settings;
using CarsAndTanks.Utilities;

namespace CarsAndTanks.Vision.UX
{
    internal static class VisionVisualiser
    {
        /// <summary>
        /// Background color of the dashboard.
        /// </summary>
        internal readonly static Color s_dashBoardColor = Color.FromArgb(255, 171, 78, 19);

        /// <summary>
        /// Really cool visualisation in real-time.
        /// </summary>
        /// <returns></returns>
        internal static Bitmap Render(NeuralNetwork network)
        {
            // sized big enough for circle map + neural network (that varies in width based on the largest neuron layer)
            Bitmap visualImage = new(200, 200);

            using Graphics graphicsOfVisualImage = Graphics.FromImage(visualImage);

            // this dashboard image is ruined if the graphics are blocky, so we set high quality at expense of performance
            graphicsOfVisualImage.ToHighQuality();

            // draw the car # as it doesn't change, and if we move to a different car the whole thing has to be redrawn
            if (network.Id >= 0) graphicsOfVisualImage.DrawString(network.Id.ToString(), Config.s_settings.Display.fontForGeneration, Brushes.Black, new PointF(10, 10));

            // draw a visualisation of the car + LIDAR superimposed on the track.
            MapVisionGauge.DrawBlobWithLIDARAndRotatedTrack(network, visualImage, graphicsOfVisualImage);


            // provide the image for "paint" operation
            return visualImage;
        }
    }
}
