using CarsAndTanks.Learn;
using CarsAndTanks.Settings;

namespace CarsAndTanks.World.UX.Track;

internal static class CheckeredFlag
{
    /// <summary>
    /// Draws a checkered start line direct onto the canvas.
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

        // this took me a few attempts to get right.
        // as we move left to right, we toggle white / black starting colour.
        // xx__xx__xx__ ...
        // ^0  1   2   x size of sizeOfCheckSquare
        //
        // But without doing something in the "y" direction we'd get
        // xx__xx__xx__ ...
        // xx__xx__xx__ ...
        // xx__xx__xx__ ...
        //
        // So when going down, we start on 0. If that's what "x" is, we colour white else back.
        // __  (c=1, d=0)   d=0. c!=d => black
        // xx  (c=1, d=1-d) d=1. c==d => white
        // __  (c=1, d=1-d) d=0. c!=d => black

        // when x=1, c=1-c, which is "0"
        // xx  (c=0, d=0)   d=0. c==d => white
        // __  (c=0, d=1-d) d=1. c!=d => black
        // xx  (c=0, d=1-d) d=0. c==d => white

        // that gives you the checkered pattern.

        for (int x = (int)Math.Min(p1.X, p2.X); x < (int)Math.Min(p1.X, p2.X) + 10; x += sizeOfCheckerSquare)
        {
            c = 1 - c;

            int d = 0;

            for (int y = (int)Math.Min(p1.Y, p2.Y); y < (int)Math.Max(p1.Y, p2.Y); y += sizeOfCheckerSquare)
            {
                graphics.FillRectangle(d == c ? brushWhitePaint : brushBlackPaint, new Rectangle(x, y, sizeOfCheckerSquare, sizeOfCheckerSquare));

                d = 1 - d;
            }
        }
    }
}
