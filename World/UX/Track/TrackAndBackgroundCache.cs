#define plain_grass
using AICarTrack.World.UX.Track;
using CarsAndTanks.Settings;
using CarsAndTanks.World;
using System.Drawing.Imaging;

namespace AICarTrack
{
    /// <summary>
    /// Draws and caches the track. It takes an in memory snapshot of the track for collision detection.
    /// </summary>
    internal static class TrackAndBackgroundCache
    {
        /// <summary>
        /// This is "panelTrack" i.e. our canvas.
        /// </summary>
        private static Control? s__canvas;

        /// <summary>
        /// This is "panelTrack" i.e. our canvas.
        /// </summary>
        internal static Control? Canvas
        {
            get
            {
                return s__canvas;
            }
            set
            {
                s__canvas = value;

                if (s__canvas != null)
                {
                    // Bitmap.Width/Height is apparently slow (GDI calls?), according to Diagnostic Tools Hotpath
                    // so we grab them here into local variables, and everything runs faster!
                    width = s__canvas.Width;
                    height = s__canvas.Height;
                }
            }
        }

        #region CACHE OF TRACK BITMAP BYTES FOR PERFORMANCE        
        /// <summary>
        /// This is the track bitmap. If null, the track image needs to be locked and copied to it.
        /// </summary>
        public static Bitmap? s_srcTrackBitMap = null;

        /// <summary>
        /// This is the attributes of the track bitmap.
        /// </summary>
        private static BitmapData? s_srcTrackBitMapData;

        /// <summary>
        /// This is a pointer to the track bitmap's data.
        /// </summary>
        private static IntPtr s_srcTrackBitMapDataPtr;

        /// <summary>
        /// Bytes per row of pixels.
        /// </summary>
        private static int s_strideTrack;

        /// <summary>
        /// This is how many bytes the track bitmap is.
        /// </summary>
        private static int s_totalLengthTrack;

        /// <summary>
        /// This is the pixels in the track bitmap.
        /// </summary>
        private static byte[] s_rgbValuesTrack = Array.Empty<byte>();

        /// <summary>
        /// This is how many bytes each pixel occupies in the track bitmap.
        /// </summary>
        private static int s_bytesPerPixelTrack;

        /// <summary>
        /// This is how many bytes per row of the track bitmap image (used to multiply "y" by to get to the correct data).
        /// </summary>
        private static int s_offsetTrack;
        #endregion

        /// <summary>
        /// This is the image of the track (null=needs to be drawn).
        /// It gets bit-blitted every time we draw cars.
        /// </summary>
        private static Bitmap? s_cachedImageOfTrack = null;

        /// <summary>
        /// This is the sillouette image of the track
        /// Used for collision detection.
        /// </summary>
        private static Bitmap? s_cachedSillouetteOfTrack = null;

        /// <summary>
        /// Width of s_canvas.
        /// </summary>
        private static int width = -1;

        /// <summary>
        /// Height of s_canvas.
        /// </summary>
        private static int height = -1;

        /// <summary>
        /// Returns a sillouette of the track used for collision detection.
        /// </summary>
        internal static Bitmap? CachedSillouetteOfTrack
        {
            get { return s_cachedSillouetteOfTrack; }
            set
            {
                s_cachedSillouetteOfTrack = value;
            }
        }

        /// <summary>
        /// Returns the image of the track, and ensures our in memory copy is always fresh by setting
        /// it to null to force it.
        /// </summary>
        internal static Bitmap? CachedImageOfTrack
        {
            get { return s_cachedImageOfTrack; }
            set
            {
                s_cachedImageOfTrack = value;
                s_srcTrackBitMap = null; // force in-memory caching
            }
        }

        /// <summary>
        /// Started off as find the colour the pixel.  EQUIV: return ImageOfTrack.GetPixel(x, y); done using our in memory map.
        /// But since separating sillouette from visuals, we only check white/black pixels.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>0=road, 255=grass</returns>
        internal static byte GetPixel(int x, int y)
        {
            //   Dereference error is CORRECT _if_ one didn't initialise it. However GetPixel occurs after track is present, and thus populated

            if (x < 0 || x >= width) return 255; // Color.White (grass)
            if (y < 0 || y >= height) return 255; // Color.White (grass)

            int indexOriginal = x * s_bytesPerPixelTrack + y * s_offsetTrack;

            return s_rgbValuesTrack[indexOriginal + 2]; // use "RED" channel as we've adopted white (255,255,255) / black (0,0,0)
        }

