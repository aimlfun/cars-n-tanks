using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace CarsAndTanks.Utilities;

/// <summary>
/// Image utilities.
/// </summary>
internal static class ImageUtils
{
    /// <summary>
    /// Pen for drawing the circles.
    /// </summary>
    private static readonly Pen s_circlePen = new(Color.FromArgb(100, 255, 255, 255));

    /// <summary>
    /// Draws a circle around a center with a specific radius.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="radius"></param>
    internal static void DrawCircle(Graphics g, float x, float y, float radius)
    {
        g.DrawEllipse(s_circlePen, x - radius / 2, y - radius / 2, radius, radius);
    }

    /// <summary>
    /// Crops the image into a circle (otherwise we'll see a bizarre looking rotated square).
    /// </summary>
    /// <param name="srcImage"></param>
    /// <param name="backGround"></param>
    /// <returns></returns>
    internal static Image CropToCircle(Image srcImage, Color backGround)
    {
        Image imageCroppedAsCircle = new Bitmap(srcImage.Width, srcImage.Height, srcImage.PixelFormat);

        using Graphics g = Graphics.FromImage(imageCroppedAsCircle);
        g.CompositingQuality = CompositingQuality.HighQuality;

        using Brush br = new SolidBrush(backGround);

        g.FillRectangle(br, 0, 0, imageCroppedAsCircle.Width, imageCroppedAsCircle.Height);

        using GraphicsPath path = new();

        path.AddEllipse(0, 0, imageCroppedAsCircle.Width, imageCroppedAsCircle.Height);
        g.SetClip(path); // when draw occurs, path is painted circle and thus only that appears
        g.DrawImageUnscaled(srcImage, 0, 0);

        return imageCroppedAsCircle;
    }

    /// <summary>
    /// Rotates an image about center.
    /// </summary>
    /// <param name="bitmap">Bitmap to rotate.</param>
    /// <param name="angleInDegrees">Angle to rotate it.</param>
    /// <returns></returns>
    internal static Bitmap RotateBitmap(Image bitmap, double angleInDegrees)
    {
        Bitmap returnBitmap = new(bitmap.Width, bitmap.Height, PixelFormat.Format32bppPArgb);

        using Graphics graphics = Graphics.FromImage(returnBitmap);

        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic; // better quality
        graphics.TranslateTransform((float)bitmap.Width / 2, (float)bitmap.Height / 2); // to center about middle, we need to move the point of rotation to middle
        graphics.RotateTransform((float)angleInDegrees);
        graphics.TranslateTransform(-(float)bitmap.Width / 2, -(float)bitmap.Height / 2); // undo the point of rotation

        var z = graphics.CompositingMode;
        graphics.CompositingMode = CompositingMode.SourceCopy;
        graphics.DrawImageUnscaled(bitmap, new Point(0, 0));
        graphics.CompositingMode = z;

        return returnBitmap;
    }

