using AICarTrack.World.Telemetry;

namespace AICarTrack.World
{
    /// <summary>
    /// Used to describe longitudinal and lateral movement.
    /// </summary>
    internal class PointCarF
    {
        /// <summary>
        /// Longitudinal.
        /// </summary>
        internal float forwardsBackwards;

        /// <summary>
        /// Lateral.
        /// </summary>
        internal float sideways;
    }

    /// <summary>
    /// Simulates physics for a car (inertia, angular momentum etc).
    /// </summary>
    internal class CarUsingRealPhysics : Vehicle
    {
        /// <summary>
        /// Velocity in local car coords (x is forward y is sideways)
        /// </summary>
        private readonly PointCarF velocityInLocalCarCoords = new(); // m/s 

        /// <summary>
        /// Acceleration in world coords.
        /// </summary>
        private PointF accelerationInWorldCoords = new();

        /// <summary>
        /// Acceleration in local car coords.
        /// </summary>
        private readonly PointCarF accelInLocalCarCoords = new();

        /// <summary>
        /// Weight of the car that pushing on the front axle.
        /// </summary>
        internal double frontAxleWeightInNewtons;

        /// <summary>
        /// Weight of the car that pushing on the rear axle.
        /// </summary>
        internal double rearAxleWeightInNewtons;

        /// <summary>
        /// Amount of steering input (-1.0..1.0)
        /// </summary>
        internal double amountOfSteeringInput = 0F;

        /// <summary>
        /// Actual front wheel steer angle (-maxSteer..maxSteer)
        /// </summary>
        internal double frontWheelSteerAngle = 0F;

        /// <summary>
        /// State of inputs from neural network that controls steering, throttle, brake, ebrake.
        /// </summary>
        internal VehicleInputState inputs = new();

        /// <summary>
        /// Input smoothing of steering (removes wiggle).
        /// </summary>
        private readonly bool smoothSteer = false;

        /// <summary>
        /// Safe steering (angle limited by speed).
        /// </summary>
        private readonly bool safeSteer = false;

        /// <summary>
        /// Inertia the engine has to overcome.
        /// 
        /// The inertia of a cylinder = Mass * radius ^ 2 / 2
        /// 
        /// So for a 75 kg wheel with a 33 cm radius that's an inertia of 75 *  0.33* 0.33 / 2 = 4.1 kg.m2 You must double 
        /// that to get the total inertia of both wheels on the rear axle and perhaps add something for the inertia of the
        /// axle itself, the inertia of the gears and the inertia of the engine.
        /// 
        ///	Go Kart wheels are smaller, and engines lighters.
        /// </summary>
        private const double inertiaOfRearWheelsAxleGearsAndEngine = 70F; // weight Config.massOfKartPlusDriverKg * Config.c_inertiaScale;

        /// <summary>
        /// Distance in metres between from and rear axle.
        /// </summary>
        private double wheelBase = 0F; // metres

        /// <summary>
        /// Ratio of car weight on the front axle.
        /// </summary>
        private double axleWeightRatioFront = 0F;

        /// <summary>
        /// Ratio of car weight on the rear axle.
        /// </summary>
        private double axleWeightRatioRear = 0F;

        /// <summary>
        /// Configuration of a "car" rather than specifics.
        /// </summary>
        internal readonly CarConfig Config = new();

        /// <summary>
        /// Velocity m/s in world coords.
        /// </summary>
        private PointF velocityMSInWorldCoords; // ms

        /// <summary>
        /// Absolute velocity m/s in World Coords.
        /// </summary>
        internal double absoluteVelocityMSInWorldCoords = 0F;

        /// <summary>
        /// 
        /// </summary>
        internal double angularAccelerationInRadians;

        /// <summary>
        /// Angular velocity in radians.
        /// </summary>
        internal double angularVelocityInRadians = 0F;

        /// <summary>
        /// 
        /// </summary>
        internal double angularTorqueInNewtons;

        /// <summary>
        /// 
        /// </summary>
        internal double frictionForceFrontLocalCarCoordsSideways;

        /// <summary>
        /// 
        /// </summary>
        internal double frictionForceRearLocalCarCoordsSideways;

        /// <summary>
        /// Sideways slip angle (alpha), which is the angle between the tire's heading and its direction of travel.
        /// </summary>
        internal double slipAngleFrontSideways;

        /// <summary>
        /// Longitudinal slip angle (alpha), which is the angle between the tire's heading and its direction of travel.
        /// </summary>
        internal double slipAngleRearSideways;

        /// <summary>
        /// Force in the forward/backward direction.
        /// </summary>
        internal double tractionForceLongitudinalInLocalCarCoords;

        /// <summary>
        /// This is the input throttle x engine.
        /// </summary>
        internal double totalThrottleForce;

