using CarsAndTanks.World.Telemetry;

namespace CarsAndTanks.World.Car;

/// <summary>
/// Abstract class that defines key attributes for a vehicle and a method for moving it.
/// </summary>
abstract class Vehicle
{
    #region TELEMETRY
    /// <summary>
    /// Where the car is on the track.
    /// </summary>
    internal PointF LocationOnTrack = new();

    /// <summary>
    /// Angle vehicle is pointing, that dictates the direction the vehicle will move in.
    /// </summary>
    internal double AngleVehicleIsPointingInDegrees = 0F;

    /// <summary>
    /// Speed car is travelling at.
    /// </summary>
    internal double Speed = 1F;

    /// <summary>
    /// All the data for the car is tracked here after applying physics.
    /// </summary>
    internal TelemetryData? LapTelemetryData;
    #endregion

    /// <summary>
    /// Implement this.
    /// Using "carInputState", move the car (change LocationOnTrack, AngleCarIsPointingInDegrees, Speed).
    /// </summary>
    /// <param name="carInputState"></param>
    internal abstract void ApplyPhysics(VehicleInputState carInputState);
}