        /// <summary>
        /// Returns the track from the cache, drawing it if the cache is empty.
        /// </summary>
        internal static Bitmap GetTrackFromCache()
        {
            if (CachedImageOfTrack is not null)
            {
                //  cache the track as a byte array rather than keep copying it (performance improved)
                if (s_srcTrackBitMap is null) CopyImageOfTrackToAnAccessibleInMemoryArray();

                return CachedImageOfTrack; // it's cached, return the image
            }

            // it's not cached, draw the track to the image bitmap and cache it.

            if (Canvas is null) throw new NullReferenceException(nameof(Canvas));

            CachedImageOfTrack = new Bitmap(width, height);
            CachedSillouetteOfTrack = new Bitmap(width, height);

            using Graphics graphicsOfTrackImage = Graphics.FromImage(CachedImageOfTrack);
            using Graphics graphicsOfTrackSillouetteImage = Graphics.FromImage(CachedSillouetteOfTrack);

            graphicsOfTrackImage.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graphicsOfTrackImage.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            graphicsOfTrackSillouetteImage.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graphicsOfTrackSillouetteImage.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            graphicsOfTrackSillouetteImage.FillRectangle(new SolidBrush(Color.White), 0, 0, width, height);

            DrawGrass(graphicsOfTrackImage);

            if (LearningAndRaceManager.s_trackSegments.Count > 1)
            {
                Config.s_settings.Display.RoadPen.Width = Config.s_settings.World.RoadWidthInPixels;
                graphicsOfTrackImage.DrawLines(Config.s_settings.Display.RoadPen, LearningAndRaceManager.s_trackSegments.ToArray());

                graphicsOfTrackSillouetteImage.DrawLines(Config.s_settings.Display.RoadPen, LearningAndRaceManager.s_trackSegments.ToArray());
            }

            graphicsOfTrackImage.Flush();
            graphicsOfTrackSillouetteImage.Flush();
            graphicsOfTrackSillouetteImage.Dispose();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (!WorldManager.IsGrass(CachedSillouetteOfTrack.GetPixel(x, y)))
                    {
                        CachedImageOfTrack.SetPixel(x, y, TrackImagesCache.s_TileableBitmapForRoad.GetPixel(x % TrackImagesCache.s_TileableBitmapForRoad.Width, y % TrackImagesCache.s_TileableBitmapForRoad.Height));
                    }
                }
            }

            if (LearningAndRaceManager.s_trackSegments.Count > 1)
                CheckeredFlag.Draw(graphicsOfTrackImage);

            //  cache the track as a byte array rather than keep copying it (performance improved)
            if (s_srcTrackBitMap is null) CopyImageOfTrackToAnAccessibleInMemoryArray();