        /// <summary>
        /// This is the drag from wind/rolling resistance
        /// </summary>
        internal double dragForceLocalCarCoordsLongitudinalAkaForwards;

        /// <summary>
        /// The drag sideways from friction.
        /// </summary>
        internal double dragForceLocalCarCoordsLateralAkaSideways;

        /// <summary>
        /// Angle car is pointed at (radians).
        /// </summary>
        internal double headingOfCarInRadians = Math.PI;

        /// <summary>
        /// 
        /// </summary>
        internal double totalForceLocalCarCoordsLongitudinalForwards;

        /// <summary>
        /// 
        /// </summary>
        internal double totalForceLocalCarCoordsLateralAkaSideways;

        /// <summary>
        /// 
        /// </summary>
        internal double yawSpeedFront;

        /// <summary>
        /// 
        /// </summary>
        internal double yawSpeedRear;

        /// <summary>
        /// 
        /// </summary>
        internal double dt = 30F / 1000F;  // delta T in seconds

        /// <summary>
        /// Applies the physics based on the input.
        /// </summary>
        /// <param name="carInputState"></param>
        internal override void ApplyPhysics(VehicleInputState carInputState)
        {
            headingOfCarInRadians = MathUtils.DegreesInRadians(AngleVehicleIsPointingInDegrees);

            inputs = carInputState;

            // adjust steering to make it easier to drive

            //  Perform filtering on steering...
            if (this.smoothSteer)
                amountOfSteeringInput = ApplySmoothSteer(inputs.Steer, dt);
            else
                amountOfSteeringInput = inputs.Steer;

            if (this.safeSteer) amountOfSteeringInput = ApplySafeSteer(amountOfSteeringInput);

            //  Now set the actual steering angle, clamping to stop them turning the steering 360 degrees!
            frontWheelSteerAngle = Clamp(amountOfSteeringInput, -Config.maxSteer, Config.maxSteer);

            //  Now that the inputs have been filtered and we have our throttle,
            //  brake and steering values, perform the car physics update...

            DoPhysics(dt);

            StoreTelemetry();
        }

        /// <summary>
        /// Store *everything* about the car.
        /// </summary>
        private void StoreTelemetry()
        {
            LapTelemetryData = new TelemetryData
            {
                LocationOnTrack = new PointF(LocationOnTrack.X, LocationOnTrack.Y),
                HeadingOfCarInRadians = headingOfCarInRadians,
                VelocityInLocalCarCoords = velocityInLocalCarCoords,
                AccelerationInWorldCoords = accelerationInWorldCoords,
                AccelInLocalCarCoords = accelInLocalCarCoords,
                FrontAxleWeightInNewtons = frontAxleWeightInNewtons,
                RearAxleWeightInNewtons = rearAxleWeightInNewtons,
                AmountOfSteeringInput = amountOfSteeringInput,
                FrontWheelSteerAngle = frontWheelSteerAngle,
                Inputs = new(),
                WheelBase = wheelBase,
                AxleWeightRatioFront = axleWeightRatioFront,
                AxleWeightRatioRear = axleWeightRatioRear,
                VelocityMSInWorldCoords = velocityMSInWorldCoords,
                AbsoluteVelocityMSInWorldCoords = absoluteVelocityMSInWorldCoords,
                AngularAccelerationInRadians = angularAccelerationInRadians,
                AngularVelocityInRadians = angularVelocityInRadians,
                AngularTorqueInNewtons = angularTorqueInNewtons,
                FrictionForceFrontLocalCarCoordsSideways = frictionForceFrontLocalCarCoordsSideways,
                FrictionForceRearLocalCarCoordsSideways = frictionForceRearLocalCarCoordsSideways,
                SlipAngleFrontSideways = slipAngleFrontSideways,
                SlipAngleRearSideways = slipAngleRearSideways,
                TractionForceLongitudinalInLocalCarCoords = tractionForceLongitudinalInLocalCarCoords,
                TotalThrottleForce = totalThrottleForce,
                DragForceLocalCarCoordsLongitudinalAkaForwards = dragForceLocalCarCoordsLongitudinalAkaForwards,
                DragForceLocalCarCoordsLateralAkaSideways = dragForceLocalCarCoordsLateralAkaSideways,
                TotalForceLocalCarCoordsLongitudinalForwards = totalForceLocalCarCoordsLongitudinalForwards,
                TotalForceLocalCarCoordsLateralAkaSideways = totalForceLocalCarCoordsLateralAkaSideways,
                YawSpeedFront = yawSpeedFront,
                YawSpeedRear = yawSpeedRear
            };

            LapTelemetryData.Inputs.Throttle1 = inputs.Throttle1;
            LapTelemetryData.Inputs.Brake = inputs.Brake;
            LapTelemetryData.Inputs.Ebrake = inputs.Ebrake;
            LapTelemetryData.Inputs.Steer = inputs.Steer;
        }

