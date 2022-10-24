using CarsAndTanks.Learn;
using CarsAndTanks.Settings;
using CarsAndTanks.Utilities;
using CarsAndTanks.World;
using CarsAndTanks.World.Car;

namespace CarsAndTanks.World.UX.VehicleRenderers;

/// <summary>
/// Renderer class for a car. INHERIT AND OVERRIDE.
/// </summary>
internal class VehicleRendererBase
{
    /// <summary>
    /// Draws the car. OVERRIDE THIS BASED ON CAR TYPE.
    /// </summary>
    /// <param name="g"></param>
    /// <param name="car"></param>
    /// <exception cref="NotImplementedException"></exception>
    internal virtual void Draw(Graphics g, VehicleDrivenByAI car)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Renders the car. Call this not "Draw".
    /// </summary>
    /// <param name="g"></param>
    /// <param name="car"></param>
    internal void Render(Graphics g, VehicleDrivenByAI car)
    {
        ConfigDisplay displayConfig = Config.s_settings.Display;

        if (car.HasBeenEliminated)
        {
            if (displayConfig.BrieflyDisplayGhostOfEliminatedCars && car.EliminatedFadeCount > 0)
            {
                car.Renderer.DrawGhostCar(g, car);
            }

            return;
        }

        // does the actual drawing of the car.
        Draw(g, car);

        // put a yellow blob on the "lead" car
        if (car.id == LearningAndRaceManager.s_currentBestCarId)
        {
            g.FillEllipse(new SolidBrush(Color.Yellow), car.CarImplementation.LocationOnTrack.X - 3, car.CarImplementation.LocationOnTrack.Y - 3, 6, 6);
        }

        // overlay with the number of the car, if not turned off
        if (displayConfig.DisplayCarNumber != CarNumberDisplayModes.None)
        {
            DrawCarNumber(car, g);
        }

        // provide dots where the car is sensing hits
        if (Config.s_settings.Display.ShowHitPointsOnCar) ShowHitPoints(g, car);
    }

    /// <summary>
    /// Returns true if car collided with object such as grass. This is performed agnostic of the shape of the car.
    /// </summary>
    /// <param name="car"></param>
    /// <returns>true - car has collided with something | false - car is on the track.</returns>
    internal virtual bool Collided(VehicleDrivenByAI car)
    {
        PointF[] points = DetermineHitTestPoints(car);

        foreach (PointF hitTestPoint in points)
        {
            if (PixelIsGrass(hitTestPoint)) return true;
        }

        return false;
    }

    /// <summary>
    /// Determines if the pixel at PointF p is grass. 
    /// </summary>
    /// <param name="p"></param>
    /// <returns>true - pixel contains grass | false - pixel is not grass</returns>
    private static bool PixelIsGrass(PointF p)
    {
        // (int) (val+0.5) is "rounding"
        if (WorldManager.PixelIsGrass((int)(0.5F + p.X), (int)(0.5F + p.Y))) return true;

        return false; // not grass
    }

    /// <summary>
    /// Compute the hit points based on the angle of the car, and its location.
    /// </summary>
    /// <param name="car"></param>
    /// <returns></returns>
    internal PointF[] DetermineHitTestPoints(VehicleDrivenByAI car)
    {
        PointF[] points = RawHitTestPoints(car);

        PointF origin = new((float)Math.Round(car.CarImplementation.LocationOnTrack.X),
                            (float)Math.Round(car.CarImplementation.LocationOnTrack.Y));

        List<PointF> rotatedPoints = new();

        foreach (PointF p in points)
        {
            rotatedPoints.Add(MathUtils.RotatePointAboutOrigin(new PointF(p.X + origin.X, p.Y + origin.Y),
                                                               origin,
                                                               car.CarImplementation.AngleVehicleIsPointingInDegrees));
        }

        return rotatedPoints.ToArray();
    }

