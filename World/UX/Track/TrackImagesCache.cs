using CarsAndTanks.Settings;

namespace AICarTrack.World.UX.Track
{
    /// <summary>
    /// Class the creates more visually appealing track "tiles" for the track grid editor to use.
    /// </summary>
    internal static class TrackImagesCache
    {
        /// <summary>
        /// This needs to match the size of the tile of grass/asphalt, so things join properly.
        /// </summary>
        internal const int c_sizeOfTile = 140; // px;

        /// <summary>
        /// Image of grass that is tileable.
        /// </summary>
        internal static Bitmap s_TileableBitmapOfGrass = new("UX/Resources/grass.png");

        /// <summary>
        /// Image of asphalt that is tileable.
        /// </summary>
        internal static Bitmap s_TileableBitmapForRoad = new("UX/Resources/asphalt-small2.png");

        /// <summary>
        /// Bitmap of road + grass left to top 90 degrees.
        /// </summary>
        internal static Bitmap? s_leftToTopTrackBitmap;

        /// <summary>
        /// Bitmap sillouette of road + grass left to top 90 degrees.
        /// </summary>
        internal static Bitmap? s_leftToTopTrackBitmapSillouette;

        /// <summary>
        /// Bitmap of road + grass top to right 90 degrees.
        /// </summary>
        internal static Bitmap? s_topToRightTrackBitmap;

        /// <summary>
        /// Bitmap sillouette of road + grass top to right 90 degrees.
        /// </summary>
        internal static Bitmap? s_topToRightTrackBitmapSillouette;

        /// <summary>
        /// Bitmap of road + grass right to bottom 90 degrees.
        /// </summary>
        internal static Bitmap? s_rightToBottomTrackBitmap;

        /// <summary>
        /// Bitmap sillouette of road + grass right to bottom 90 degrees.
        /// </summary>
        internal static Bitmap? s_rightToBottomTrackBitmapSillouette;

        /// <summary>
        /// Bitmap of road + grass left to bottom 90 degrees.
        /// </summary>
        internal static Bitmap? s_leftToBottomTrackBitmap;

        /// <summary>
        /// Bitmap sillouette of road + grass left to bottom 90 degrees.
        /// </summary>
        internal static Bitmap? s_leftToBottomTrackBitmapSillouette;

        /// <summary>
        /// Bitmap of road + grass left to right.
        /// </summary>
        internal static Bitmap? s_leftToRightTrackBitmap;

        /// <summary>
        /// Bitmap sillouette of road + grass left to right.
        /// </summary>
        internal static Bitmap? s_leftToRightTrackBitmapSillouette;

        /// <summary>
        /// Bitmap of road + grass top to bottom.
        /// </summary>
        internal static Bitmap? s_topToBottomTrackBitmap;

        /// <summary>
        /// Bitmap sillouette of road + grass top to bottom.
        /// </summary>
        internal static Bitmap? s_topToBottomTrackBitmapSillouette;

        /// <summary>
        /// Initialises the track images.
        /// </summary>
        internal static void Initialise()
        {
            // the size of tile can be altered, so this handles the re-size otherwise the tiles will look like distinct squares
            s_TileableBitmapOfGrass = (Bitmap)ImageUtils.ResizeImage(s_TileableBitmapOfGrass, c_sizeOfTile, c_sizeOfTile);
            s_TileableBitmapForRoad = (Bitmap)ImageUtils.ResizeImage(s_TileableBitmapForRoad, c_sizeOfTile, c_sizeOfTile);

            // create a left-to-top angle, which we'll then flip and rotate to create the "others".
            s_leftToTopTrackBitmap = CreateLeftToTopTrackBitmap();
            s_leftToTopTrackBitmapSillouette = CreateLeftToTopTrackBitmap(true);

            // lots of flipping of tile images. Rather than have to create logic for each track direction we simply flip in one or both directions.

            s_topToRightTrackBitmap = ImageUtils.FlipH(s_leftToTopTrackBitmap);
            s_topToRightTrackBitmapSillouette = ImageUtils.FlipH(s_leftToTopTrackBitmapSillouette);

            s_rightToBottomTrackBitmap = ImageUtils.FlipV(s_topToRightTrackBitmap);
            s_rightToBottomTrackBitmapSillouette = ImageUtils.FlipV(s_topToRightTrackBitmapSillouette);

            s_leftToBottomTrackBitmap = ImageUtils.FlipH(s_rightToBottomTrackBitmap);
            s_leftToBottomTrackBitmapSillouette = ImageUtils.FlipH(s_rightToBottomTrackBitmapSillouette);

            // left to right
            s_leftToRightTrackBitmap = CreateLeftToRightTrackBitmap();
            s_leftToRightTrackBitmapSillouette = CreateLeftToRightTrackBitmap(true);

            // top to bottom
            s_topToBottomTrackBitmap = ImageUtils.FlipHV(s_leftToRightTrackBitmap);
            s_topToBottomTrackBitmapSillouette = ImageUtils.FlipHV(s_leftToRightTrackBitmapSillouette);
        }

