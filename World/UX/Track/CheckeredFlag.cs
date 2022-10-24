using CarsAndTanks.Settings;

namespace AICarTrack
{
    internal static class CheckeredFlag
    {
        /// <summary>
        /// Draws a checkered start line direct onto the canvas used for hit detection.
        /// </summary>
        /// <param name="graphics"></param>
        internal static void Draw(Graphics graphics)
        {
            PointF p1 = new(LearningAndRaceManager.s_startPoint.X, LearningAndRaceManager.s_startPoint.Y - Config.s_settings.World.RoadWidthInPixels / 2 - 0);
            PointF p2 = new(LearningAndRaceManager.s_startPoint.X, LearningAndRaceManager.s_startPoint.Y + Config.s_settings.World.RoadWidthInPixels / 2 + 1);

            int sizeOfCheckerSquare = 3;

            using SolidBrush brushBlackPaint = new(Color.FromArgb(230, 0, 0, 0));
            using SolidBrush brushWhitePaint = new(Color.FromArgb(230, 255, 255, 255));

            int c = 0;

            for (int x = (int)Math.Min(p1.X, p2.X); x < (int)Math.Min(p1.X, p2.X) + 10; x += sizeOfCheckerSquare)
            {
                c = 1 - c;

                int d = 0;

                for (int y = (int)Math.Min(p1.Y, p2.Y); y < (int)Math.Max(p1.Y, p2.Y); y += sizeOfCheckerSquare)
                {
                    graphics.FillRectangle((d == c) ? brushWhitePaint : brushBlackPaint, new Rectangle(x, y, sizeOfCheckerSquare, sizeOfCheckerSquare));

                    d = 1 - d;
                }
            }
        }

    }
}