        /// <summary>
        /// This is the application of angular momentum and inertia, basic applied maths to move the car.
        /// </summary>
        /// <param name="dt">time</param>
        internal void DoPhysics(double dt)
        {
            // pre-calc sin/cos for the heading, as we need it multiple times.
            double sin = Math.Sin(headingOfCarInRadians);
            double cos = Math.Cos(headingOfCarInRadians);

            /* Physics modelled based on this excellent explanation https://asawicki.info/Mirror/Car%20Physics%20for%20Games/Car%20Physics%20for%20Games.html
			 *
			 *   LocalCarCoords are based on a fixed reference: 
			 *   - Car is going faster/slower in the "X" direction.
			 *   - Difting in the "y" direction.
			 *   
			 *   
			 *   <--> x
			 *				 [###]
			 *     / ## /-----{|--}               /|\
			 *     |===O(   )[@@@]}  << engine     |   y
			 *     \ ## \-----{|--}               \|/
			 *               [###]
			 *               
			 *   We rotate/move the car based on LocalCarCoords calculations in the "real" world.
			 * 
			 */


            //       [###]    cg  [###]
            //  F  +---|-------:----|----+
            //  R  |   |       :    |    | R
            //  O  |   |       X    |    | E
            //  N  |   |       :    |    | A 
            //  T  +---|-------:----|----+ R
            //       [###]        [###]
            //             
            //                
            //                 ¦----¦ centreOfGravityToRearAxle             
            //
            //         ¦------| centreOfGravityToFrontAxle           
            //
            //         ¦------------¦ wheelbase
            //
            //a wheelbase is defined as the distance between the front and rear axles (where the wheels are attached, hence wheel "base").
            wheelBase = Config.centreOfGravityToFrontAxle + Config.centreOfGravityToRearAxle;

            //   .   .            .    .
            //   |   |            |    |
            //   +---|------------|----+          
            //     [###]        [###]
            //
            //       |------cg----| wheelbase
            //                 ^ axleWeightRatioRearPct             
            //           ^ axleWeightRatioFrontPct           

            // apportion weight relative to centre of gravity to front and rear axles
            axleWeightRatioFront = Config.centreOfGravityToRearAxle / wheelBase; // car weight on the front axle
            axleWeightRatioRear = Config.centreOfGravityToFrontAxle / wheelBase; // car weight on the rear axle

            CalculateLocalVelocityWithYbeingSidewaysXbeingForwardBackwards(sin, cos);

            // Weight on axles based on centre of gravity and weight shift due to forward/reverse acceleration
            CalculateWeightTransfer();

            // Resulting velocity of the wheels as result of the yaw rate of the car body.
            CalculateYaw();

            CalculateSidewaysSlip();

            CalculateFrictionFromTyres();

            CalculateTractionForce();

            CalculateDragForces();

            CalculateTotalForcesActingOnCar();

            CalculateAccelerationFromForces(sin, cos);

            CalculateVelocityChangeDueToAcceleration(dt);

            CalculateAngularTorque();

            StabiliseCarAtLowSpeed();

            CalculateAngularAcceleration();

            ApplyAngularAccelerationToVelocity(dt);

            ApplyAngularVelocityToHeading(dt);

            ApplyVelocityToMoveCar(dt);

            // adjust the car's direction to match the adjusted "heading" we computed
            AngleVehicleIsPointingInDegrees = MathUtils.RadiansInDegrees(headingOfCarInRadians);

            // adjust the car's speed to the velocity we computerd.
            Speed = absoluteVelocityMSInWorldCoords;
        }

        /// <summary>
        /// Before we start we take the velocity of the car, and reconstruct in reference coordinates (car pointing left-right).
        /// </summary>
        /// <param name="sin">Sine of angle</param>
        /// <param name="cos">Cosine of angle</param>
        private void CalculateLocalVelocityWithYbeingSidewaysXbeingForwardBackwards(double sin, double cos)
        {
            /*
			 *						     FORCES:
			 *						     
			 *								longitudinal
			 *  y    __                          /|\
			 * /|\   /|                         __|__
 			 *  |   /                 lateral  |    |
			 *  |  /                      /___ |    |___\
			 *  | /                       \    |    |   /
			 *  +---------> x                  |____|
			 * 
			 */

            // BECAUSE we are computing x as forwards/reverse and y as drift, we use this formula to rotate the "real" car (about its centre)
            // computationally to be referenced as local direction. i.e. it's now pointing l2r.

            // Rotate the velocity acting on the car into local car coordinates. (see: utils.RotatePointAboutOrigin)
            // velocityInLocalCarCoords is now expressed X as left to right existing velocity with Y representing drifting
            velocityInLocalCarCoords.forwardsBackwards = (float)(cos * velocityMSInWorldCoords.X + sin * velocityMSInWorldCoords.Y);
            velocityInLocalCarCoords.sideways = (float)(cos * velocityMSInWorldCoords.Y - sin * velocityMSInWorldCoords.X);
        }

