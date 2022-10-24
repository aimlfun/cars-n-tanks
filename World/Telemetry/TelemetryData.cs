using CarsAndTanks.World.Car;

namespace CarsAndTanks.World.Telemetry
{
    /// <summary>
    /// Physics data that controls why the car behaved that way.
    /// </summary>
    internal class TelemetryData
    {
        internal PointF LocationOnTrack = new();
        internal PointCarF VelocityInLocalCarCoords = new();
        internal PointF AccelerationInWorldCoords = new();
        internal PointCarF AccelInLocalCarCoords = new();
        internal double FrontAxleWeightInNewtons;
        internal double RearAxleWeightInNewtons;
        internal double AmountOfSteeringInput = 0F;
        internal double FrontWheelSteerAngle = 0F;
        internal VehicleInputState Inputs = new();
        internal double WheelBase = 0F; // metres
        internal double AxleWeightRatioFront = 0F;
        internal double AxleWeightRatioRear = 0F;
        internal PointF VelocityMSInWorldCoords; // ms
        internal double AbsoluteVelocityMSInWorldCoords = 0F;
        internal double AngularAccelerationInRadians;
        internal double AngularVelocityInRadians = 0F;
        internal double AngularTorqueInNewtons;
        internal double FrictionForceFrontLocalCarCoordsSideways;
        internal double FrictionForceRearLocalCarCoordsSideways;
        internal double SlipAngleFrontSideways;
        internal double SlipAngleRearSideways;
        internal double TractionForceLongitudinalInLocalCarCoords;
        internal double TotalThrottleForce;
        internal double DragForceLocalCarCoordsLongitudinalAkaForwards;
        internal double DragForceLocalCarCoordsLateralAkaSideways;
        internal double HeadingOfCarInRadians = 0;
        internal double TotalForceLocalCarCoordsLongitudinalForwards;
        internal double TotalForceLocalCarCoordsLateralAkaSideways;
        internal double YawSpeedFront;
        internal double YawSpeedRear;
    }
}