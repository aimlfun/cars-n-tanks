using AICarTrack.Settings;
using AICarTrack.World;
using AICarTrack.World.Telemetry;
using CarsAndTanks.Settings;

namespace AICarTrack
{

    /// <summary>
    /// Physics for a basic car. It has no understanding of friction, drag, momentum or anything.
    /// </summary>
    internal class CarUsingBasicPhysics : Vehicle
    {
        /// <summary>
        /// Moves the car using speed at given angle.
        /// </summary>
        /// <param name="carInputState"></param>
        internal override void ApplyPhysics(VehicleInputState carInputState)
        {
            var displayConfig = Config.s_settings.Display;
            // currently the NN provides the delta, but cars don't instantly rotate, so we should feed this
            // into more realistic (spread steer, cap at 30 degrees or something)

            if (displayConfig.VehicleShape == ConfigDisplay.VehicleSupportedShapes.Tank)
            {
                // based on which throttle is higher we steer in the direction of the slower
                AngleVehicleIsPointingInDegrees += (carInputState.Throttle2 - carInputState.Throttle1) / 2;

                // if both are "1" we move top speed, anything else is less than.
                Speed = (((carInputState.Throttle1 + carInputState.Throttle2) * 0.9F) / 2F);
            }
            else
            {
                // Remember: speed x angle determines where the car ends up next
                AngleVehicleIsPointingInDegrees += carInputState.Steer;

                // currently the NN provides the "speed" not the acceleration. TODO: change
                Speed = ((carInputState.Throttle1 + 0.9F) / 2F);
            }

            // it'll work even if we violate this, but let's keep it clean 0..359.999 degrees.
            if (AngleVehicleIsPointingInDegrees < 0) AngleVehicleIsPointingInDegrees += 360;
            if (AngleVehicleIsPointingInDegrees >= 360) AngleVehicleIsPointingInDegrees -= 360;

            // move the car using basic sin/cos math ->  x = r cos(theta), y = r x sin(theta)
            // in this instance "r" is the speed output, theta is the angle of the car.

            double angleCarIsPointingInRadians = MathUtils.DegreesInRadians(AngleVehicleIsPointingInDegrees);
            LocationOnTrack.X += (float)(Math.Cos(angleCarIsPointingInRadians) * Speed);
            LocationOnTrack.Y += (float)(Math.Sin(angleCarIsPointingInRadians) * Speed);

            LapTelemetryData = new TelemetryData
            {
                AbsoluteVelocityMSInWorldCoords = Speed,
                HeadingOfCarInRadians = MathUtils.DegreesInRadians(AngleVehicleIsPointingInDegrees),
                LocationOnTrack = new PointF(LocationOnTrack.X, LocationOnTrack.Y)
            };
        }
    }
}
