namespace AICarTrack
{
    /// <summary>
    /// Maths related utility functions.
    /// </summary>
    internal static class MathUtils
    {
        /// <summary>
        /// Determine a point rotated by an angle around an origin.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="origin"></param>
        /// <param name="angleInDegrees"></param>
        /// <returns></returns>
        internal static PointF RotatePointAboutOrigin(PointF point, PointF origin, double angleInDegrees)
        {
            return RotatePointAboutOriginInRadians(point, origin, DegreesInRadians(angleInDegrees));
        }

        /// <summary>
        /// Determine a point rotated by an angle around an origin.
        /// </summary>
        /// <param name="point"></param>
        /// <param name="origin"></param>
        /// <param name="angleInRadians"></param>
        /// <returns></returns>
        internal static PointF RotatePointAboutOriginInRadians(PointF point, PointF origin, double angleInRadians)
        {
            double cos = Math.Cos(angleInRadians);
            double sin = Math.Sin(angleInRadians);
            float dx = point.X - origin.X;
            float dy = point.Y - origin.Y;

            // standard maths for rotation.
            return new PointF((float)(cos * dx - sin * dy + origin.X),
                              (float)(sin * dx + cos * dy + origin.Y)
            );
        }

        /// <summary>
        /// Logic requires radians but we track angles in degrees, this converts.
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        internal static double DegreesInRadians(double angle)
        {
            return (double)Math.PI * angle / 180;
        }

        /// <summary>
        /// Converts radians into degrees. 
        /// One could argue, WHY not just use degrees? Preference. Degrees are more intuitive than 2*PI offset values.
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        internal static double RadiansInDegrees(double radians)
        {
            // radians = PI * angle / 180
            // radians * 180 / PI = angle
            return radians * 180F / Math.PI;
        }

        /// <summary>
        /// Computes the distance between 2 points using Pythagoras's theorem a^2 = b^2 + c^2.
        /// </summary>
        /// <param name="pt1">First point.</param>
        /// <param name="pt2">Second point.</param>
        /// <returns></returns>
        internal static float DistanceBetweenTwoPoints(PointF pt1, PointF pt2)
        {
            float dx = pt2.X - pt1.X;
            float dy = pt2.Y - pt1.Y;

            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Used to determine if the car centre "point" is touching the gate give or take a small margin of error.
        /// It works on the basis that if point C is on line AB, then sum of distance point to A + point C to B should be the same as distance between point A & point B.
        /// </summary>
        /// <param name="linePointA">Start of line.</param>
        /// <param name="linePointB">End of line.</param>
        /// <param name="point">Point that might be on the line.</param>
        /// <returns>true - "point" is on the line between linePointA and linePointB.</returns>
        internal static bool IsPointOnLine(PointF linePointA, PointF linePointB, PointF point)
        {
            // get distance from the point to the two ends of the line
            float distanceBetweenPointAndLinePointA = DistanceBetweenTwoPoints(point, linePointA);
            float distanceBetweenPointAndLinePointB = DistanceBetweenTwoPoints(point, linePointB);

            // if the two distances are equal to the line's length, the point is on the line!
            // note we use the buffer here to give a range, rather than one.
            float sum = distanceBetweenPointAndLinePointA + distanceBetweenPointAndLinePointB - DistanceBetweenTwoPoints(linePointA, linePointB);

            // 0.1F is a buffer; since floats are so minutely accurate, add
            // a little buffer zone that will give collision. higher # = less accurate
            return (sum >= -0.1F && sum <= 0.1F);
        }

        /// <summary>
        /// Returns a value between min and max (never outside of).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        internal static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0)
            {
                return min;
            }

            if (val.CompareTo(max) > 0)
            {
                return max;
            }

            return val;
        }
    }
}