        /// <summary>
        /// The car's position is in turn determined by integrating the velocity over time:
        ///
        ///	p = p + dt * v
        /// </summary>
        /// <param name="dt"></param>
        private void ApplyVelocityToMoveCar(double dt)
        {
            // move the car based on velocity in UI x,y units / direction.

            // As s = v * t, i.e. distance covered is velocity * time and it's acting on our "car" the distance is added to the car.
            LocationOnTrack.X += (float)(velocityMSInWorldCoords.X * dt);
            LocationOnTrack.Y += (float)(velocityMSInWorldCoords.Y * dt);
        }

        /// <summary>
        /// Adds the angular velocity to the heading of the car.
        /// </summary>
        /// <param name="dt"></param>
        private void ApplyAngularVelocityToHeading(double dt)
        {
            // rotation is based on angular velocity 
            headingOfCarInRadians += angularVelocityInRadians * dt;

            // wrap angle, so we don't exceed 2*pi (360) or go lower than 0.
            if (headingOfCarInRadians > 2 * Math.PI) headingOfCarInRadians -= 2 * Math.PI; else if (headingOfCarInRadians < 0) headingOfCarInRadians += 2 * Math.PI;
        }

        /// <summary>
        /// Adds the angular acceleration to the velocity.
        /// </summary>
        /// <param name="dt"></param>
        private void ApplyAngularAccelerationToVelocity(double dt)
        {
            // v=u+at for angular vel.
            angularVelocityInRadians += angularAccelerationInRadians * dt;
        }

        /// <summary>
        /// 
        /// </summary>
        private void CalculateAngularAcceleration()
        {
            /*
				The net torque on the rear axle is the sum of following torques:

				    total torque = drive torque + traction torques from both wheels + brake torque

				Remember that torques are signed quantities, the drive torque wil generally have a different sign than the 
			    traction and brake torque. If the driver is not braking, the brake torque is zero.

				This torque will cause an angular acceleration of the drive wheels, just like a force applied on a mass will 
				cause it to accelerate.

					angular acceleration = total torque / rear wheel inertia.

				I've found somewhere that the inertia of a solid cylinder around its axis can be calculated as follows:

					inertia of a cylinder = Mass * radius^2 / 2

				So for a 75 kg wheel with a 33 cm radius that's an inertia of 75 *  0.33* 0.33 / 2 = 4.1 kg.m2 You must double 
				that to get the total inertia of both wheels on the rear axle and perhaps add something for the inertia of the 
				axle itself, the inertia of the gears and the inertia of the engine. 

				A positive angular acceleration will increase the angular velocity of the rear wheels over time. As for the car 
				velocity which depends on the linear acceleration, we simulate this by doing one step of numerical integration 
				each time we go through the physics calculation:

					rear wheel angular velocity += rear wheel angular acceleration * time step

				where time step is the amount of simulated time between two calls of this function.  
			
				This way we can determine how fast the drive wheels are turning and therefore we can calculate the engine's rpm.

			*/
            // angular acceleration = total torque / rear wheel inertia.	
            angularAccelerationInRadians = angularTorqueInNewtons / inertiaOfRearWheelsAxleGearsAndEngine;
        }

        /// <summary>
        /// Stop the car when it is nearly stopped.
        /// </summary>
        private void StabiliseCarAtLowSpeed()
        {
            //  gets unstable at very slow speeds, so just stop the car
            if (Math.Abs(absoluteVelocityMSInWorldCoords) < 0.5 && totalThrottleForce == 0)
            {
                velocityMSInWorldCoords.X = 0;
                velocityMSInWorldCoords.Y = 0;
                absoluteVelocityMSInWorldCoords = 0;
                angularTorqueInNewtons = 0;
                angularVelocityInRadians = 0;
            }
        }

        /// <summary>
        /// Determines whether the car rotates or not.
        /// </summary>
        private void CalculateAngularTorque()
        {
            // calculate rotational forces
            angularTorqueInNewtons = frictionForceFrontLocalCarCoordsSideways * Config.centreOfGravityToFrontAxle - frictionForceRearLocalCarCoordsSideways * Config.centreOfGravityToRearAxle;
        }

