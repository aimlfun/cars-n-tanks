namespace AICarTrack.World.UX.VehicleRenderers
{
    internal static class TankTelemetry
    {
        /// <summary>
        /// Cache the tank image, so we simply draw the throttle on top.
        /// </summary>
        private static Bitmap? s_tankImage = null;

        /// <summary>
        /// Cache the width of the container we are rendering into.
        /// </summary>
        private static int s_width = 0;

        /// <summary>
        /// Cache the height of the container we are rendering into.
        /// </summary>
        private static int s_height = 0;

        /// <summary>
        /// Draws a tank with 2 bars (indicating throttle power for left and right).
        /// </summary>
        /// <param name="car"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        internal static Bitmap TankTelemetryAsBitmap(VehicleDrivenByAI car, int width, int height)
        {
            if (s_tankImage is null)
            {
                s_tankImage = new Bitmap("UX/Resources/TankFromAbove.png");
                s_tankImage.MakeTransparent(Color.White);
                s_tankImage = (Bitmap)ImageUtils.ResizeImage((Image)s_tankImage, width, height);
                s_tankImage = ReplaceColourWithTolerance(s_tankImage, 70, Color.White, Color.Transparent);
                s_width = width;
                s_height = height;
                s_tankImage.MakeTransparent(Color.White);
            }

            Bitmap b = new(s_width, s_height);

            Graphics g = Graphics.FromImage(b);

            g.DrawImage(s_tankImage, -1, 0);
            g.ToHighQuality();

            double barMiddle = s_height - s_height / 3;
            double barLeft = car.carInputState.Throttle2 * (s_height / 3);
            double barRight = car.carInputState.Throttle1 * (s_height / 3);

            int tankWidth = 55;
            int posX = b.Width / 2;

            int y1 = (int)barMiddle;
            int y2 = y1 - (int)barLeft;
            if (y1 > y2) (y1, y2) = (y2, y1);

            g.FillRectangle(new SolidBrush(Color.FromArgb(200, 255, 0, 0)), posX - tankWidth, y1, 10, y2 - y1);
            g.DrawLine(new Pen(Color.Black, 2), posX - tankWidth, (int)barMiddle, posX - tankWidth + 12, (int)barMiddle);

            y1 = (int)barMiddle;
            y2 = y1 - (int)barRight;
            if (y1 > y2) (y1, y2) = (y2, y1);

            g.FillRectangle(new SolidBrush(Color.FromArgb(200, 255, 0, 0)), posX + tankWidth - 12, y1, 10, y2 - y1);

            g.DrawLine(new Pen(Color.Black, 2), posX + tankWidth - 14, (int)barMiddle, posX + tankWidth - 1, (int)barMiddle);

            return b;
        }

        /// <summary>
        /// Replaces a colour on an image, with a degree of tolerance.
        /// </summary>
        /// <param name="inputBitmap"></param>
        /// <param name="tolerance"></param>
        /// <param name="oldColour"></param>
        /// <param name="newColour"></param>
        /// <returns></returns>
        internal static Bitmap ReplaceColourWithTolerance(Bitmap inputBitmap, int tolerance, Color oldColour, Color newColour)
        {
            Bitmap outputImage = new(inputBitmap.Width, inputBitmap.Height);
            using Graphics graphicsForReplacementBitmap = Graphics.FromImage(outputImage);
            graphicsForReplacementBitmap.DrawImage(inputBitmap, 0, 0);

            for (int y = 0; y < outputImage.Height; y++)
            {
                for (int x = 0; x < outputImage.Width; x++)
                {
                    Color PixelColor = outputImage.GetPixel(x, y);

                    // pixel is not within tolerance?
                    if (PixelColor.R <= oldColour.R - tolerance || PixelColor.R >= oldColour.R + tolerance ||
                        PixelColor.G <= oldColour.G - tolerance || PixelColor.G >= oldColour.G + tolerance ||
                        PixelColor.B <= oldColour.B - tolerance || PixelColor.B >= oldColour.B + tolerance)
                    {
                        continue;
                    }

                    int RColorDiff = ReplaceColour(oldColour.R, newColour.R, PixelColor.R);
                    int GColorDiff = ReplaceColour(oldColour.G, newColour.G, PixelColor.G);
                    int BColorDiff = ReplaceColour(oldColour.B, newColour.B, PixelColor.B);

                    outputImage.SetPixel(x, y, Color.FromArgb(RColorDiff, GColorDiff, BColorDiff));
                }
            }

            return outputImage;
        }

        /// <summary>
        /// Replaces a colourr.
        /// </summary>
        /// <param name="oldColor"></param>
        /// <param name="newColour"></param>
        /// <param name="pixelColor"></param>
        /// <returns></returns>
        private static int ReplaceColour(byte oldColor, byte newColour, byte pixelColor)
        {
            int colourDiff = oldColor - pixelColor;

            if (pixelColor > oldColor) colourDiff = newColour + colourDiff; else colourDiff = newColour - colourDiff;

            return colourDiff.Clamp(0, 255);
        }
    }
}