    /// <summary> 
    /// Override this to provide a list of hit test points. This has to be subclassed because each car differs in the hit points.
    /// </summary>
    /// <param name="car"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    internal virtual PointF[] RawHitTestPoints(VehicleDrivenByAI car)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Draws dots where the hit points are. Great for debugging!
    /// </summary>
    /// <param name="g"></param>
    /// <param name="car"></param>
    internal void ShowHitPoints(Graphics g, VehicleDrivenByAI car)
    {
        PointF[] points = DetermineHitTestPoints(car);

        foreach (PointF hitTestPoint in points)
        {
            if (g is not null) g.DrawRectangle(Pens.Cyan, (int)(0.5F + hitTestPoint.X), (int)(0.5F + hitTestPoint.Y), 1, 1);
        }
    }

    /// <summary>
    /// Draws a car circle, and after a period it disappears. This can be overridden, this is the default (a circle).
    /// </summary>
    /// <param name="g"></param>
    /// <param name="car"></param>
    /// <exception cref="Exception"></exception>
    internal virtual void DrawGhostCar(Graphics g, VehicleDrivenByAI car)
    {
        ConfigDisplay displayConfig = Config.s_settings.Display;

        --car.EliminatedFadeCount; // decrease, so when it hits zero we no longer draw it.

        Pen ghostPen = car.EliminatedReason switch
        {
            VehicleDrivenByAI.EliminationReasons.notEliminated => throw new Exception("this only applies to eliminated cars"),
            VehicleDrivenByAI.EliminationReasons.collided => displayConfig.RedPenForCollidedEliminatedCars,
            VehicleDrivenByAI.EliminationReasons.backwardsGate => displayConfig.GreenPenForBackwardsGateEliminatedCars,
            VehicleDrivenByAI.EliminationReasons.gateMutate => displayConfig.BluePenForEliminatedCars,
            VehicleDrivenByAI.EliminationReasons.moves100 => displayConfig.GreyPenFor200movesEliminatedCars,
            VehicleDrivenByAI.EliminationReasons.userPunish => displayConfig.BluePenForEliminatedCars,
            _ => throw new Exception("missing reason"),
        };

        PointF[] pointsArray = DetermineHitTestPoints(car);
        List<PointF> points = new(pointsArray);
        points.Add(pointsArray[0]);

        g.DrawLines(ghostPen, points.ToArray());
    }

    /// <summary>
    /// Draws the label for this car, either to the right of it, or on top of the car.
    /// </summary>
    /// <param name="car"></param>
    /// <param name="brushLabelForCarNumber"></param>
    /// <exception cref="Exception"></exception>
    private void DrawCarNumber(VehicleDrivenByAI car, Graphics g)
    {
        ConfigDisplay displayConfig = Config.s_settings.Display;
        ConfigWorld worldConfig = Config.s_settings.World;

        Brush brushLabelForCarNumber = displayConfig.BrushCarNumberExteriorLabel;

        switch (displayConfig.DisplayCarNumber)
        {
            case CarNumberDisplayModes.WithinVehicle:
                brushLabelForCarNumber = displayConfig.BrushCarNumberInteriorLabel;
                break;

            case CarNumberDisplayModes.OutsideOfVehicle:
                brushLabelForCarNumber = displayConfig.BrushCarNumberExteriorLabel;
                break;
        }

        // work out how big the label is, so we can center it on the car etc.
        string carId = car.id.ToString();
        SizeF sizeOfNumber = g.MeasureString(carId, displayConfig.fontForCarLabel);

        var positionOfNumber = displayConfig.DisplayCarNumber switch
        {
            CarNumberDisplayModes.WithinVehicle => new PointF(car.CarImplementation.LocationOnTrack.X - sizeOfNumber.Width / 2,
                                                          car.CarImplementation.LocationOnTrack.Y + 2 - sizeOfNumber.Height / 2),// +2 because measure of "descending" chars

            CarNumberDisplayModes.OutsideOfVehicle => new PointF(car.CarImplementation.LocationOnTrack.X + worldConfig.DiameterOfCarInPixels + 3,
                                                             car.CarImplementation.LocationOnTrack.Y - sizeOfNumber.Height / 2),

            _ => throw new Exception("Config.Settings.Display.DisplayCarNumber is set to a value this code doesn't account for."),
        };

        g.DrawString(carId, displayConfig.fontForCarLabel, brushLabelForCarNumber, positionOfNumber);
    }
}
