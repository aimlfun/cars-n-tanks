using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace CarsAndTanks.Utilities;

/// <summary>
/// Convert an ENUM into a string text representation
/// </summary>
static class EnumExtensions
{
    public static string? ToFriendlyString(this Enum code)
    {
        return Enum.GetName(code.GetType(), code);
    }
}

/// <summary>
/// GDI graphics extensions (as opposed to utilities).
/// </summary>
static class GraphicsExtensions
{
    /// <summary>
    /// Makes the graphics object best quality (slower, but looks better).
    /// </summary>
    /// <param name="graphics"></param>
    public static void ToHighQuality(this Graphics graphics)
    {
        if (graphics.InterpolationMode == InterpolationMode.HighQualityBicubic) return; // saves 5 assigns each call

        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
    }

    /// <summary>
    /// Make the graphics object low quality (faster, but looks horrible).
    /// </summary>
    /// <param name="graphics"></param>
    public static void ToLowQuality(this Graphics graphics)
    {
        if (graphics.InterpolationMode == InterpolationMode.Low) return; // saves 5 assigns each call

        graphics.InterpolationMode = InterpolationMode.Low;
        graphics.CompositingQuality = CompositingQuality.HighSpeed;
        graphics.SmoothingMode = SmoothingMode.HighSpeed;
        graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
        graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
    }

    /// <summary>
    /// Draw a thick arc with different inside and outside pens to both GDI canvas and Cairo interface.
    /// </summary>
    /// <param name="gr"></param>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    /// <param name="start_angle"></param>
    /// <param name="sweep_angle"></param>
    /// <param name="fill_pen"></param>
    /// <param name="outline_pen"></param>
    public static void DrawThickArc(this Graphics gr, PointF center, float radius, float start_angle, float sweep_angle, Pen fill_pen, Pen outline_pen)
    {
        // Draw a "thick" arc.
        gr.DrawArc(fill_pen, center.X - radius, center.Y - radius, 2 * radius, 2 * radius, start_angle, sweep_angle);

        // Draw a line following the outside outline of the arc.
        float r1 = radius + fill_pen.Width / 2f + outline_pen.Width / 2f; //- 0.5F 
        gr.DrawArc(outline_pen, center.X - r1, center.Y - r1, 2 * r1, 2 * r1, start_angle, sweep_angle);

        // Draw a line following the inside outline of the arc.
        float r2 = radius - fill_pen.Width / 2f - outline_pen.Width / 2f;
        gr.DrawArc(outline_pen, center.X - r2, center.Y - r2, 2 * r2, 2 * r2, start_angle, sweep_angle);
    }

    /// <summary>
    /// Returns a rounded rectangle path.
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="rectangle"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    private static GraphicsPath GenerateRoundedRectangle(this Graphics graphics, RectangleF rectangle, float radius)
    {
        if (radius >= Math.Min(rectangle.Width, rectangle.Height) / 2.0) return graphics.GenerateCapsule(rectangle);

        GraphicsPath path = new();

        if (radius <= 0.0F)
        {
            path.AddRectangle(rectangle);
        }
        else
        {
            float diameter = radius * 2.0F;

            SizeF sizeF = new(diameter, diameter);
            RectangleF arc = new(rectangle.Location, sizeF);

            path.AddArc(arc, 180, 90);
            arc.X = rectangle.Right - diameter;

            path.AddArc(arc, 270, 90);
            arc.Y = rectangle.Bottom - diameter;

            path.AddArc(arc, 0, 90);
            arc.X = rectangle.Left;

            path.AddArc(arc, 90, 90);
        }

        path.CloseFigure();

        return path;
    }

