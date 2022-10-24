using CarsAndTanks.Learn;
using CarsAndTanks.Settings;
using System.Security.Cryptography;

namespace CarsAndTanks.World.UX.Track;

/// <summary>
/// Creates a random track based on a polygon with warped sides and variable angles.
/// </summary>
internal static class RandomPolygonTrack
{
    /// <summary>
    /// Picks a random number of sides, and then draws lines between them. 
    /// An extra wiggle is put in to make the track less uniform.
    /// </summary>
    internal static void Create()
    {
        TrackAndBackgroundCache.Clear();

        LearningAndRaceManager.RemoveAllTrackSegments();

        int sides = 3 + RandomNumberGenerator.GetInt32(1, 20);
        int radius = Config.s_settings.World.PolygonRadiusOfGeneratedTracksInPixels;

        double anglePerSide = 2 * Math.PI / sides; // 2*PI=360 degrees. "n" sides requires dividing 360/n as the angle we rotate for each corner

        for (double i = 0; i < sides; i++)
        {
            double warp = anglePerSide / 5 * (RandomNumberGenerator.GetInt32(0, 1000) - 500) / 1000;
            int wiggle = RandomNumberGenerator.GetInt32(1, 8) > 6 ? -Config.s_settings.World.PolygonMaxWiggleInPixels : 0;

            double x = Math.Sin(i * anglePerSide + warp) * (radius + wiggle);
            double y = Math.Cos(i * anglePerSide + warp) * (radius + wiggle);

            float pointX = (float)(radius + Config.s_settings.World.OffsetOfGeneratedTrackFromEdgeInPixels + x);
            float pointY = (float)(radius + Config.s_settings.World.OffsetOfGeneratedTrackFromEdgeInPixels + y);

            LearningAndRaceManager.AddTrackSegment(new Point((int)(pointX + 0.5F), (int)(pointY + 0.5F)));
        }
    }

}
