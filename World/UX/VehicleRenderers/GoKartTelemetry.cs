using System.Drawing.Drawing2D;
using CarsAndTanks.Utilities;
using CarsAndTanks.World.Car;

namespace CarsAndTanks.World.UX.VehicleRenderers;

internal static class GoKartTelemetry
{
    /// <summary>
    /// Cache the GoKart so we can add the cog and front wheels
    /// </summary>
    private static Bitmap? backgroundVehicle;

    /// <summary>
    /// Cache the width of the container we are rendering into.
    /// </summary>
    private static int s_width = 0;

    /// <summary>
    /// Cache the height of the container we are rendering into.
    /// </summary>
    private static int s_height = 0;

    /// <summary>
    /// 
    /// </summary>
    static double frontAxle, rearAxle;

    /// <summary>
    /// 
    /// </summary>
    static Point tyreFrontLeft = new();

    /// <summary>
    /// 
    /// </summary>
    static Point tyreFrontRight = new();

    /// <summary>
    /// 
    /// </summary>
    static Point tyreRearLeft = new();

    /// <summary>
    /// 
    /// </summary>
    static Point tyreRearRight = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="car"></param>
    internal static Bitmap ShowGoKartSteeringAndSlide(VehicleDrivenByAI car, int width, int height)
    {
        if (backgroundVehicle is null)
        {
            s_width = width;
            s_height = height;
            backgroundVehicle = DrawTelemetryCar(car);
        }

        Bitmap b = new(backgroundVehicle);

        CarUsingRealPhysics data = (CarUsingRealPhysics)car.CarImplementation;
        Graphics g = Graphics.FromImage(b);
        g.ToHighQuality();

        int y = 30;

        const float carHeight = 100F * (float)CarConfig.goKartLengthMetres;

        int centreX = b.Width / 2;

        // height includes total of front to rear cog's
        double f2rcog = data.Config.centreOfGravityToFront + data.Config.centreOfGravityToRear;
        double yunits = carHeight / f2rcog;
        double cog = y + yunits * data.Config.centreOfGravityToFront;

        DrawFrontTyres(data, g);
        DrawSidewaysSlip(data, g, centreX);
        DrawCOGandShiftOfCOG(data, g, y, carHeight, centreX, cog);

        return b;
    }

