namespace AICarTrack.World
{
    /// <summary>
    /// Consistent data for controlling a vehicle.
    /// </summary>
    internal class VehicleInputState
    {
        /// <summary>
        /// Does not apply to tank, it uses 2 throttles.
        /// </summary>
        internal double Steer;

        /// <summary>
        /// 
        /// </summary>
        internal double Ebrake;

        /// <summary>
        /// Amount to break 0..1 from the neural network.
        /// </summary>
        private double brake;

        /// <summary>
        /// Amount to break 0..1 from the neural network.
        /// </summary>
        internal double Brake { get => brake; set => brake = MathUtils.Clamp(value, 0, 1); }

        /// <summary>
        /// 
        /// </summary>
        private double throttle1;

        /// <summary>
        /// 
        /// </summary>
        private double throttle2;

        /// <summary>
        /// Throttle amount 0..1.5, excludes <0.
        /// </summary>
        internal double Throttle1 { get => throttle1; set => throttle1 = MathUtils.Clamp(value, 0, 1.5); }

        /// <summary>
        /// Throttle amount 0..1.5, excludes <0. 2nd throttle is to support tank tracks (left vs. right)
        /// </summary>
        internal double Throttle2 { get => throttle2; set => throttle2 = MathUtils.Clamp(value, 0, 1.5); }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="steer"></param>
        /// <param name="throttle"></param>
        /// <param name="brake"></param>
        internal VehicleInputState(double steer, double throttle, double brake = 0, double ebrake = 0, double throttle2 = 0)
        {
            Steer = steer;
            Throttle1 = throttle;
            Brake = brake;
            Ebrake = ebrake;
            Throttle2 = throttle2;
        }

        /// <summary>
        /// 
        /// </summary>
        internal VehicleInputState()
        {

        }
    }
}