    /// <summary>
    /// Draw a rectangle in the indicated Rectangle rounding the indicated corners.
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="bottomPart"></param>
    /// <param name="xradius"></param>
    /// <param name="yradius"></param>
    /// <param name="round_ul"></param>
    /// <param name="round_ur"></param>
    /// <param name="round_lr"></param>
    /// <param name="round_ll"></param>
    /// <returns></returns>
    private static GraphicsPath MakeRoundedRect(RectangleF rect, float bottomPart,
                                                  float xradius, float yradius,
                                                  bool round_ul, bool round_ur, bool round_lr, bool round_ll)
    {
        // Make a GraphicsPath to draw the rectangle.
        PointF point1, point2;
        GraphicsPath path = new();

        // Upper left corner.
        if (round_ul)
        {
            RectangleF corner = new(rect.X, rect.Y, 2 * xradius, 2 * yradius);
            path.AddArc(corner, 180, 90);
            point1 = new PointF(rect.X + xradius, rect.Y);
        }
        else point1 = new PointF(rect.X, rect.Y + (rect.Height - bottomPart));

        // Top side.
        if (round_ur)
            point2 = new PointF(rect.Right - xradius, rect.Y);
        else
            point2 = new PointF(rect.Right, rect.Y + (rect.Height - bottomPart));

        path.AddLine(point1, point2);

        // Upper right corner.
        if (round_ur)
        {
            RectangleF corner = new(rect.Right - 2 * xradius, rect.Y, 2 * xradius, 2 * yradius);
            path.AddArc(corner, 270, 90);
            point1 = new PointF(rect.Right, rect.Y + yradius);
        }
        else point1 = new PointF(rect.Right, rect.Y);

        // Right side.
        if (round_lr)
            point2 = new PointF(rect.Right, rect.Bottom - yradius);
        else
            point2 = new PointF(rect.Right, rect.Bottom);

        path.AddLine(point1, point2);

        // Lower right corner.
        if (round_lr)
        {
            RectangleF corner = new(rect.Right - 2 * xradius, rect.Bottom - 2 * yradius, 2 * xradius, 2 * yradius);

            path.AddArc(corner, 0, 90);
            point1 = new PointF(rect.Right - xradius, rect.Bottom);
        }
        else point1 = new PointF(rect.Right, rect.Bottom);

        // Bottom side.
        if (round_ll)
            point2 = new PointF(rect.X + xradius, rect.Bottom);
        else
            point2 = new PointF(rect.X, rect.Bottom);

        path.AddLine(point1, point2);

        // Lower left corner.
        if (round_ll)
        {
            RectangleF corner = new(rect.X, rect.Bottom - 2 * yradius, 2 * xradius, 2 * yradius);
            path.AddArc(corner, 90, 90);
            point1 = new PointF(rect.X, rect.Bottom - yradius);
        }
        else point1 = new PointF(rect.X, rect.Bottom);

        // Left side.
        if (round_ul)
            point2 = new PointF(rect.X, rect.Y + yradius);
        else
            point2 = new PointF(rect.X, rect.Y);

        path.AddLine(point1, point2);

        // Join with the start point.
        path.CloseFigure();

        return path;
    }

    /// <summary>
    /// Returns a rounded rectangle path.
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="rectangle"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    private static GraphicsPath GenerateBottomHalfRoundedRectangle(this Graphics graphics, RectangleF rectangle, float bottomPart, float radius)
    {
        GraphicsPath path = MakeRoundedRect(rectangle, bottomPart, radius, radius, false, false, true, true);

        return path;
    }

    /// <summary>
    /// Draws a capsule shape.
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="baseRect"></param>
    /// <returns></returns>
    private static GraphicsPath GenerateCapsule(this Graphics graphics, RectangleF baseRect)
    {
        float diameter;
        RectangleF arc;
        GraphicsPath path = new();

        try
        {
            if (baseRect.Width > baseRect.Height)
            {
                diameter = baseRect.Height;
                SizeF sizeF = new(diameter, diameter);
                arc = new RectangleF(baseRect.Location, sizeF);
                path.AddArc(arc, 90, 180);
                arc.X = baseRect.Right - diameter;
                path.AddArc(arc, 270, 180);
            }
            else if (baseRect.Width < baseRect.Height)
            {
                diameter = baseRect.Width;
                SizeF sizeF = new(diameter, diameter);
                arc = new RectangleF(baseRect.Location, sizeF);
                path.AddArc(arc, 180, 180);
                arc.Y = baseRect.Bottom - diameter;
                path.AddArc(arc, 0, 180);
            }
            else path.AddEllipse(baseRect);
        }
        catch
        {
            path.AddEllipse(baseRect);
        }
        finally
        {
            path.CloseFigure();
        }

        return path;
    }