    /// <summary>
    /// Similar to RotateBitmap, but with the background coloured.
    /// </summary>
    /// <param name="bitmap"></param>
    /// <param name="angleInDegrees"></param>
    /// <returns></returns>
    internal static Bitmap RotateBitmapWithColoredBackground(Bitmap bitmap, double angleInDegrees)
    {
        Bitmap returnBitmap = new(bitmap.Width, bitmap.Height, PixelFormat.Format32bppPArgb);

        using Graphics graphics = Graphics.FromImage(returnBitmap);

        Color c = bitmap.GetPixel(0, 0);

        graphics.InterpolationMode = InterpolationMode.NearestNeighbor; // rough quality
        graphics.CompositingQuality = CompositingQuality.HighSpeed;
        graphics.TranslateTransform((float)bitmap.Width / 2, (float)bitmap.Height / 2); // to center about middle, we need to move the point of rotation to middle
        graphics.RotateTransform((float)angleInDegrees);
        graphics.TranslateTransform(-(float)bitmap.Width / 2, -(float)bitmap.Height / 2); // undo the point of rotation
        //graphics.Clear(c);

        var z = graphics.CompositingMode;
        graphics.CompositingMode = CompositingMode.SourceCopy;
        graphics.DrawImageUnscaled(bitmap, new Point(0, 0));
        graphics.CompositingMode = z;

        return returnBitmap;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="originalFilename"></param>
    /// <param name="canvasWidth"></param>
    /// <param name="canvasHeight"></param>
    /// <returns></returns>
    internal static Image ResizeImage(Image image, int canvasWidth, int canvasHeight)
    {
        int originalWidth = image.Width;
        int originalHeight = image.Height;

        Image thumbnail = new Bitmap(canvasWidth, canvasHeight); // changed parm names
        using (Graphics graphic = Graphics.FromImage(thumbnail))
        {
            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;

            // Figure out the ratio
            double ratioX = canvasWidth / (double)originalWidth;
            double ratioY = canvasHeight / (double)originalHeight;

            double ratio = ratioX < ratioY ? ratioX : ratioY; // use whichever multiplier is smaller

            // now we can get the new height and width
            int newHeight = Convert.ToInt32(originalHeight * ratio);
            int newWidth = Convert.ToInt32(originalWidth * ratio);

            // Now calculate the X,Y position of the upper-left corner 
            // (one of these will always be zero)
            int posX = Convert.ToInt32((canvasWidth - originalWidth * ratio) / 2);
            int posY = Convert.ToInt32((canvasHeight - originalHeight * ratio) / 2);

            graphic.Clear(Color.Transparent); // white padding
            graphic.DrawImage(image, posX, posY, newWidth, newHeight);
        }

        return thumbnail;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bmp"></param>
    /// <param name="col"></param>
    /// <param name="delta"></param>
    /// <returns></returns>
    internal static Bitmap MakeTransparent(Bitmap bmp, Color col, int delta)
    {
        // we expect a 32bpp bitmap!
        var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                                    ImageLockMode.ReadWrite, bmp.PixelFormat);

        long len = bmpData.Height * bmpData.Stride;
        byte[] data = new byte[len];
        Marshal.Copy(bmpData.Scan0, data, 0, data.Length);

        for (int i = 0; i < len; i += 4)
        {
            int dist = Math.Abs(data[i + 0] - col.B);
            dist += Math.Abs(data[i + 1] - col.G);
            dist += Math.Abs(data[i + 2] - col.R);

            if (dist <= delta) data[i + 3] = 0;
        }

        Marshal.Copy(data, 0, bmpData.Scan0, data.Length);

        bmp.UnlockBits(bmpData);

        return bmp;
    }

    /// <summary>
    /// Flips an image horizontally.
    /// </summary>
    /// <param name="src"></param>
    /// <returns></returns>
    internal static Bitmap FlipH(Bitmap src)
    {
        Bitmap dest = new(src.Width, src.Height);

        for (int x = 0; x < src.Width; x++)
        {
            for (int y = 0; y < src.Height; y++)
            {
                dest.SetPixel(x, y, src.GetPixel(src.Width - x - 1, y));
            }
        }

        return dest;
    }

    /// <summary>
    /// Flips an image vertically.
    /// </summary>
    /// <param name="src"></param>
    /// <returns></returns>
    internal static Bitmap FlipV(Bitmap src)
    {
        Bitmap dest = new(src.Width, src.Height);

        for (int x = 0; x < src.Width; x++)
        {
            for (int y = 0; y < src.Height; y++)
            {
                dest.SetPixel(x, y, src.GetPixel(x, src.Height - 1 - y));
            }
        }

        return dest;
    }

    /// <summary>
    /// Flips an image both horizontally and vertically. 
    /// </summary>
    /// <param name="src"></param>
    /// <returns></returns>
    internal static Bitmap FlipHV(Bitmap src)
    {
        Bitmap dest = new(src.Width, src.Height);

        for (int x = 0; x < src.Width; x++)
        {
            for (int y = 0; y < src.Height; y++)
            {
                dest.SetPixel(x, y, src.GetPixel(y, x));
            }
        }

        return dest;
    }

}