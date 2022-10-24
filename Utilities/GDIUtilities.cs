using System.Drawing.Drawing2D;

namespace AICarTrack
{
    /// <summary>
    /// GDI (Graphics) Utility Functions.
    /// </summary>
    public static class GDIUtilities
    {

        /// <summary>
        /// Returns a rounded rectange graphics path.
        /// </summary>
        /// <param name="bounds"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new(diameter, diameter);
            Rectangle arc = new(bounds.Location, size);
            GraphicsPath path = new();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        /// <summary>
        /// Draws a rounded rectangle.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="pen"></param>
        /// <param name="bounds"></param>
        /// <param name="cornerRadius"></param>
        public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, Rectangle bounds, int cornerRadius)
        {
            if (graphics == null)
                throw new ArgumentNullException(nameof(graphics));

            if (pen == null)
                throw new ArgumentNullException(nameof(pen));

            using GraphicsPath path = RoundedRect(bounds, cornerRadius);
            graphics.DrawPath(pen, path);
        }

        /// <summary>
        /// Draws a filled it rounded rectangle.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="brush"></param>
        /// <param name="bounds"></param>
        /// <param name="cornerRadius"></param>
        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle bounds, int cornerRadius)
        {
            if (graphics == null)
                throw new ArgumentNullException(nameof(graphics));

            if (brush == null)
                throw new ArgumentNullException(nameof(brush));

            using GraphicsPath path = RoundedRect(bounds, cornerRadius);

            graphics.FillPath(brush, path);
        }
    }
}