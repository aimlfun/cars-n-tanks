namespace AICarTrack.World
{
    /// <summary>
    /// 
    /// </summary>
    internal class CarConfig
    {
        //  Defaults approximate a real-wheel drive go-kart.

        /// <summary>
        /// Gravity.
        /// </summary>
        internal double c_gravity = 9.81F;  // m/s^2

        /// <summary>
        /// 
        /// </summary>
        internal double c_inertiaScale = 1.0F;  // Multiply by mass for inertia

        /// <summary>
        /// The mass of the car. (Weight is multiplied by gravity)
        /// </summary>
        internal const double c_massOfKartKg = 81.6F; // kg

        /// <summary>
        /// The mass of the pretend person driving the car. (Weight is multiplied by gravity)
        /// </summary>
        internal const double c_massOfDriverKg = 76F; // kg

        /// <summary>
        /// Gross Vehicle Weight GVW is approx 185kg. (Weight is multiplied by gravity)
        /// </summary>
        internal double massOfKartPlusDriverKg = c_massOfKartKg + c_massOfDriverKg; // kg

        // Wheelbase 1250mm

        /// <summary>
        /// Go Kart 78" long = 1.867 metres
        /// </summary>
        internal const double goKartLengthMetres = 1.63F;

        /// <summary>
        /// 31" wide = 0.78695 metres
        /// </summary>
        internal const double goKartWidthMetres = 0.78695F;

        /*
		 *			  | 76kg
		 *			 \|/      
		 *				 [###]
		 *     / ##  -----{|[]}     
		 *     |=====O(   )|  }  << engine
		 *     \ ##  -----{|--}
		 *               [###]
		 *     
		 *     ¦  ¦     ¦--¦  ¦
		 *             cg axle
		 */
        internal double halfWidth = goKartWidthMetres / 2; // Centre to side of chassis (metres)

        /// <summary>
        /// Centre of gravity to front of chassis (metres)
        /// </summary>
        internal double centreOfGravityToFront = 60F / 100F * goKartLengthMetres; /* was 52 */

        /// <summary>
        /// Centre of gravity to rear of chassis
        /// </summary>
        internal double centreOfGravityToRear = 40F / 100F * goKartLengthMetres; /* was 48 */

        /// <summary>
        /// // Centre gravity to front axle
        /// </summary>
        internal double centreOfGravityToFrontAxle = 34F / 100F * goKartLengthMetres; /* was 31/100 */

        /// <summary>
        /// Estimated centre gravity to rear axle.
        /// </summary>
        internal double centreOfGravityToRearAxle = 24F / 100F * goKartLengthMetres; /* was 42/100*/

        /// <summary>
        /// 250 mm Centre gravity height (g isn't the engine, it isn't the person it's a blended height).
        /// </summary>
        internal double centreOfGravityHeight = 0.25F;

        /// <summary>
        /// Co-efficient of friction for slicks u = 0.9
        /// </summary>
        internal double GripOfFrontTyres = 0.7F;  // How much grip tires have, smaller.

        /// <summary>
        /// Co-efficient of friction for slicks u = 0.9
        /// </summary>
        internal double GripOfRearTyres = 0.8F;  // How much grip tires have

        /// <summary>
        /// % of grip available when wheel is locked.
        /// 1..100 
        /// </summary>
        internal double gripWhenWheelIsLocked = 70F; // % 

        /// <summary>
        /// Engine Power P 3.5hp 2611 watts.
        /// </summary>
        internal double engineForce = 26110F;

        /// <summary>
        /// Clamping force * coefficient of friction between the pads=4991.06*0.45=2245.977N, for a normal car.
        /// </summary>
        internal double brakeForce = 22000F;

        /// <summary\>
        /// Clamping force of the eBrake.
        /// </summary>
        internal double eBrakeForce = 12000F /*brakeForce*/ / 2.5F;

        /// <summary>
        /// How much weight is transferred during acceleration/deceleration.
        /// </summary>
        internal double weightTransfer = 0.2F;  // How much weight is transferred during acceleration/braking

        /// <summary>
        /// Steering wheel lock angle 50.52° is how far one can twist the wheel.
        /// </summary>
        internal double maxSteer = MathUtils.DegreesInRadians(50.52); // Maximum steering angle in radians

        /// <summary>
        /// Stiffness of front corner.
        /// </summary>
        internal double cornerStiffnessFront = 4F;

        /// <summary>
        /// Stiffness of rear corner.
        /// </summary>
        internal double cornerStiffnessRear = 4.2F;

        /// <summary>
        /// Cdrag 
        /// Air resistance is approximated by the following formula (Fluid Mechanics by Landau and Lifshitz, [Beckham] chapter 6, [Zuvich])
        /// 
        /// Fdrag =  0.5 * Cd * A * rho * v2
        ///    where Cd = coefficient of friction,  A is frontal area of car, rho (Greek symbol) is density of air, v = speed of the car
        /// 
        /// Air density (rho) is 1.29 kg/m3 (0.0801 lb-mass/ft3), frontal area is approx. 2.2 m2 (20 sq. feet), Cd depends on the shape of the car and determined via wind tunnel tests.  
        /// 
        /// Approximate value for a Corvette: 0.30.  This gives us a value for Cdrag:  
        ///   Cdrag = 0.5 * 0.30 * 2.2 * 1.29 
        ///         = 0.4257
        /// 
        /// Air resistance (proportional to velocity)
        /// air density 1.226 kg/𝑚3
        /// Co-efficient of drag 0.35
        /// 
        /// Asawiki said: I have my doubts about this last constant. I couldn't confirm its value anywhere. Be prepared to finetune this one to get realistic behaviour.
        /// </summary>
        internal double airResist = 2.85F; // this should be  0.4257 for a Corvette, but works well for a Go Kart

        /// <summary>
        /// Crr
        /// Co-efficient of Rolling Resistance 0.012, used to determine drag. 
        /// At approx. 100 km/h (60 mph, 30 m/s) Cdrag and Crr are equal ([Zuvich]). This means Crr must be approximately 30 times the value of Cdrag.
        /// </summary>
        internal double rollResist = 83.0F; // this should be 12.8 for a Corvette, but works well for a Go Kart
    }
}