            return CachedImageOfTrack;
        }

        /// <summary>
        /// Cache the track as a byte array rather than keep copying it (performance improved).
        /// </summary>
        private static void CopyImageOfTrackToAnAccessibleInMemoryArray()
        {
            if (CachedSillouetteOfTrack is null) throw new Exception("track image should be populated before calling this."); // can't cache what has been drawn!

            s_srcTrackBitMap = CachedSillouetteOfTrack;
            s_srcTrackBitMapData = s_srcTrackBitMap.LockBits(new Rectangle(0, 0, s_srcTrackBitMap.Width, s_srcTrackBitMap.Height), ImageLockMode.ReadOnly, CachedSillouetteOfTrack.PixelFormat);
            s_srcTrackBitMapDataPtr = s_srcTrackBitMapData.Scan0;
            s_strideTrack = s_srcTrackBitMapData.Stride;

            s_totalLengthTrack = Math.Abs(s_strideTrack) * s_srcTrackBitMap.Height;
            s_rgbValuesTrack = new byte[s_totalLengthTrack];

            s_bytesPerPixelTrack = Bitmap.GetPixelFormatSize(s_srcTrackBitMapData.PixelFormat) / 8;
            s_offsetTrack = s_strideTrack;
            System.Runtime.InteropServices.Marshal.Copy(s_srcTrackBitMapDataPtr, s_rgbValuesTrack, 0, s_totalLengthTrack);

            s_srcTrackBitMap.UnlockBits(s_srcTrackBitMapData);
        }

        /// <summary>
        /// Draws grass.
        /// </summary>
        /// <param name="gr"></param>
        private static void DrawGrass(Graphics gr)
        {
            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            if (FormMain.s_trackCanvas is null) throw new Exception(nameof(FormMain.s_trackCanvas) + " should not be null"); // this is the canvas we're drawing on

            for (int y = 0; y < FormMain.s_trackCanvas.Height; y += TrackImagesCache.s_TileableBitmapOfGrass.Height - 1)
            {
                for (int x = 0; x < FormMain.s_trackCanvas.Width; x += TrackImagesCache.s_TileableBitmapOfGrass.Width - 1)
                {
                    gr.DrawImage(TrackImagesCache.s_TileableBitmapOfGrass, x, y, TrackImagesCache.s_TileableBitmapOfGrass.Width, TrackImagesCache.s_TileableBitmapOfGrass.Height);
                }
            }
        }

        /// <summary>
        /// Forces it to redraw the track next time.
        /// </summary>
        internal static void Clear()
        {
            CachedImageOfTrack = null;
        }

        /// <summary>
        /// Copies a square of the map centred on the point provided. This is used in the NN visualiser.
        /// </summary>
        /// <param name="centerX"></param>
        /// <param name="centerY"></param>
        /// <param name="offset"></param>
        /// <returns>Bitmap size (offsetx2,offsetx2) copied off track centered on the location.</returns>
        internal static Bitmap CopySquareFromTrackImageAt(int centerX, int centerY, int offset)
        {
            Bitmap srcSNAPBitMap = new(2 * offset, 2 * offset);
            BitmapData srcSNAPMapData = srcSNAPBitMap.LockBits(new Rectangle(0, 0, srcSNAPBitMap.Width, srcSNAPBitMap.Height), ImageLockMode.ReadWrite, srcSNAPBitMap.PixelFormat);
            IntPtr srcSNAPMapDataPtr = srcSNAPMapData.Scan0;
            int strideSNAP = srcSNAPMapData.Stride;

            int totalLengthSNAP = Math.Abs(strideSNAP) * srcSNAPBitMap.Height;
            byte[] rgbValuesSNAP = new byte[totalLengthSNAP];

            int bytesPerPixelSNAP = Bitmap.GetPixelFormatSize(srcSNAPBitMap.PixelFormat) / 8;
            int offsetSNAP = strideSNAP;

            for (int x = -offset; x < offset; x++)
            {
                for (int y = -offset; y < offset; y++)
                {
                    int indexOriginal = (centerX + x) * s_bytesPerPixelTrack + ((centerY + y) * s_offsetTrack);
                    int indexNew = (x + offset) * bytesPerPixelSNAP + (y + offset) * offsetSNAP;

                    if (indexOriginal >= 0 && indexOriginal < s_totalLengthTrack)
                    {
                        rgbValuesSNAP[indexNew] = s_rgbValuesTrack[indexOriginal];
                        rgbValuesSNAP[indexNew + 1] = s_rgbValuesTrack[indexOriginal + 1];
                        rgbValuesSNAP[indexNew + 2] = s_rgbValuesTrack[indexOriginal + 2];
                        rgbValuesSNAP[indexNew + 3] = s_rgbValuesTrack[indexOriginal + 3];
                    }
                    else
                    {
                        rgbValuesSNAP[indexNew] = 0;
                        rgbValuesSNAP[indexNew + 1] = 0;
                        rgbValuesSNAP[indexNew + 2] = 0;
                        rgbValuesSNAP[indexNew + 3] = 0;
                    }
                }
            }

            // copy the memory array with bitmaps into the Bitmap
            System.Runtime.InteropServices.Marshal.Copy(rgbValuesSNAP, 0, srcSNAPMapDataPtr, totalLengthSNAP);

            srcSNAPBitMap.UnlockBits(srcSNAPMapData);

            return srcSNAPBitMap;
        }

    }
}