        /// <summary>
        /// The car's velocity (in meters per second) is determined by integrating the acceleration over time.  
        /// This sounds more complicated than it is, usually the following equation does the trick.  
        /// This is known as the Euler method for numerical integration.
        /// 
        /// v = v + dt * a,
        ///   where dt is the time increment in seconds between subsequent calls of the physics engine.
        /// </summary>
        /// <param name="dt"></param>
        private void CalculateVelocityChangeDueToAcceleration(double dt)
        {
            // update velocity in real world X & Y direction.
            // v=u+at  u=velocityMSInWorldCoords | a = accelerationInWorldCoords | t = dt
            velocityMSInWorldCoords.X += (float)(accelerationInWorldCoords.X * dt); // v=u+at in x-direction
            velocityMSInWorldCoords.Y += (float)(accelerationInWorldCoords.Y * dt); // v=u+at in y-direction

            // using phythagoras (a^2 = b^2 + c^2) compute velocity, absolute velocity is comprised of 2 directions
            absoluteVelocityMSInWorldCoords = (float)Math.Sqrt(velocityMSInWorldCoords.X * velocityMSInWorldCoords.X + velocityMSInWorldCoords.Y * velocityMSInWorldCoords.Y);
        }

        /// <summary>
        /// The acceleration (a) of the car (in meters per second squared) is determined by the net force on the car (in Newton)
        /// and the car's mass M (in kilogram) via Newton's second law:
        /// a = F / M
        /// </summary>
        /// <param name="sin"></param>
        /// <param name="cos"></param>
        private void CalculateAccelerationFromForces(double sin, double cos)
        {
            // Calculate acceleration along car axes.
            // From F=m*a, we can flip it to a=F/M.
            //    where m=cart+driver
            //		    F=total force.
            //
            // That provides the acceleration in a direction. It may not be the same direction the car intends to travel if the sideways grip is exceeded.
            accelInLocalCarCoords.forwardsBackwards = (float)(totalForceLocalCarCoordsLongitudinalForwards / Config.massOfKartPlusDriverKg);  // forward/reverse accel
            accelInLocalCarCoords.sideways = (float)(totalForceLocalCarCoordsLateralAkaSideways / Config.massOfKartPlusDriverKg);  // sideways accel

            // acceleration in local is with respect to X=forwards, Y=sideways. We need to convert to world coordinates, and this rotate to forward direction for the car.
            accelerationInWorldCoords.X = (float)(cos * accelInLocalCarCoords.forwardsBackwards - sin * accelInLocalCarCoords.sideways);
            accelerationInWorldCoords.Y = (float)(sin * accelInLocalCarCoords.forwardsBackwards + cos * accelInLocalCarCoords.sideways);
        }

        /// <summary>
        /// The total longtitudinal force is the vector sum of these three forces.
        /// 
        /// Flong = Ftraction + Fdrag  + Frr
        /// 
        /// In our calculation we computer Fdrag and Frr as positive amounts and subtract below. dragForceLocalCarCoordsLongitudinalAkaForwards = -(Fdrag+Frr)
        /// </summary>
        private void CalculateTotalForcesActingOnCar()
        {
            /*
			    The lateral forces of the four tyres have two results: a net cornering force and a torque around the yaw axis. 
				
				The cornering force is the force on the CG at a right angle to the car orientation and serves as the centripetal 
				force which is required to describe a circular path.  
			
				The contribution of the rear wheels to the cornering force is the same as the lateral force. 
				
				For the front wheels, multiply the lateral force with cos(delta) to allow for the steering angle.

				Fcornering = Flat, rear + cos(delta) * Flat,front				
			 */

            totalForceLocalCarCoordsLateralAkaSideways = (float)Math.Cos(frontWheelSteerAngle) * frictionForceFrontLocalCarCoordsSideways + frictionForceRearLocalCarCoordsSideways - dragForceLocalCarCoordsLateralAkaSideways;

            totalForceLocalCarCoordsLongitudinalForwards = tractionForceLongitudinalInLocalCarCoords - dragForceLocalCarCoordsLongitudinalAkaForwards;
        }