    /// <summary>
    /// Draws a rounded rectangle specified by a pair of coordinates, a width, a height and the radius 
    /// for the arcs that make the rounded edges.
    /// </summary>
    /// <param name="brush">System.Drawing.Pen that determines the color, width and style of the rectangle.</param>
    /// <param name="x">The x-coordinate of the upper-left corner of the rectangle to draw.</param>
    /// <param name="y">The y-coordinate of the upper-left corner of the rectangle to draw.</param>
    /// <param name="width">Width of the rectangle to draw.</param>
    /// <param name="height">Height of the rectangle to draw.</param>
    /// <param name="radius">The radius of the arc used for the rounded edges.</param>
    public static void DrawRoundedRectangle(
        this Graphics graphics,
        Pen pen,
        float x, float y,
        float width, float height,
        float radius)
    {
        RectangleF rectangle = new(x, y, width, height);

        using GraphicsPath path = graphics.GenerateRoundedRectangle(rectangle, radius);
        SmoothingMode old = graphics.SmoothingMode; // preserve smoothing mode

        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.DrawPath(pen, path);

        graphics.SmoothingMode = old; // restore smoothing mode
    }

    /// <summary>
    /// Draws a rounded rectangle specified by a pair of coordinates, a width, a height and the radius 
    /// for the arcs that make the rounded edges.
    /// </summary>
    /// <param name="brush">System.Drawing.Pen that determines the color, width and style of the rectangle.</param>
    /// <param name="x">The x-coordinate of the upper-left corner of the rectangle to draw.</param>
    /// <param name="y">The y-coordinate of the upper-left corner of the rectangle to draw.</param>
    /// <param name="width">Width of the rectangle to draw.</param>
    /// <param name="height">Height of the rectangle to draw.</param>
    /// <param name="radius">The radius of the arc used for the rounded edges.</param>
    public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, int x, int y, int width, int height, int radius)
    {
        graphics.DrawRoundedRectangle(
            pen,
            Convert.ToSingle(x), Convert.ToSingle(y),
            Convert.ToSingle(width), Convert.ToSingle(height),
            Convert.ToSingle(radius));
    }

    /// <summary>
    /// Fills the interior of a rounded rectangle specified by a pair of coordinates, a width, a height
    /// and the radius for the arcs that make the rounded edges.
    /// </summary>
    /// <param name="brush">System.Drawing.Brush that determines the characteristics of the fill.</param>
    /// <param name="x">The x-coordinate of the upper-left corner of the rectangle to fill.</param>
    /// <param name="y">The y-coordinate of the upper-left corner of the rectangle to fill.</param>
    /// <param name="width">Width of the rectangle to fill.</param>
    /// <param name="height">Height of the rectangle to fill.</param>
    /// <param name="radius">The radius of the arc used for the rounded edges.</param>
    public static void FillRoundedRectangle(this Graphics graphics, Brush brush, float x, float y, float width, float height, float radius)
    {
        RectangleF rectangle = new(x, y, width, height);

        using GraphicsPath path = graphics.GenerateRoundedRectangle(rectangle, radius);
        SmoothingMode old = graphics.SmoothingMode;

        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.FillPath(brush, path);

        graphics.SmoothingMode = old;
    }

    /// <summary>
    /// Fills the interior of a rounded rectangle specified by a pair of coordinates, a width, a height
    /// and the radius for the arcs that make the rounded edges.
    /// </summary>
    /// <param name="brush">System.Drawing.Brush that determines the characteristics of the fill.</param>
    /// <param name="x">The x-coordinate of the upper-left corner of the rectangle to fill.</param>
    /// <param name="y">The y-coordinate of the upper-left corner of the rectangle to fill.</param>
    /// <param name="width">Width of the rectangle to fill.</param>
    /// <param name="height">Height of the rectangle to fill.</param>
    /// <param name="radius">The radius of the arc used for the rounded edges.</param>
    public static void FillBottomHalfRoundedRectangle(this Graphics graphics, Brush brush, float x, float y, float width, float height, float bottomPart, float radius)
    {
        RectangleF rectangle = new(x, y, width, height);

        using GraphicsPath path = graphics.GenerateBottomHalfRoundedRectangle(rectangle, bottomPart, radius);
        SmoothingMode old = graphics.SmoothingMode;

        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.FillPath(brush, path);

        graphics.SmoothingMode = old;
    }

    /// <summary>
    /// Fills the interior of a rounded rectangle specified by a pair of coordinates, a width, a height
    /// and the radius for the arcs that make the rounded edges.
    /// </summary>
    /// <param name="brush">System.Drawing.Brush that determines the characteristics of the fill.</param>
    /// <param name="x">The x-coordinate of the upper-left corner of the rectangle to fill.</param>
    /// <param name="y">The y-coordinate of the upper-left corner of the rectangle to fill.</param>
    /// <param name="width">Width of the rectangle to fill.</param>
    /// <param name="height">Height of the rectangle to fill.</param>
    /// <param name="radius">The radius of the arc used for the rounded edges.</param>
    public static void FillBottomHalfRoundedRectangle(this Graphics graphics, Brush brush, Pen pen, float x, float y, float width, float height, float bottomPart, float radius)
    {
        RectangleF rectangle = new(x, y, width, height);

        using GraphicsPath path = graphics.GenerateBottomHalfRoundedRectangle(rectangle, bottomPart, radius);
        SmoothingMode old = graphics.SmoothingMode;

        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.FillPath(brush, path);
        graphics.DrawPath(pen, path);

        graphics.SmoothingMode = old;
    }

    /// <summary>
    /// Fills the interior of a rounded rectangle specified by a pair of coordinates, a width, a height
    /// and the radius for the arcs that make the rounded edges.
    /// </summary>
    /// <param name="brush">System.Drawing.Brush that determines the characteristics of the fill.</param>
    /// <param name="x">The x-coordinate of the upper-left corner of the rectangle to fill.</param>
    /// <param name="y">The y-coordinate of the upper-left corner of the rectangle to fill.</param>
    /// <param name="width">Width of the rectangle to fill.</param>
    /// <param name="height">Height of the rectangle to fill.</param>
    /// <param name="radius">The radius of the arc used for the rounded edges.</param>
    public static void FillRoundedRectangle(
        this Graphics graphics,
        Brush brush,
        int x, int y,
        int width, int height,
        int radius)
    {
        graphics.FillRoundedRectangle(brush,
                                      Convert.ToSingle(x),
                                      Convert.ToSingle(y),
                                      Convert.ToSingle(width),
                                      Convert.ToSingle(height),
                                      Convert.ToSingle(radius));
    }

    /// <summary>
    /// Fills the interior of a rounded rectangle specified by a pair of coordinates, a width, a height
    /// and the radius for the arcs that make the rounded edges.
    /// </summary>
    /// <param name="brush">System.Drawing.Brush that determines the characteristics of the fill.</param>
    /// <param name="x">The x-coordinate of the upper-left corner of the rectangle to fill.</param>
    /// <param name="y">The y-coordinate of the upper-left corner of the rectangle to fill.</param>
    /// <param name="width">Width of the rectangle to fill.</param>
    /// <param name="height">Height of the rectangle to fill.</param>
    /// <param name="radius">The radius of the arc used for the rounded edges.</param>
    public static void FillBottomHalfRoundedRectangle(
        this Graphics graphics,
        Brush brush,
        int x, int y,
        int width, int height,
        int bottomHalf,
        int radius)
    {
        graphics.FillBottomHalfRoundedRectangle(brush,
                                      Convert.ToSingle(x), Convert.ToSingle(y),
                                      Convert.ToSingle(width), Convert.ToSingle(height),
                                      Convert.ToSingle(bottomHalf),
                                      Convert.ToSingle(radius));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="graphics"></param>
    /// <param name="brush"></param>
    /// <param name="p"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="bottomHalf"></param>
    /// <param name="radius"></param>
    public static void FillBottomHalfRoundedRectangle(
        this Graphics graphics,
        Brush brush,
        Pen p,
        int x, int y,
        int width, int height,
        int bottomHalf,
        int radius)
    {
        graphics.FillBottomHalfRoundedRectangle(brush,
                                      p,
                                      Convert.ToSingle(x),
                                      Convert.ToSingle(y),
                                      Convert.ToSingle(width),
                                      Convert.ToSingle(height),
                                      Convert.ToSingle(bottomHalf),
                                      Convert.ToSingle(radius));
    }

}

static class StringExtensions
{
    /// <summary>
    /// Pads both left and right evenly (centering the text).
    /// </summary>
    /// <param name="str">String to pad.</param>
    /// <param name="chars">Which character to pad.</param>
    /// <param name="length">The length required.</param>
    /// <returns></returns>
    public static string PadBoth(this string str, char chars, int length)
    {
        int spaces = length - str.Length;
        int padLeft = spaces / 2 + str.Length;

        return str.PadLeft(padLeft, chars).PadRight(length, chars);
    }
}
