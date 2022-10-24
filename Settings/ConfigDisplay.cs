namespace CarsAndTanks.Settings;

/// <summary>
/// Vehicles have no label | a label drawn in the middle of the vehicle | a label "tag" outside to right of the vehicle
/// </summary>
internal enum CarNumberDisplayModes { None, WithinVehicle, OutsideOfVehicle }

/// <summary>
/// Items around visual display
/// </summary>
internal class ConfigDisplay
{
    /// <summary>
    /// Different visualisation
    /// </summary>
    public enum VehicleSupportedShapes { Blob, GoKart, Tank };

    /// <summary>
    /// Drawing of the car can be a blob (conveniece) or a go-kart.
    /// </summary>
    public VehicleSupportedShapes VehicleShape { get; set; } = VehicleSupportedShapes.GoKart;

    /// <summary>
    /// If true, then each car will be a random color.
    /// </summary>
    public bool CarsAreRandomColours { get; set; } = true;

    /// <summary>
    /// When true, it flashes the car as it goes over the gate.
    /// </summary>
    public bool FlashCarAsItGoesOverAGate { get; set; } = false;

    /// <summary>
    /// Shows the path and forces overlaid on top of track once the vehicles have done a lap.
    /// </summary>
    public bool ShowTelemetry { get; set; } = false;

    /// <summary>
    /// When true, it draws the gate numbers and lines indicating where the gate is.
    /// </summary>
    public bool DisplayGates { get; set; } = false;

    /// <summary>
    /// When true, it shows a "ghost" of the car so you can see where it was eliminated / what it hit.
    /// </summary>
    public bool BrieflyDisplayGhostOfEliminatedCars { get; set; } = true;

    /// <summary>
    /// If BrieflyDisplayGhostOfEliminatedCars == true, this determines how long they are visible for.
    /// We could faff with timers, but a simple way is to do it after so many moves.
    /// </summary>
    public int HowLongToShowGhostOfEliminatedCars { get; set; } = 100; // moves of cars (timer based) 

    /// <summary>
    /// Cars can be displayed with or without number, with a label or a number inside the car.
    /// </summary>
    public CarNumberDisplayModes DisplayCarNumber { get; set; } = CarNumberDisplayModes.None;

    /// <summary>
    /// If TRUE it draws dot where the collision sensors are.
    /// </summary>
    public bool ShowHitPointsOnCar { get; set; } = false;

    /// <summary>
    /// Location of "rank" pane.
    /// </summary>
    public Point LocationOfMoveableRankingsPane { get; set; } = new Point(5, 0);

    /// <summary>
    /// Location of "neural network" pane.
    /// </summary>
    public Point LocationOfMoveableNeuralNetworkPane { get; set; } = new Point(300, 0);

    /// <summary>
    /// Location of "telemetry" pane.
    /// </summary>
    public Point LocationOfMoveableTelemetryPane { get; set; } = new Point(500, 0);

    /// <summary>
    /// Location of "vision" pane.
    /// </summary>
    public Point LocationOfMoveableVisionPane { get; set; } = new Point(700, 0);

    /// <summary>
    /// Location of "speed" pane.
    /// </summary>
    public Point LocationOfMoveableSpeedPane { get; set; } = new Point(900, 0);

    /// <summary>
    /// Location of "speed" pane.
    /// </summary>
    public Point LocationOfMoveableStatisicsPane { get; set; } = new Point(1000, 0);

    #region PENS
    /// <summary>
    /// Pen used to draw eliminated cars.
    /// </summary>
    internal readonly Pen BluePenForEliminatedCars = new(Color.FromArgb(180, 0, 0, 255));

    /// <summary>
    /// Pen used for cars that collided and were eliminated.
    /// </summary>
    internal readonly Pen RedPenForCollidedEliminatedCars = new(Color.FromArgb(180, 255, 0, 0));

    /// <summary>
    /// Pen used for cars that went backwards and were eliminated.
    /// </summary>
    internal readonly Pen GreenPenForBackwardsGateEliminatedCars = new(Color.FromArgb(180, 0, 255, 0));

    /// <summary>
    /// Pen used for cars the failed to relocate after 200 moves and got eliminated for it.
    /// </summary>
    internal readonly Pen GreyPenFor200movesEliminatedCars = new(Color.FromArgb(180, 30, 30, 30));

    /// <summary>
    /// Pen used to draw fitness gates.
    /// </summary>
    internal readonly Pen PenForDrawingTrackFitnessGates = new(Color.FromArgb(200, Color.Yellow)); // 40 DarkRed

    /// <summary>
    /// Pen used to draw the road.
    /// </summary>        
    internal readonly Pen RoadPen = new(Color.FromArgb(0, 0, 0));
    #endregion

    #region BRUSHES
    /// <summary>
    /// 
    /// </summary>
    internal SolidBrush brushGate = new(Color.White);

    /// <summary>
    /// 
    /// </summary>
    internal Brush BrushCar = new SolidBrush(Color.Red);

    /// <summary>
    /// 
    /// </summary>
    internal SolidBrush BrushCarNumberInteriorLabel = new(Color.Black);

    /// <summary>
    /// 
    /// </summary>
    internal SolidBrush BrushCarNumberExteriorLabel = new(Color.Black);

    /// <summary>
    /// Background colour for car as it goes thru a gate.
    /// </summary>
    internal Brush BrushCarAtGate = new SolidBrush(Color.DarkSlateGray);
    #endregion

    #region FONTS
    /// <summary>
    /// 
    /// </summary>
    internal readonly Font fontForGeneration = new("Arial", 15);

    /// <summary>
    /// 
    /// </summary>
    internal readonly Font fontForCarLabel = new("Arial", 7);

    /// <summary>
    /// 
    /// </summary>
    internal readonly Font fontForTelemetry = new("Segoe UI", 8);
    #endregion

    /// <summary>
    /// Constructor
    /// </summary>
    internal ConfigDisplay()
    {
        RoadPen.StartCap = System.Drawing.Drawing2D.LineCap.Round;
        RoadPen.EndCap = System.Drawing.Drawing2D.LineCap.Round;

        PenForDrawingTrackFitnessGates.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
    }
}