        /// <summary>
        /// Calculates approximate drag from air resistance and roll resistance.
        /// </summary>
        private void CalculateDragForces()
        {
            /*
				If this were the only force, the car would accelerate to infinite speeds.  Clearly, this is not the case in real life.  
				Enter the resistance forces.  The first and usually the most important one is air resistance, a.k.a. aerodynamic drag.  
				This force is so important because it is proportional to the square of the velocity. When we're driving fast (and which 
				game doesn't involve high speeds?) this becomes the most important resistance force.

					Fdrag = - Cdrag * v * |v|
					where Cdrag is a constant and v is the velocity vector and the notation |v| refers to the magnitude of vector v
   
			 
				The magnitude of the velocity vector is more commonly known as the speed. Note the difference of data type: speed is 
				a scalar, velocity is a vector. Use something like the following code:

					speed = sqrt(v.x*v.x + v.y*v.y);
					fdrag.x = - Cdrag * v.x * speed;
					fdrag.y = - Cdrag * v.y * speed;
			
				Then there is the rolling resistance. This is caused by friction between the rubber and road surface as the wheels roll 
				along and friction in the axles, etc.etc.. 
			 
				We'll approximate this with a force that's proportional to the velocity using another constant.

					Frr = - Crr * v

					  where Crr is a constant and v is the velocity vector.

				At low speeds the rolling resistance is the main resistance force, at high speeds the drag takes over in magnitude. 
			
				At approx. 100 km/h (60 mph, 30 m/s) they are equal ([Zuvich]). This means Crr must be approximately 30 times the value of Cdrag.

				The total longtitudinal force is the vector sum of these three forces.

					Flong =  Ftraction + Fdrag   + Frr

				Note that if you're driving in a straight line the drag and rolling resistance forces will be in the opposite direction from the
				traction force. So in terms of  magnitude, you're subtracting the resistance force from the traction force.  When the car is 
				cruising at a constant speed the forces are in equilibrium and Flong is zero.
			 */

            float longitudinalVelocity = velocityInLocalCarCoords.forwardsBackwards;
            float lateralVelocity = velocityInLocalCarCoords.sideways;

            // 	Fdrag = - Cdrag * v * |v|	  = Config.airResist * longitudinalVelocity * Math.Abs(longitudinalVelocity)		
            //  Frr = -Crr * v                = Config.rollResist * longitudinalVelocity 

            dragForceLocalCarCoordsLongitudinalAkaForwards = longitudinalVelocity * (Config.rollResist + Config.airResist * Math.Abs(longitudinalVelocity) /*|v|*/);

            dragForceLocalCarCoordsLateralAkaSideways = lateralVelocity * (Config.rollResist + Config.airResist * Math.Abs(lateralVelocity)/*|v|*/);

            // note: ^ sign is +ve, because we will SUBTRACT it later rather than add a negative amount.
        }

        /// <summary>
        /// The inputs: throttle, brake and ebrake are applied to determine the traction force.
        /// Traction force is engine pull minus the braking applied. Breaking is proportional to the pad efficiency. 
        /// We don't model brake fade (their decreased stopping ability as the brake pads get hotter).
        /// 
        /// Braking:
        /// 
        /// When braking, the traction force is replaced by a braking force which is oriented in the opposite direction.  
        /// The total longtitudinal force is then the vector sum of these three forces.
        ///   Flong =   Fbraking + Fdrag   + Frr
        /// 
        /// A simple model of braking:
        ///   Fbraking = -u * Cbraking
        ///  
        /// In this model the braking force is a constant.  Keep in mind to stop applying the braking force as soon as the speed is reduced to zero otherwise the car will end up going in reverse. 
        /// </summary>
        private void CalculateTractionForce()
        {
            // Clamping force * coefficient of friction between the pads

            // Config.brakeForce defines the amount the brake pads can stop.
            // We cap braking at that number, because it cannot exceeed that limit.
            double brake = inputs.Brake * Config.brakeForce + inputs.Ebrake * Config.eBrakeForce;

            brake = Math.Min(brake, Config.brakeForce);

            // no gears, just stop and go. so we scale throttle from 0..full engine force. Technically we're mixing power and force, but it's just a number.
            totalThrottleForce = inputs.Throttle1 * Config.engineForce;

            // traction force is the pull from the engine minus the braking force. Using "Sign()" it ensures "brake" is subtractive against forwards velocity.																											 
            tractionForceLongitudinalInLocalCarCoords = totalThrottleForce - brake * Math.Sign(velocityInLocalCarCoords.forwardsBackwards);

            // resulting traction force in local car coordinated (referenced X=left to right,y-sideways) implemented as a RWD car only which a go kart is.
        }

