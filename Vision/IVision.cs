namespace AICarTrack
{
    /// <summary>
    /// Declare a "vision" system for the AI.
    /// - sensors from vision system as numbers for the neural network
    /// - draw what the sensor saw.
    /// 
    /// The idea is that one can plug in "different" sensors. Currently we have a "Mono" LIDAR only.
    /// </summary>
    interface IVision
    {
        /// <summary>
        /// Allows caller to plug sensor values in.
        /// </summary>
        /// <param name="sensor"></param>
        void SetLIDAR(int id, double[] sensor);

        /// <summary>
        /// Looks at the track and provide inputs to the neural network.
        /// It MUST store the location and angle, to be able to draw.
        /// </summary>
        /// <param name="AngleCarIsPointingInDegrees">Direction of view.</param>
        /// <param name="location">Where looking from.</param>
        /// <returns></returns>
        double[] VisionSensorOutput(int id, double AngleLookingInDegrees, PointF location);

        /// <summary>
        /// Draws the vision sensor onto a bitmap.
        /// </summary>
        /// <param name="graphics">What to draw on.</param>
        /// <param name="visualImage">The image the graphics is for.</param>
        /// <param name="visualCenterPoint">The centerpoint the sensor radiates out of.</param>
        void DrawSensorToImage(int id, Graphics graphics, Bitmap visualImage, PointF visualCenterPoint);
    }
}