        /// <summary>
        /// Creates a 90 degree track (grass overlaid with road) and
        /// sillouette of the road (for collision detection).
        ///     ____      ____
        ///     |  |      |##|
        /// .__/  /   .__/##/ 
        /// |____/    |####/  
        /// 
        /// </summary>
        /// <returns></returns>
        private static Bitmap CreateLeftToTopTrackBitmap(bool sillouette = false)
        {
            int halfroadwidth = Config.s_settings.World.RoadWidthInPixels / 2;
            int r = c_sizeOfTile / 2;

            Bitmap b = new(c_sizeOfTile, c_sizeOfTile);

            using Graphics g = Graphics.FromImage(b);
            g.ToHighQuality();

            // for sillouettes, colour the background white (grass), and for non-sillouette overlay the grass PNG image
            if (!sillouette) g.DrawImageUnscaled(s_TileableBitmapOfGrass, 0, 0); else g.FillRectangle(new SolidBrush(Color.White), 0, 0, b.Width, b.Height);

            // we're drawing a quarter circle.
            for (int r1 = r - (halfroadwidth + 1); r1 <= r + halfroadwidth; r1++)
            {
                for (float a = 0; a < 90; a += 0.15f)
                {
                    int x = (int)(0.5F + Math.Cos(MathUtils.DegreesInRadians(a)) * r1);
                    int y = (int)(0.5F + Math.Sin(MathUtils.DegreesInRadians(a)) * r1);

                    if (!sillouette)
                    {
                        // road pixel color unless edge, which we color white.
                        if (r1 == r - (halfroadwidth + 1) || r1 == r + halfroadwidth) b.SetPixel(x, y, Color.FromArgb(180, 255, 255, 255)); else b.SetPixel(x, y, s_TileableBitmapForRoad.GetPixel(x, y));
                    }
                    else
                    {
                        // black on sillouette = track
                        b.SetPixel(x, y, Color.Black);
                    }
                }
            }

            return b;
        }

        /// <summary>
        /// Creates a road left to right.
        /// Creates sillouette of a road left to right.
        /// ._____.      ._____.
        /// |_____|      |#####|
        /// </summary>
        /// <returns></returns>
        private static Bitmap CreateLeftToRightTrackBitmap(bool sillouette = false)
        {
            int halfroadwidth = Config.s_settings.World.RoadWidthInPixels / 2;

            int radiusToCenter = c_sizeOfTile / 2;

            Bitmap b = new(c_sizeOfTile, c_sizeOfTile);

            using Brush br = new SolidBrush(Color.Green);

            using Graphics g = Graphics.FromImage(b);
            g.ToHighQuality();

            // for sillouettes, color the background white (grass), and for non-sillouette overlay the grass PNG image
            if (!sillouette) g.DrawImageUnscaled(s_TileableBitmapOfGrass, 0, 0); else g.FillRectangle(new SolidBrush(Color.White), 0, 0, b.Width, b.Height);

            for (int r1 = radiusToCenter - (halfroadwidth + 1); r1 <= radiusToCenter + halfroadwidth; r1++)
            {
                for (int a = 0; a < b.Width; a++)
                {
                    int x = a;
                    int y = r1;

                    if (!sillouette)
                    {
                        // road pixel color unless it's an edge, which we color white.
                        if (r1 == radiusToCenter - (halfroadwidth + 1) || r1 == radiusToCenter + halfroadwidth) b.SetPixel(x, y, Color.FromArgb(180, 255, 255, 255)); else b.SetPixel(x, y, s_TileableBitmapForRoad.GetPixel(x, y));
                    }
                    else
                    {
                        // black on sillouette = track
                        b.SetPixel(x, y, Color.Black);
                    }
                }
            }

            return b;
        }
    }
}