        /// <summary>
        /// 
        /// </summary>
        private void CalculateFrictionFromTyres()
        {
            /*
				Now let's look at high speed cornering from the wheel's point of view. In this situation we need to calculate the sideways speed of the tires. 
				Because wheels roll, they have relatively low resistance to motion in forward or rearward direction. In the perpendicular direction, however, 
			    wheels have great resistance to motion. 
			
				Try pushing a car tyre sideways. This is very hard because you need to overcome the maximum static friction force to get the wheel to slip.

				In high speed cornering, the tires develop lateral forces also known as the cornering force. This force depends on the slip angle (alpha), which 
			    is the angle between the tire's heading and its direction of travel. As the slip angle grows, so does the cornering force. The cornering force per 
			    tyre also depends on the weight on that tire. At low slip angles, the relationship between slip angle and cornering force is linear, in other words

				Flateral = Ca * alpha
			   			      where the constant Ca  is known as the cornering stiffness.
			*/

            double tyreGripFront = Config.GripOfFrontTyres;
            double tyreGripRear = Config.GripOfRearTyres * (1.0F - this.inputs.Ebrake * (1.0F - Config.gripWhenWheelIsLocked / 100)); // reduce rear grip when ebrake is on

            // friction is limited to tyre grip. Front and rear are independent.

            // F=mu * W where mu = friction coefficient of tyre, W = m*g (weight over axle).  mu is dependent on corner stiffness, limited to +/- tyre grip.
            frictionForceFrontLocalCarCoordsSideways = Clamp(-Config.cornerStiffnessFront * slipAngleFrontSideways, -tyreGripFront, tyreGripFront) * frontAxleWeightInNewtons;
            frictionForceRearLocalCarCoordsSideways = Clamp(-Config.cornerStiffnessRear * slipAngleRearSideways, -tyreGripRear, tyreGripRear) * rearAxleWeightInNewtons;
        }

        /// <summary>
        /// 
        /// </summary>
        private void CalculateSidewaysSlip()
        {
            /*
			    The velocity vector of the wheel has angle alpha relative to the direction in which the wheel can roll. 
				
			    We can split the velocity vector v up into two component vectors. 
				- The longtitudinal vector has magnitude cos(alpha) * v. Movement in this direction corresponds to the rolling motion of the wheel. 
			    - The lateral vector has magnitude sin(alpha) * v and causes a resistance force in the opposite direction: the cornering force.

				There are three contributors to the slip angle of the wheels: the sideslip angle of the car, the angular rotation of the car around 
			    the up axis (yaw rate) and, for the front wheels, the steering angle.

				The sideslip angle b (bèta) is the difference between the car orientation and the direction of movement. In other words, it's the 
			    angle between the longtitudinal axis and the actual direction of travel. So it's similar in concept to what the slip angle is for the tyres. 
			    Because the car may be moving in a different direction than where it's pointing at, it experiences a sideways motion. This is equivalent to 
			    the perpendicular component of the velocity vector.
			
			    If the car is turning round the centre of geometry (CG) at a rate omega (in rad/s!), the front wheels are describing a circular path 
			    around CG at that same rate.
		
				If the car turns full circle, the front wheel describes a circular path of distance 2*pi*b around CG in 1/(2*pi*omega) seconds 
				where b is the distance from the front axle to the CG. 
			
				This means a lateral velocity of omega * b. For the rear wheels, this is -omega * c. 
			
				Note the sign reversal. To express this as an angle, take  the arctangent of the lateral velocity divided by the longtitudinal 
				velocity (just like we did for beta).  
				
 			    For small angles we can approximate arctan(vy/vx) by vx/vy.

											 	 /~_
 											    /    ~/
											   /  *  /-.
											  /_    /   \
			                                     ~_/	\|/

			    The steering angle (delta) is the angle that the front wheels make relative to the orientation of the car. There is no steering angle for the rear 
				wheels, these are always in line with the car body orientation. If the car is reversing, the effect of the steering is also reversed. 
			 */

            // Calculate slip angles for front and rear wheels (a.k.a. alpha)
            slipAngleFrontSideways = (float)Math.Atan2( /*vy*/ velocityInLocalCarCoords.sideways + yawSpeedFront,
                                                        /*vx*/ Math.Abs(velocityInLocalCarCoords.forwardsBackwards)
                                                       ) - Math.Sign(velocityInLocalCarCoords.forwardsBackwards) * frontWheelSteerAngle;

            slipAngleRearSideways = (float)Math.Atan2( /*vy*/ velocityInLocalCarCoords.sideways + yawSpeedRear,
                                                       /*vx*/ Math.Abs(velocityInLocalCarCoords.forwardsBackwards));
        }