    /// <summary>
    /// Draws a cart with moving heels.
    /// </summary>
    /// <param name="car"></param>
    /// <returns></returns>
    private static Bitmap DrawTelemetryCar(VehicleDrivenByAI car)
    {
        Bitmap b = new(s_width, s_height);

        CarUsingRealPhysics data = (CarUsingRealPhysics)car.CarImplementation;

        using Graphics g = Graphics.FromImage(b);
        g.ToHighQuality();

        int y = 30;

        const float carWidth = 100F * (float)CarConfig.goKartWidthMetres;
        const float carHeight = 100F * (float)CarConfig.goKartLengthMetres;
        int centreX = b.Width / 2;

        // height includes total of fron to rear cog's
        double f2rcog = data.Config.centreOfGravityToFront + data.Config.centreOfGravityToRear;
        double yunits = carHeight / f2rcog;
        double cog = y + yunits * data.Config.centreOfGravityToFront;

        frontAxle = cog - yunits * data.Config.centreOfGravityToFrontAxle;
        rearAxle = cog + yunits * data.Config.centreOfGravityToRearAxle;

        tyreFrontLeft = new Point(10 + (int)(centreX - carWidth / 2) - 8, (int)frontAxle);
        tyreFrontRight = new Point(-10 + (int)(centreX + carWidth / 2) + 5, (int)frontAxle);
        tyreRearLeft = new Point(8 + (int)(centreX - carWidth / 2) - 8, (int)rearAxle);
        tyreRearRight = new Point(-8 + (int)(centreX + carWidth / 2) + 8, (int)rearAxle);


        // box for car
        Rectangle carRect = new(centreX - (int)carWidth / 2, y, (int)carWidth, (int)carHeight);

        using Pen blackPen = new(Color.Black);

        DrawFrontSpoiler(g, carWidth, centreX, carHeight, y);
        DrawSides(g, carWidth, carRect.X, carHeight, y);
        DrawTubing(g, carWidth, carRect.X, carHeight, y);

        DrawAxles(data, g, carWidth, centreX, yunits, cog);
        DrawRearTyres(g);
        DrawSeat(g, carWidth, carRect.X, carHeight, y);
        DrawSteeringWheel(g, carWidth, carRect.X, carHeight, y);
        DrawExhaust(g, carWidth, carRect.X, carHeight, y);

        return b;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="g"></param>
    /// <param name="carWidth"></param>
    /// <param name="centreX"></param>
    /// <param name="carHeight"></param>
    /// <param name="y"></param>
    private static void DrawExhaust(Graphics g, float carWidth, int centreX, float carHeight, int y)
    {
        float ratioX = carHeight / 1133F;
        float ratioY = carWidth / 547F;
        float left = centreX - 12;

        g.DrawLine(new Pen(Color.Silver, 4),
                        left + 145 * ratioX, y + 1161 * ratioY,
                        left + 490 * ratioX, y + 911 * ratioY);


        g.DrawLine(new Pen(Color.Silver, 4),
                        left + 490 * ratioX, y + 875 * ratioY,
                        left + 490 * ratioX, y + 911 * ratioY);

        g.DrawLine(new Pen(Color.DarkGray, 10),
                        left + 165 * ratioX, y + 1139 * ratioY,
                        left + 395 * ratioX, y + 975 * ratioY);

        g.FillRoundedRectangle(new SolidBrush(Color.FromArgb(30, 30, 30)), left + 487 * ratioX, y + 473 * ratioY, 90 * ratioX, 280 * ratioY, 8);

        g.FillRectangle(new SolidBrush(Color.FromArgb(255, 172, 23, 19)), new RectangleF(left + 547 * ratioX, y + 743 * ratioY, 40 * ratioX, 250 * ratioY));

        g.FillRectangle(new SolidBrush(Color.FromArgb(255, 172, 23, 19)), new RectangleF(left + 447 * ratioX, y + 733 * ratioY, 100 * ratioX, 140 * ratioY));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="g"></param>
    /// <param name="carWidth"></param>
    /// <param name="centreX"></param>
    /// <param name="carHeight"></param>
    /// <param name="y"></param>
    private static void DrawSteeringWheel(Graphics g, float carWidth, int centreX, float carHeight, int y)
    {
        // 393-285, 228, 176r
        float ratioX = carHeight / 1133F;
        float ratioY = carWidth / 547F;
        g.DrawEllipse(new Pen(Color.Silver, 3), centreX + carWidth / 2 - 114 * ratioX, y + (533 - 88) * ratioY, 228 * ratioX, 176 * ratioY);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="g"></param>
    /// <param name="carWidth"></param>
    /// <param name="centreX"></param>
    /// <param name="carHeight"></param>
    /// <param name="y"></param>
    private static void DrawSeat(Graphics g, float carWidth, int centreX, float carHeight, int y)
    {
        float ratioX = carHeight / 1133F;
        float ratioY = carWidth / 547F;
        float left = centreX - 12;
        float right = centreX + 12 + carWidth;

        PointF[] seat = { new PointF(left+295*ratioX, y+481*ratioY),
                          new PointF(left+247*ratioX, y+507*ratioY),
                            new PointF(left+249*ratioX, y+613*ratioY),
                            new PointF(left+273*ratioX, y+735*ratioY),
                            new PointF(left+271*ratioX, y+889*ratioY),
                            new PointF(left+293*ratioX, y+921*ratioY),
                            new PointF(left+385*ratioX, y+939*ratioY),

                            new PointF(right-385*ratioX, y+939*ratioY),
                            new PointF(right-293*ratioX, y+921*ratioY),
                            new PointF(right-271*ratioX, y+889*ratioY),
                            new PointF(right-273*ratioX, y+735*ratioY),
                            new PointF(right-249*ratioX, y+613*ratioY),
                            new PointF(right-247*ratioX, y+507*ratioY),
                            new PointF(right-295*ratioX, y+481*ratioY),
                            new PointF(left+295*ratioX, y+481*ratioY)
        };

        PointF[] seatBottom = { new PointF(left+269*ratioX,y+545*ratioY),
                                new PointF(left+293*ratioX, y+673*ratioY),
                                new PointF(left+299*ratioX, y+747*ratioY),

                                new PointF(right-299*ratioX, y+747*ratioY),
                                new PointF(right-293*ratioX, y+673*ratioY),
                                new PointF(right-269*ratioX,y+545*ratioY),
                                new PointF(left+269*ratioX,y+545*ratioY) };

        g.FillPolygon(new SolidBrush(Color.FromArgb(240, 0, 0, 0)), seat);

        g.FillPolygon(new SolidBrush(Color.FromArgb(180, 90, 90, 90)), seatBottom);
    }

    /// <summary>
    /// Draw the tubing structure of the cart.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="carWidth"></param>
    /// <param name="centreX"></param>
    /// <param name="carHeight"></param>
    /// <param name="y"></param>
    private static void DrawTubing(Graphics g, float carWidth, int centreX, float carHeight, int y)
    {
        float ratioX = carHeight / 1133F;
        float ratioY = carWidth / 547F;
        float left = centreX - 13;
        float right = centreX + 13 + carWidth;

        // box under the front
        g.DrawRoundedRectangle(new Pen(Color.Gray, 2), left + 300 * ratioX, y + 183 * ratioY, 117 * ratioX, 117 * ratioY, 3);

        // triangle panel at front
        PointF[] panel =
        {
            new PointF(left+385*ratioX-5, y+161*ratioY),
            new PointF(left+355*ratioX-5, y+271*ratioY),
            new PointF(left+329*ratioX-5, y+272*ratioY),
            new PointF(left+323*ratioX-5, y+343*ratioY),
            new PointF(left+409*ratioX-5, y+339*ratioY),

            new PointF(right-409*ratioX+5, y+339*ratioY),
            new PointF(right-323*ratioX+5, y+343*ratioY),
            new PointF(right-329*ratioX+5, y+272*ratioY),
            new PointF(right-355*ratioX+5, y+271*ratioY),
            new PointF(right-385*ratioX+5, y+161*ratioY),
            new PointF(left+385*ratioX-5, y+161*ratioY)
        };

        g.FillPolygon(new SolidBrush(Color.Black), panel);

        PointF[] points = { new PointF(left+279*ratioX, y+103*ratioY),
                            new PointF(left+223*ratioX, y+287*ratioY),
                            new PointF(left+335*ratioX, y+411*ratioY),
                            new PointF(left+321*ratioX, y+467*ratioY),
                            new PointF(left+195*ratioX, y+641*ratioY),
                            new PointF(left+193*ratioX, y+1053*ratioY),
                            new PointF(left+247*ratioX, y+1135*ratioY),

                            new PointF(right-247*ratioX, y+1135*ratioY),
                            new PointF(right-193*ratioX, y+1053*ratioY),
                            new PointF(right-195*ratioX, y+641*ratioY),
                            new PointF(right-321*ratioX, y+467*ratioY),
                            new PointF(right-335*ratioX, y+411*ratioY),
                            new PointF(right-223*ratioX, y+287*ratioY),
                            new PointF(right-279*ratioX, y+103*ratioY),
                            new PointF(left+279*ratioX, y+103*ratioY)
        };


        PointF[] points2 = { new PointF(left+291 * ratioX, y+363 * ratioY),
                             new PointF(left+145 * ratioX, y+559 * ratioY),
                             new PointF(left+129 * ratioX, y+785 * ratioY),
                             new PointF(left+249 * ratioX, y+811 * ratioY),
                             new PointF(right-249 * ratioX, y+811 * ratioY),
                             new PointF(right-129 * ratioX, y+785 * ratioY),
                             new PointF(right-145 * ratioX, y+559 * ratioY),
                             new PointF(right-291 * ratioX, y+363 * ratioY)

        };

        PointF[] points3 = { new PointF(left+203 * ratioX, y+293* ratioY),
                             new PointF(left + 299* ratioX,y+191  * ratioY),
                             new PointF(left + 329* ratioX,y+177 * ratioY),

                             new PointF(right- 329* ratioX,y+177 * ratioY),
                             new PointF(right- 299* ratioX,y+191  * ratioY),
                             new PointF(right-203 * ratioX, y+293* ratioY)
        };


        Pen tubingPen = new(Color.FromArgb(200, 50, 50, 255), 4);
        g.DrawLines(tubingPen, points);
        g.DrawLine(tubingPen, left + 180 * ratioX, y + 296 * ratioX, left + 244 * ratioX, y + 299 * ratioX);
        g.DrawLine(tubingPen, right - 180 * ratioX, y + 296 * ratioX, right - 244 * ratioX, y + 299 * ratioX);

        tubingPen = new Pen(Color.FromArgb(160, 0, 0, 255), 2);
        g.DrawLines(tubingPen, points);
        g.DrawLine(tubingPen, left + 180 * ratioX, y + 296 * ratioX, left + 244 * ratioX, y + 299 * ratioX);
        g.DrawLine(tubingPen, right - 180 * ratioX, y + 296 * ratioX, right - 244 * ratioX, y + 299 * ratioX);

        tubingPen.Width = 3;

        g.DrawLines(tubingPen, points2);
        g.DrawLines(tubingPen, points3);

    }

    /// <summary>
    /// Draws the side of the car.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="carWidth"></param>
    /// <param name="centreX"></param>
    /// <param name="carHeight"></param>
    /// <param name="y"></param>
    private static void DrawSides(Graphics g, float carWidth, int centreX, float carHeight, int y)
    {
        float ratioX = carHeight / 1133F;
        float ratioY = carWidth / 547F;
        float left = centreX - 12;
        float right = centreX + 12 + carWidth;

        PointF[] leftSide = { new PointF(left+70 * ratioX, y+410*ratioY-1),
                              new PointF(left+3 * ratioX, y+843*ratioY-1),
                              new PointF(left+109 * ratioX, y+855*ratioY-1),
                              new PointF(left+131 * ratioX, y+419*ratioY-1),
                              new PointF(left+70 * ratioX, y+424*ratioY-1),
                            };

        PointF[] rightSide = { new PointF(right-70 * ratioX, y+410*ratioY-1),
                               new PointF(right-3 * ratioX, y+843*ratioY-1),
                               new PointF(right-109 * ratioX, y+855*ratioY-1),
                               new PointF(right-131 * ratioX, y+419*ratioY-1),
                               new PointF(right-70 * ratioX, y+424*ratioY-1),
                             };

        using SolidBrush sideBrush = new(Color.FromArgb(255, 255, 10, 10));

        g.FillPolygon(sideBrush, leftSide);
        g.FillPolygon(sideBrush, rightSide);

        using Pen p = new(Color.FromArgb(100, 245, 245, 245), 2);
        g.DrawLine(p, new PointF(left + 1 + 70 * ratioX, y + 410 * ratioY - 1),
                      new PointF(left + 1 + 3 * ratioX, y + 843 * ratioY - 1));

        g.DrawLine(p, new PointF(1 + right - 109 * ratioX, y + 855 * ratioY - 1),
                      new PointF(1 + right - 131 * ratioX, y + 419 * ratioY - 1));

        using Pen p2 = new(Color.FromArgb(100, 20, 20, 20), 2);

        g.DrawLine(p2, new PointF(left + -1 + 109 * ratioX, y + 855 * ratioY - 1),
                       new PointF(left + 131 * ratioX, y + 419 * ratioY - 1));

        g.DrawLine(p2, new PointF(right - 1 - 70 * ratioX, y + 410 * ratioY - 1),
                       new PointF(right - 1 - 3 * ratioX, y + 843 * ratioY - 1));
    }

    /// <summary>
    /// Draw the front spoiler.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="carWidth"></param>
    /// <param name="centreX"></param>
    /// <param name="carHeight"></param>
    /// <param name="y"></param>
    private static void DrawFrontSpoiler(Graphics g, float carWidth, int centreX, float carHeight, float y)
    {
        float ratioX = carHeight / 1133F;
        float ratioY = carWidth / 547F;

        PointF[] spoiler = { new PointF(centreX, y+15*ratioY),
                             new PointF(centreX-146 * ratioX, y+15*ratioY),
                             new PointF(centreX-278 * ratioX, y+127*ratioY),
                             new PointF(centreX-248 * ratioX, y+151*ratioY),
                             new PointF(centreX-190 * ratioX, y+159*ratioY),
                             new PointF(centreX-124 * ratioX, y+89*ratioY),
                             new PointF(centreX, y+89*ratioY),

                             new PointF(centreX+124 * ratioX, y+89*ratioY),
                             new PointF(centreX+190 * ratioX, y+159*ratioY),
                             new PointF(centreX+248 * ratioX, y+151*ratioY),
                             new PointF(centreX+278 * ratioX, y+127*ratioY),
                             new PointF(centreX+146 * ratioX, y+15*ratioY),
                             new PointF(centreX, y+15*ratioY)
        };

        using SolidBrush sideBrush = new(Color.FromArgb(220, 255, 10, 10));
        g.FillPolygon(sideBrush, spoiler);
    }

    /// <summary>
    /// Draw a blob showing the shift of centre-of-gravity.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="g"></param>
    /// <param name="y"></param>
    /// <param name="carHeight"></param>
    /// <param name="centreX"></param>
    /// <param name="cog"></param>
    private static void DrawCOGandShiftOfCOG(CarUsingRealPhysics data, Graphics g, int y, float carHeight, int centreX, double cog)
    {
        double frontAxleMass = data.frontAxleWeightInNewtons / data.Config.c_gravity;
        double frontAxleRatio = frontAxleMass / data.Config.massOfKartPlusDriverKg;

        double cogShift = (y + frontAxleRatio * carHeight).Clamp(y, y + carHeight) * 0.5f;

        using Pen cogPen = new(Color.LightGreen);

        // draw center of gravity circle
        g.DrawEllipse(cogPen, centreX - 3, (int)cog - 3, 6, 6);
        g.DrawLine(cogPen, centreX, (int)cog, centreX, (int)cogShift);

        using Pen redPen = new(Color.Red);

        // draw shifted centre of gravity circle
        g.FillEllipse(new SolidBrush(Color.Red), centreX - 3, (int)cogShift - 3, 6, 6);
    }

    /// <summary>
    /// Draws the axles. 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="g"></param>
    /// <param name="carWidth"></param>
    /// <param name="centreX"></param>
    /// <param name="yunits"></param>
    /// <param name="cog"></param>
    /// <param name="tyreFrontLeft"></param>
    /// <param name="tyreFrontRight"></param>
    /// <param name="tyreRearLeft"></param>
    /// <param name="tyreRearRight"></param>
    private static void DrawAxles(CarUsingRealPhysics data, Graphics g, float carWidth, int centreX, double yunits, double cog)
    {
        double frontAxle = cog - yunits * data.Config.centreOfGravityToFrontAxle;
        double rearAxle = cog + yunits * data.Config.centreOfGravityToRearAxle;

        tyreFrontLeft = new Point(10 + (int)(centreX - carWidth / 2) - 8, (int)frontAxle);
        tyreFrontRight = new Point(-10 + (int)(centreX + carWidth / 2) + 5, (int)frontAxle);
        tyreRearLeft = new Point(8 + (int)(centreX - carWidth / 2) - 8, (int)rearAxle);
        tyreRearRight = new Point(-8 + (int)(centreX + carWidth / 2) + 8, (int)rearAxle);

        using Pen pen = new(Color.Black, 3);

        g.DrawLine(pen, tyreRearLeft, tyreRearRight);
    }

    /// <summary>
    /// Draw the tyes
    /// </summary>
    /// <param name="data"></param>
    /// <param name="g"></param>
    /// <param name="tyreFrontLeft"></param>
    /// <param name="tyreFrontRight"></param>
    /// <param name="tyreRearLeft"></param>
    /// <param name="tyreRearRight"></param>
    private static void DrawFrontTyres(CarUsingRealPhysics data, Graphics g)
    {
        // rotate for frontWheelSteerAngle
        double angleInRadians = data.frontWheelSteerAngle / 2;

        if (angleInRadians < 0) angleInRadians += 2 * Math.PI; else if (angleInRadians > 2 * Math.PI) angleInRadians -= 2 * Math.PI;

        // left

        PointF pTopLeftFrontTyreLEFT = MathUtils.RotatePointAboutOriginInRadians(new PointF(tyreFrontLeft.X - 3, tyreFrontLeft.Y - 8),
                                                                    new PointF(tyreFrontLeft.X, tyreFrontLeft.Y),
                                                                    angleInRadians);

        PointF pTopRightFrontTyreLEFT = MathUtils.RotatePointAboutOriginInRadians(new PointF(tyreFrontLeft.X - 3 + 10, tyreFrontLeft.Y - 8),
                                                                     new PointF(tyreFrontLeft.X, tyreFrontLeft.Y),
                                                                     angleInRadians);

        PointF pBottomRightFrontTyreLEFT = MathUtils.RotatePointAboutOriginInRadians(new PointF(tyreFrontLeft.X - 3 + 10, tyreFrontLeft.Y - 8 + 16),
                                                                        new PointF(tyreFrontLeft.X, tyreFrontLeft.Y),
                                                                        angleInRadians);

        PointF pBottomLeftFrontTyreLEFT = MathUtils.RotatePointAboutOriginInRadians(new PointF(tyreFrontLeft.X - 3, tyreFrontLeft.Y - 8 + 16),
                                                                       new PointF(tyreFrontLeft.X, tyreFrontLeft.Y),
                                                                       angleInRadians);

        // right

        PointF pTopLeftFrontTyreRIGHT = MathUtils.RotatePointAboutOriginInRadians(new PointF(tyreFrontRight.X - 3, tyreFrontRight.Y - 8),
                                                                    new PointF(tyreFrontRight.X, tyreFrontRight.Y),
                                                                    angleInRadians);

        PointF pTopRightFrontTyreRIGHT = MathUtils.RotatePointAboutOriginInRadians(new PointF(tyreFrontRight.X - 3 + 10, tyreFrontRight.Y - 8),
                                                                     new PointF(tyreFrontRight.X, tyreFrontRight.Y),
                                                                     angleInRadians);

        PointF pBottomRightFrontTyreRIGHT = MathUtils.RotatePointAboutOriginInRadians(new PointF(tyreFrontRight.X - 3 + 10, tyreFrontRight.Y - 8 + 16),
                                                                        new PointF(tyreFrontRight.X, tyreFrontRight.Y),
                                                                        angleInRadians);

        PointF pBottomLeftFrontTyreRIGHT = MathUtils.RotatePointAboutOriginInRadians(new PointF(tyreFrontRight.X - 3, tyreFrontRight.Y - 8 + 16),
                                                                       new PointF(tyreFrontRight.X, tyreFrontRight.Y),
                                                                       angleInRadians);

        // to -- x1
        //     \
        //      x2
        PointF pLeftWheelIsAnchoredTo = new(tyreFrontLeft.X + 15, tyreFrontLeft.Y);
        PointF pLeftWheelIsAnchoredTo90x1 = new(tyreFrontLeft.X + 10, tyreFrontLeft.Y);
        PointF pLeftWheelIsAnchoredTo90x2 = new(tyreFrontLeft.X + 13, tyreFrontLeft.Y + 11);

        // x1 -- to
        //     /
        //    x2
        PointF pRightWheelIsAnchoredTo = new(tyreFrontRight.X - 10, tyreFrontRight.Y);
        PointF pRightWheelIsAnchoredTo90x1 = new(tyreFrontRight.X - 13 + 7, tyreFrontRight.Y);
        PointF pRightWheelIsAnchoredTo90x2 = new(tyreFrontRight.X - 13 + 4, tyreFrontRight.Y + 11);


        // if wheel is rotated, then anchor rotates
        PointF pPerpendicularToLeftWheel = MathUtils.RotatePointAboutOriginInRadians(pLeftWheelIsAnchoredTo,
                                                                                      new PointF(tyreFrontLeft.X, tyreFrontLeft.Y),
                                                                                      data.frontWheelSteerAngle + 2 * Math.PI);

        PointF pPerpendicularToRightWheel = MathUtils.RotatePointAboutOriginInRadians(pRightWheelIsAnchoredTo,
                                                                                       new PointF(tyreFrontRight.X, tyreFrontRight.Y),
                                                                                       data.frontWheelSteerAngle + 2 * Math.PI);

        //
        PointF pPerpendicularToLeftWheel90x1 = MathUtils.RotatePointAboutOriginInRadians(pLeftWheelIsAnchoredTo90x1,
                                                                                          new PointF(tyreFrontLeft.X, tyreFrontLeft.Y),
                                                                                          data.frontWheelSteerAngle + 2 * Math.PI);

        PointF pPerpendicularToLeftWheel90x2 = MathUtils.RotatePointAboutOriginInRadians(pLeftWheelIsAnchoredTo90x2,
                                                                                          new PointF(tyreFrontLeft.X, tyreFrontLeft.Y),
                                                                                          data.frontWheelSteerAngle + 2 * Math.PI);

        //
        PointF pPerpendicularToRightWheel90x1 = MathUtils.RotatePointAboutOriginInRadians(pRightWheelIsAnchoredTo90x1,
                                                                                           new PointF(tyreFrontRight.X, tyreFrontRight.Y),
                                                                                           data.frontWheelSteerAngle + 2 * Math.PI);

        PointF pPerpendicularToRightWheel90x2 = MathUtils.RotatePointAboutOriginInRadians(pRightWheelIsAnchoredTo90x2,
                                                                                           new PointF(tyreFrontRight.X, tyreFrontRight.Y),
                                                                                           data.frontWheelSteerAngle + 2 * Math.PI);


        PointF leftWheelConnectToPivotAdjustment = new(pPerpendicularToLeftWheel.X - pLeftWheelIsAnchoredTo.X,
                                          pPerpendicularToLeftWheel.Y - pLeftWheelIsAnchoredTo.Y);

        PointF rightWheelConnectToPivotAdjustment = new(pPerpendicularToRightWheel.X - pRightWheelIsAnchoredTo.X,
                                           pPerpendicularToRightWheel.Y - pRightWheelIsAnchoredTo.Y);

        PointF[] pointsForLeftWheel = {
                                   new PointF( pTopLeftFrontTyreLEFT.X-leftWheelConnectToPivotAdjustment.X,
                                               pTopLeftFrontTyreLEFT.Y-leftWheelConnectToPivotAdjustment.Y),
                                   new PointF( pTopRightFrontTyreLEFT.X-leftWheelConnectToPivotAdjustment.X,
                                               pTopRightFrontTyreLEFT.Y-leftWheelConnectToPivotAdjustment.Y),
                                   new PointF( pBottomRightFrontTyreLEFT.X-leftWheelConnectToPivotAdjustment.X,
                                               pBottomRightFrontTyreLEFT.Y-leftWheelConnectToPivotAdjustment.Y),
                                   new PointF( pBottomLeftFrontTyreLEFT.X-leftWheelConnectToPivotAdjustment.X,
                                               pBottomLeftFrontTyreLEFT.Y-leftWheelConnectToPivotAdjustment.Y)
                                      };

        PointF[] pointsForRightWheel = { new PointF(pTopLeftFrontTyreRIGHT.X -rightWheelConnectToPivotAdjustment.X,
                                                    pTopLeftFrontTyreRIGHT.Y-rightWheelConnectToPivotAdjustment.Y ),
                                         new PointF(pTopRightFrontTyreRIGHT.X -rightWheelConnectToPivotAdjustment.X,
                                                    pTopRightFrontTyreRIGHT.Y-rightWheelConnectToPivotAdjustment.Y),
                                         new PointF(pBottomRightFrontTyreRIGHT.X -rightWheelConnectToPivotAdjustment.X,
                                                    pBottomRightFrontTyreRIGHT.Y-rightWheelConnectToPivotAdjustment.Y),
                                         new PointF(pBottomLeftFrontTyreRIGHT.X -rightWheelConnectToPivotAdjustment.X,
                                                    pBottomLeftFrontTyreRIGHT.Y-rightWheelConnectToPivotAdjustment.Y),
                                         new PointF(pTopLeftFrontTyreRIGHT.X -rightWheelConnectToPivotAdjustment.X,
                                                    pTopLeftFrontTyreRIGHT.Y-rightWheelConnectToPivotAdjustment.Y)
                                       };

        using Brush blackBrush = new SolidBrush(Color.Black);

        // left front wheel link
        g.DrawLine(new Pen(Color.Silver, 3),
                    tyreFrontLeft.X - leftWheelConnectToPivotAdjustment.X,
                    tyreFrontLeft.Y - leftWheelConnectToPivotAdjustment.Y,
                    pPerpendicularToLeftWheel.X - leftWheelConnectToPivotAdjustment.X,
                    pPerpendicularToLeftWheel.Y - leftWheelConnectToPivotAdjustment.Y);

        // left front wheel
        g.FillPolygon(blackBrush, pointsForLeftWheel);

        using Pen linkagePen = new(Color.Silver, 3);

        // right front wheel link
        g.DrawLine(linkagePen,
                     tyreFrontRight.X - rightWheelConnectToPivotAdjustment.X,
                     tyreFrontRight.Y - rightWheelConnectToPivotAdjustment.Y,
                     pPerpendicularToRightWheel.X - rightWheelConnectToPivotAdjustment.X,
                     pPerpendicularToRightWheel.Y - rightWheelConnectToPivotAdjustment.Y);


        g.DrawLine(linkagePen, pPerpendicularToLeftWheel90x1.X - leftWheelConnectToPivotAdjustment.X,
                              pPerpendicularToLeftWheel90x1.Y - leftWheelConnectToPivotAdjustment.Y,
                              pPerpendicularToLeftWheel90x2.X - leftWheelConnectToPivotAdjustment.X,
                              pPerpendicularToLeftWheel90x2.Y - leftWheelConnectToPivotAdjustment.Y);

        g.DrawLine(linkagePen, pPerpendicularToRightWheel90x2.X - rightWheelConnectToPivotAdjustment.X,
                               pPerpendicularToRightWheel90x2.Y - rightWheelConnectToPivotAdjustment.Y,
                               pPerpendicularToRightWheel90x1.X - rightWheelConnectToPivotAdjustment.X,
                               pPerpendicularToRightWheel90x1.Y - rightWheelConnectToPivotAdjustment.Y);

        linkagePen.Width = 1;
        g.DrawLine(linkagePen, pPerpendicularToLeftWheel90x2.X - leftWheelConnectToPivotAdjustment.X,
                               pPerpendicularToLeftWheel90x2.Y - leftWheelConnectToPivotAdjustment.Y,
                               pPerpendicularToRightWheel90x2.X - rightWheelConnectToPivotAdjustment.X,
                               pPerpendicularToRightWheel90x2.Y - rightWheelConnectToPivotAdjustment.Y);


        // right front wheel 
        g.FillPolygon(blackBrush, pointsForRightWheel);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="g"></param>
    private static void DrawRearTyres(Graphics g)
    {
        using Brush blackBrush = new SolidBrush(Color.Black);

        // left rear wheel
        g.FillRoundedRectangle(blackBrush, new Rectangle(tyreRearLeft.X - 11, tyreRearLeft.Y - 11, 16, 24), 3);

        // right rear wheel
        g.FillRoundedRectangle(blackBrush, new Rectangle(tyreRearRight.X - 2, tyreRearRight.Y - 12, 16, 24), 3);
    }


    /// <summary>
    /// Shows the slip.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="g"></param>
    /// <param name="centreX"></param>
    /// <param name="tyreFrontRight"></param>
    /// <param name="tyreRearRight"></param>
    private static void DrawSidewaysSlip(CarUsingRealPhysics data, Graphics g, int centreX)
    {
        using Pen pArrow = new(Color.Red, 1);
        pArrow.DashStyle = DashStyle.Dash;
        pArrow.EndCap = LineCap.ArrowAnchor;

        // arrow on front

        double angle = data.slipAngleFrontSideways + Math.PI;
        double xr = 25 * Math.Sin(angle);
        double yr = 25 * Math.Cos(angle);

        g.DrawLine(pArrow, centreX, tyreFrontRight.Y - 40, (int)(centreX + xr), (int)(tyreFrontRight.Y + yr) - 40);

        // arrow on rear

        angle = data.slipAngleRearSideways;
        xr = 25 * Math.Sin(angle);
        yr = 25 * Math.Cos(angle);

        g.DrawLine(pArrow, centreX, tyreRearRight.Y + 30, (int)(centreX + xr), (int)(tyreRearRight.Y + yr) + 30);
    }

}