        /// <summary>
        /// Weight on axles based on centre of gravity and weight shift due to forward/reverse acceleration.
        /// </summary>
        private void CalculateWeightTransfer()
        {
            /*
			    An important effect when accelerating or braking is the effect of dynamic weight transfer.  
				When braking hard the car will nosedive.  During accelerating, the car leans back.  
			    
			    This is because just like the driver is pushed back in his seat when the pedal hits the metal, 
			    so is the car's centre of mass. The effect of this is that the weight on the rear wheels increases 
				during acceleration and the front wheels conversely have less weight to bear.
				
				The effect of weight transfer is important for driving games for two reasons.  First of all the visual 
			    effect of the car pitching in response to driver actions adds a lot of realism to the game. Suddenly, the
			    simulation becomes a lot more lifelike in the user's experience.

				Second of all, the weight distribution dramatically affects the maximum traction force per wheel. This is 
			    because there is a friction limit for a wheel that is proportional to the load on that wheel:

					Fmax = mu * W
						where mu is the friction coefficient of the tyre. For street tyres this may be 1.0, 
						for racing car tyres this can get as high as 1.5.

				For a stationary vehicle the total weight of the car (W, which equals M *g) is distributed over the front and 
				rear wheels according to the distance of the rear and front axle to the CM (c and b respectively):
				
					Wf = (c/L)*W
					Wr = (b/L)*W
						where b is the distance from CG to front axle, c the distance from CG to rear axle and L is the wheelbase.

				If the car is accelerating or decelerating at rate a, the weight on front (Wf) and rear axle (Wr) can be calculated as follows:
					Wf = (c/L)*W - (h/L)*M*a
					Wr = (b/L)*W + (h/L)*M*a,
						where h is the height of the CG, M is the car's mass and a is the acceleration (negative in case of deceleration).

				Note that if the CG is further to the rear (c < b), then more weight falls on the rear axle and vice versa. Makes sense, doesn't it?

				If you want to simplify this, you could assume that the static weight distribution is 50-50 over the front and rear axle. 
			    In other words, assume b = c = L/2. In that case, Wf = 0.5*W - (h/L)*M*a and Wr = 0.5*W +(h/L)*M*a;

			*/

            //         |    AXLE    |
            //        \|/          \|/
            //
            //       [###]    cg  [###]
            //  F  +---|-------:----|----+
            //  R  |   |       :    |    | R
            //  O  |   |       X    |    | E
            //  N  |   |       :    |    | A 
            //  T  +---|-------:----|----+ R
            //       [###]        [###]

            // weightTransfer is to apportion the amount of acceleration / deceleration to the front or rear wheels 

            // Wf = (c/L)*W - (h/L)*M*a,   c/L is axleWeightRatioFront, h is Config.centreOfGravityHeight, L=wheel base, a=accelInLocalCarCoords.X, W = Config.massOfKartPlusDriver x Gravity
            frontAxleWeightInNewtons = Config.massOfKartPlusDriverKg * (axleWeightRatioFront * Config.c_gravity - Config.weightTransfer * accelInLocalCarCoords.forwardsBackwards * Config.centreOfGravityHeight / wheelBase);

            // Wr = (b/L)*W + (h/L)*M*a,   c/L is axleWeightRatioRear, h is Config.centreOfGravityHeight, L=wheel base, a=accelInLocalCarCoords.X, W = Config.massOfKartPlusDriver x Gravity
            rearAxleWeightInNewtons = Config.massOfKartPlusDriverKg * (axleWeightRatioRear * Config.c_gravity + Config.weightTransfer * accelInLocalCarCoords.forwardsBackwards * Config.centreOfGravityHeight / wheelBase);
        }

        /// <summary>
        /// Calculations of Yaw.
        /// </summary>
        private void CalculateYaw()
        {
            // v = yawrate * r where r is distance from axle to centre of gravity and yawRate (angular velocity) in rad/s.
            yawSpeedFront = Config.centreOfGravityToFrontAxle * angularVelocityInRadians;
            yawSpeedRear = -Config.centreOfGravityToRearAxle * angularVelocityInRadians;

            // remember yaw is opposite between front and back, so rear is MINUS, front is NEGATIVE
        }

        /// <summary>
        /// Clamps min/max of value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private static double Clamp(double value, double min, double max)
        {
            if (value.CompareTo(min) < 0) return min;

            if (value.CompareTo(max) > 0) return max;

            return value;
        }

        /// <summary>
        /// Smooth the steering by applying maximum steering angle change velocity.
        /// </summary>
        /// <param name="steerInput"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        double ApplySmoothSteer(double steerInput, double dt)
        {
            if (Math.Abs(steerInput) > 0.001)
            {
                //  Move toward steering input
                return Clamp(amountOfSteeringInput + steerInput * dt * 7.0F, -1.0F, 1.0F);
            }

            if (amountOfSteeringInput > 0)
            {
                return Math.Max(amountOfSteeringInput - dt * 1.0F, 0);
            }

            if (amountOfSteeringInput < 0)
            {
                return Math.Min(amountOfSteeringInput + dt * 1.0F, 0);
            }

            return 0; // straight ahead, nothing to adjust
        }

        /// <summary>
        /// Safe Steering: Limit the steering angle by the speed of the car preventing oversteer at expense of more understeer.
        /// </summary>
        /// <param name="steerInput"></param>
        /// <returns></returns>
        double ApplySafeSteer(double steerInput)
        {
            return steerInput * (1.0F - (Math.Min(absoluteVelocityMSInWorldCoords, 250.0F) / /* m/s */ 280.0F));
        }
    }
}