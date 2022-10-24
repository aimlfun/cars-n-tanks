using AICarTrack.World.Telemetry;
using CarsAndTanks.Settings;
using System.Drawing.Imaging;
using System.Text;

namespace AICarTrack
{
    /// <summary>
    /// Responsible for starting learning and races, from creation of the NN assigned to a car to asking cars to move around the track
    /// </summary>
    public static class LearningAndRaceManager
    {
        /// <summary>
        /// Modes that change behaviour.
        /// </summary>
        internal enum LearningRaceMode { random50pct, optimise }

        #region PRIVATE
        /// <summary>
        /// A timer is called when this amount of time has elapsed.
        /// </summary>
        private const int c_timeInMSBetweenCarMoves = 5; //ms

        /// <summary>
        /// Indicates whether learning is in progress.
        /// </summary>
        private static bool s_learningInProgress = false;

        /// <summary>
        /// How many moves we've done for this mutation. Enables those that don't move to be eliminated quickly.
        /// </summary>
        private static int s_movesMadeByCars = 0;

        /// <summary>
        /// A simple timer approach that moves the cars at a fixed cadence.
        /// </summary>
        private static System.Windows.Forms.Timer s_timerForCarMove = new();
        #endregion

        #region INTERNAL

        internal static LearningRaceMode s_learningRaceMode = LearningRaceMode.random50pct;

        /// <summary>
        /// Track telemetry for last completed lap.
        /// </summary>
        internal static Dictionary<int, List<TelemetryData>> s_telemetryPerCar = new();

        /// <summary>
        /// 
        /// </summary>
        private static List<TelemetryData> s_telemetryOfBestCarAfterLaps = new();

        internal static int s_telemetryCar = -1;

        internal static List<TelemetryData> TelemetryOfBestCarAfterLaps
        {
            get { return s_telemetryOfBestCarAfterLaps; }
            set
            {
                s_telemetryCar = s_currentBestCarId;
                s_telemetryOfBestCarAfterLaps = value;
            }
        }

        /// <summary>
        /// This is the ID of the best car. -1 = no best car.
        /// </summary>
        internal static int s_currentBestCarId = -1; // no car selected

        /// <summary>
        /// 
        /// </summary>
        internal static int s_lastLapBestCarId = -1; // no car selected

        /// <summary>
        /// This is the start point of learning / race.
        /// </summary>
        internal static PointF s_startPoint = new();

        /// <summary>
        /// Contains the track segments (a point defines center within edge of a track. 
        /// </summary>
        internal readonly static List<PointF> s_trackSegments = new();

        /// <summary>
        /// One half of the gate. (A gate is 2 pairs of coordinates aka perpendicular line coordinates).
        ///</summary>
        internal static PointF[] s_gatesHalf1 = Array.Empty<PointF>();

        /// <summary>
        /// Second half of the gate. (A gate is 2 pairs of coordinates aka perpendicular line coordinates).
        /// </summary>
        internal static PointF[] s_gatesHalf2 = Array.Empty<PointF>();

        /// <summary>
        /// Indicates everything is initialised and the cars can learn.
        /// </summary>
        internal static bool s_initialised = false;

        /// <summary>
        /// The generation (how many times the network has been mutated).
        /// </summary>
        internal static float s_generation = 0;

        /// <summary>
        /// Used to determine which was best number of gates crossed since last mutation.
        /// </summary>
        internal static int s_bestGatePassed = 0;

        /// <summary>
        /// The list of cars indexed by their "id".
        /// </summary>
        internal readonly static Dictionary<int, VehicleDrivenByAI> s_cars = new();

        /// <summary>
        /// If set to true, then this ignores requests to mutate.
        /// </summary>
        internal static bool s_stopMutation = false;

        /// <summary>
        /// Defines the number of moves it will initialise the mutate counter to.
        /// This number increases with each generation.
        /// </summary>
        internal static int s_movesToCountBetweenEachMutation = 0;

        /// <summary>
        /// Defines the number of moves before a mutation occurs. 
        /// This is decremented each time the cars move, and upon reaching zero triggers
        /// a mutation.
        /// </summary>
        internal static int s_movesLeftBeforeNextMutation = 0;

        /// <summary>
        /// Indicates whether the learning is in progress.
        /// </summary>
        internal static bool InLearning
        {
            get { return s_learningInProgress; }
        }

        /// <summary>
        /// 
        /// </summary>
        internal static int NumberOfMovesMadeByCars
        {
            get => s_movesMadeByCars;
            set
            {
                if (value < 1)
                {
                    //Debugger.Break();
                }

                s_movesMadeByCars = value;
            }
        }
        #endregion

        /// <summary>
        /// When true, it's learning silently (much quicker). We don't need the UI to work out the best car/ai, it's
        /// there to help humans visualise the result.
        /// </summary>
        private static bool s_quietLearn = false;

        internal static bool SilentLearning
        {
            get { return s_quietLearn; }
        }

        internal static void StopImmediate()
        {
            s_quietLearn = false;
        }

        /// <summary>
        /// [Quiet Mode] [Visual Mode] clicked.
        /// When "quiet" no rendering of track or cars occurs. The AI process is hugely accelerated.
        /// Visual mode is of course more fun, watching the cars skip around!
        /// </summary>
        internal static void ToggleQuietMode()
        {
            s_quietLearn = !s_quietLearn;

            SetQuietModeOnOff(s_quietLearn); // reduces or increases time
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="g1"></param>
        /// <param name="g2"></param>
        internal static void AssignGates(List<Point> g1, List<Point> g2)
        {
            List<PointF> temp = new();
            foreach (Point p in g1) temp.Add(new PointF(p.X, p.Y));
            s_gatesHalf1 = temp.ToArray();

            temp.Clear();
            foreach (Point p in g2) temp.Add(new PointF(p.X, p.Y));
            s_gatesHalf2 = temp.ToArray();
        }

        /// <summary>
        /// Adds a segment of the road to the list of tracks.
        /// </summary>
        /// <param name="roadSegment"></param>
        internal static void AddTrackSegment(PointF roadSegment)
        {
            s_trackSegments.Add(roadSegment);
        }

        /// <summary>
        /// Removes the track segments
        /// </summary>
        internal static void RemoveAllTrackSegments()
        {
            s_trackSegments.Clear();
            s_gatesHalf1 = Array.Empty<PointF>();
            s_gatesHalf2 = Array.Empty<PointF>();
        }

        /// <summary>
        /// Start the AI learning process.
        /// </summary>
        internal static void StartLearning()
        {
            StopLearning();

            s_telemetryOfBestCarAfterLaps = new();
            s_generation = 0;

            s_learningRaceMode = LearningRaceMode.random50pct;

            if (Config.s_settings.AI.NumberOfAICarsToCreate % 2 != 0) Config.s_settings.AI.NumberOfAICarsToCreate++; // we cannot mutate 50% if it's odd, so we make it even 

            if (VehicleDrivenByAI.c_hardCodedBrain) Config.s_settings.AI.NumberOfAICarsToCreate = 1; // one car as they all drive identically

            InitialiseTheNeuralNetworksForTheCars(); // this can include loading from disk

            InitialiseCars(); // creates the cars

            // initialise the cars to mutate at a pre-determined point
            s_movesToCountBetweenEachMutation = Config.s_settings.AI.CarMovesBeforeFirstMutation;
            s_movesLeftBeforeNextMutation = s_movesToCountBetweenEachMutation;
            s_currentBestCarId = -1;
            s_lastLapBestCarId = (VehicleDrivenByAI.c_hardCodedBrain) ? 0 : -1;
            s_telemetryOfBestCarAfterLaps.Clear();
            s_telemetryPerCar.Clear();
            s_learningInProgress = true;

            // start the timer that moves the cars.
            InitialiseAndStartMoveTimer();
        }

        /// <summary>
        /// Initialise the timer. It requires us to dispose of any existing timer, otherwise we end up with
        /// multiple timers all calling the move cars. Everything seems faster without, but we've lost control.
        /// We cannot cancel or pause the timer...
        /// </summary>
        private static void InitialiseAndStartMoveTimer()
        {
            s_timerForCarMove?.Dispose();

            s_timerForCarMove = new System.Windows.Forms.Timer();
            s_timerForCarMove.Tick += MoveCars_Tick;
            s_timerForCarMove.Interval = c_timeInMSBetweenCarMoves;

            s_timerForCarMove.Start();
        }

        internal static void SetQuietModeOnOff(bool value)
        {
            s_timerForCarMove.Interval = value ? 1 : c_timeInMSBetweenCarMoves;
            s_timerForCarMove.Start();
        }

        /// <summary>
        /// Start the AI learning process.
        /// </summary>
        internal static void ResumeLearning()
        {
            // start the independent timers
            s_learningInProgress = true;

            s_timerForCarMove.Start();
        }

        /// <summary>
        /// Stops the learning process if running.
        /// </summary>
        internal static void StopLearning()
        {
            s_quietLearn = false;
            s_bestGatePassed = -1;
            s_currentBestCarId = -1;

            s_timerForCarMove.Stop();
            
            if (!s_learningInProgress) return; // no need to stop

            s_learningInProgress = false;
        }

        /// <summary>
        /// All the cars have crashed, we need to reset them. The reset includes mutation of bottom 50%.
        /// </summary>
        internal static void NotifyAllCrashed()
        {
            s_movesLeftBeforeNextMutation = s_movesToCountBetweenEachMutation;

            // race mode car should NOT crash, if it does, learning should commence
            
            InitialiseCars();
        }

        /// <summary>
        /// Mutate timer has elapsed, it's normally time to rate the neural network quality per car
        /// then clone & mutate the worst 50%. But the problem with that approach is that if the AI
        /// makes little progress in the time allotted, the time is wasted; versus if the time is 
        /// short the AI has no time to learn. To alleviate this an optimisation is applied. First
        /// it extends the time between each mutate (cars go further and learn more). It also kills
        /// cars that aren't moving between gates.
        /// </summary>
        private static void TimeToMutate()
        {
            if (!s_learningInProgress ) return; // mutation only happens during learning

            s_currentBestCarId = -1; // no car selected

            int pos = DetermineBestCarAndEliminateThoseMovingBackwards();

            // enable each mutation to have longer to run, so the cars go further.
            s_movesToCountBetweenEachMutation = (int)(s_movesToCountBetweenEachMutation * (100 + Config.s_settings.AI.PercentageIncreaseBetweenMutations) / 100);

            s_movesLeftBeforeNextMutation = s_movesToCountBetweenEachMutation;

            // if it did better than last time, we postpone mutation
            if (pos >= s_bestGatePassed)
            {
                s_bestGatePassed = pos;
                return;
            }

            // after a mutation, we crush cars and get new ones (reset their position/state) whilst keeping the neural networks
            InitialiseCars();
        }

        /// <summary>
        /// Review where each car got to, and track the best. Also eliminate those moving backwards. Doing so improves performance
        /// with less cars to move unproductively, and ensures they get mutated.
        /// </summary>
        /// <returns></returns>
        private static int DetermineBestCarAndEliminateThoseMovingBackwards()
        {
            int pos = -1;

            for (int i = 0; i < s_cars.Count; i++)
            {
                if (s_cars[i].CurrentGate > pos)
                {
                    pos = s_cars[i].CurrentGate;
                }

                if (s_cars[i].LastGatePassed == s_cars[i].CurrentGate)
                {
                    s_cars[i].Eliminate(VehicleDrivenByAI.EliminationReasons.gateMutate);
                }
            }

            return pos;
        }

        /// <summary>
        /// Request to force a mutation. We stop the mutation timer, perform the action and restart it.
        /// If we didn't reset the timer, the model will regress as cars won't have had a chance to move to their fullest.
        /// </summary>
        internal static void ForceMutate()
        {
            s_timerForCarMove.Stop();
            TimeToMutate(); // pretend the timer fired
            s_timerForCarMove.Start();
        }

        /// <summary>
        /// Initialises the neural network (one per car).
        /// </summary>
        internal static void InitialiseTheNeuralNetworksForTheCars()
        {
            NeuralNetwork.s_networks.Clear();

            for (int i = 0; i < Config.s_settings.AI.NumberOfAICarsToCreate; i++)
            {
                _ = new NeuralNetwork(i, Config.s_settings.AI.Layers);
            }

            s_bestGatePassed = 0; // best is defined as the car in the lead during training
        }

        /// <summary>
        /// Initialises cars.
        /// 
        /// If there are cars on the track it destroys them but leaving the AI, which it mutates the bottom 50% of cars.
        /// It then adds new cars (reset to start) attached to the AI. The high achievers should take the same path as last time, 
        /// and the poor achievers should be replaced with equivalent cars (although some will be better, some worse depending on 
        /// the effect of mutation.
        /// </summary>
        internal static void InitialiseCars()
        {
            s_generation++;

            NumberOfMovesMadeByCars = 0;

            // existing cars removed, and poor 50% have their brains mutated
            if (s_cars.Count > 0)
            {
                if (s_learningInProgress) MutateCars();

                s_cars.Clear();
            }

            // create cars with their respective brain attached (create is simpler than resetting)
            // race mode is ONE car
            for (int i = 0; i <  Config.s_settings.AI.NumberOfAICarsToCreate; i++)
            {
                s_cars.Add(i, new(i));
            }
        }

        /// <summary>
        /// Computes the gates for the tracks.
        /// </summary>
        /// <param name="v"></param>
        internal static void InitialiseCustomTracks()
        {
            s_trackSegments.Add(s_trackSegments[0]);

            s_startPoint = s_trackSegments[0];

            List<PointF> points1 = new();
            List<PointF> points2 = new();

            int RoadWidth = Config.s_settings.World.RoadWidthInPixels;
            int RoadWidthMargin = (RoadWidth * 20) / 35;

            // road segments are a list of lines
            for (int i = 1; i < s_trackSegments.Count; i++)
            {
                PointF start = s_trackSegments[i - 1];
                PointF end = s_trackSegments[i];

                float x = end.X - start.X;
                float y = end.Y - start.Y;

                /*       2                                                                     145
                 *       |\                                                                      \  90
                 *       | \                          90 deg = 1.5708                             \ |
                 *       |  \                         30 deg = 0.523599                         180 -  0 
                 *       |   \                       180 deg = PI                                 / |
                 *       |    \                                                                  / 270
                 *       -------  145.52342463217851787                                         235
                 *     0        1                                            
                 *     
                 */

                // using Pythagoras theorem, compute the length of the line
                double distanceBetweenPoints = Math.Sqrt(x * x + y * y);

                // if the line is too long the AI won't pass gates and learn, so we break the
                // line up into chunks each with a gate
                // how many gates do we need?
                double gatesRequiredForLengthOfLine = distanceBetweenPoints / Config.s_settings.World.GateThresholdInPixels;

                // dx / dy is how much each "gate" is separated
                double dx = x / gatesRequiredForLengthOfLine;
                double dy = y / gatesRequiredForLengthOfLine;

                double angle = (dy == 0) ? 0 : Math.Asin(x / distanceBetweenPoints);

                angle -= Math.PI / 2;

                float xdirection = -1;
                float ydirection = -1;

                if (dy < 0 && dx > 0)
                {
                    angle -= Math.PI / 2;
                }

                if (dy < 0 && dx < 0)
                {
                    ydirection = 1;
                    xdirection = 1;
                    angle += Math.PI / 2;
                }

                if (dy > 0 && dx < 0)
                {
                    ydirection = -1;
                    xdirection = 1;
                    angle += Math.PI / 2 - Math.PI;
                }

                if (dy > 0 && dx >= 0)
                {
                    ydirection = -1;
                    xdirection = 1;
                    angle += Math.PI / 2;
                }

                // add a gate in between (not the start of 0, or end point)
                for (float z = 0; z < gatesRequiredForLengthOfLine; z++)
                {
                    double dx1 = Math.Cos(angle) * xdirection * (RoadWidth / 2); // aqua
                    double dy1 = Math.Sin(angle) * ydirection * (RoadWidth / 2); // aqua

                    // create a point on the line
                    double x1 = (start.X + z * dx) + dx1;
                    double y1 = (start.Y + z * dy) + dy1;

                    if (Math.Sqrt((x1 - start.X) * (x1 - start.X) + (y1 - start.Y) * (y1 - start.Y)) < RoadWidthMargin) continue; // if too close to the intersection, don't add a gate
                    if (Math.Sqrt((x1 - end.X) * (x1 - end.X) + (y1 - end.Y) * (y1 - end.Y)) < RoadWidthMargin) continue; // if too close to the intersection, don't add a gate

                    double dx2 = Math.Cos(Math.PI + angle) * xdirection * (RoadWidth / 2);
                    double dy2 = Math.Sin(Math.PI + angle) * ydirection * (RoadWidth / 2);

                    double x2 = (start.X + z * dx) + dx2;
                    double y2 = (start.Y + z * dy) + dy2;

                    if (Math.Sqrt((x2 - start.X) * (x2 - start.X) + (y2 - start.Y) * (y2 - start.Y)) < RoadWidthMargin) continue; // if too close to the intersection, don't add a gate
                    if (Math.Sqrt((x2 - end.X) * (x2 - end.X) + (y2 - end.Y) * (y2 - end.Y)) < RoadWidthMargin) continue; // if too close to the intersection, don't add a gate

                    points1.Add(new Point((int)(x1 + 0.5F), (int)(y1 + 0.5F)));
                    points2.Add(new Point((int)(x2 + 0.5F), (int)(y2 + 0.5F)));
                }
            }

            // gates work best when an array with a subscript (simpler to parse)
            s_gatesHalf1 = points1.ToArray();
            s_gatesHalf2 = points2.ToArray();

            // this is so the caller knows we are ready, esp. if they are writing gates to the track
            s_initialised = true;

            FormMain.s_trackCanvas?.ForceRepaintOfTrackAndCars();
        }

        /// <summary>
        /// Saves networks weights and biases to file.
        /// </summary>
        /// <param name="filename"></param>
        internal static void SaveNeuralNetworkStateForCars(string filename)
        {
            foreach (var neuralNetwork in NeuralNetwork.s_networks.Keys)
            {
                NeuralNetwork.s_networks[neuralNetwork].Save(filename.Replace("{{id}}", neuralNetwork.ToString()));
            }
        }

        /// <summary>
        /// Saves networks weights and biases to file.
        /// </summary>
        /// <param name="filename"></param>
        internal static bool LoadNeuralNetworkStateForCars(string filename)
        {
            bool loadedSomething = false;

            foreach (var neuralNetwork in NeuralNetwork.s_networks.Keys)
            {
                loadedSomething |= NeuralNetwork.s_networks[neuralNetwork].Load(filename.Replace("{{id}}", neuralNetwork.ToString()));
            }

            // if we loaded the network, we're generation zero. We don't track what generation is was saved as.
            s_generation = 0;
            
            return loadedSomething;
        }

        /// <summary>
        /// After 2 full laps, we should have viable car. We copy that repeatedly, increasing the chance
        /// of improving the best car even further.
        /// </summary>
        private static void MutateCars()
        {
            s_currentBestCarId = -1; // no car selected

            // update networks fitness for each car
            foreach (int id in s_cars.Keys) s_cars[id].UpdateFitness();

            NeuralNetwork.SortNetworkByFitness(); // largest "fitness" (best performing) goes to the bottom

            // sorting is great but index no longer matches the "id".
            // this is because the sort swaps but this misaligns id with the entry            
            List<NeuralNetwork> n = new();
            foreach (int n2 in NeuralNetwork.s_networks.Keys) n.Add(NeuralNetwork.s_networks[n2]);

            NeuralNetwork[] array = n.ToArray();

            if (s_learningRaceMode == LearningRaceMode.random50pct)
            {
                // replace the 50% worse offenders with the best, then mutate them.
                // we do this by copying top half (lowest fitness) with top half.
                for (int worstNeuralNetworkIndex = 0; worstNeuralNetworkIndex < Config.s_settings.AI.NumberOfAICarsToCreate / 2; worstNeuralNetworkIndex++)
                {
                    // 50..100 (in 100 neural networks) are in the top performing
                    int neuralNetworkToCloneFromIndex = worstNeuralNetworkIndex + Config.s_settings.AI.NumberOfAICarsToCreate / 2; // +50% -> top 50% 

                    NeuralNetwork.CopyFromTo(array[neuralNetworkToCloneFromIndex], array[worstNeuralNetworkIndex]); // copy

                    array[worstNeuralNetworkIndex].Mutate(25, 0.5F); // mutate
                }
            }
            else
            {
                // replace all but top car with a mutated version of the best car
                // mutation chance and strength is halved.
                int NumberToPreserve = ((int)(4 + s_generation / 500 - 4) > Config.s_settings.AI.NumberOfAICarsToCreate) ? Config.s_settings.AI.NumberOfAICarsToCreate - 2 : (int)(4 + (s_generation / 500F));

                int offset = 0;
                // replace all but top rocket with a mutated version of the best rocket
                // mutation chance and strength is halved.
                for (int worstNeuralNetworkIndex = 0; worstNeuralNetworkIndex < Config.s_settings.AI.NumberOfAICarsToCreate - NumberToPreserve; worstNeuralNetworkIndex++)
                {
                    // 50..100 (in 100 neural networks) are in the top performing
                    int neuralNetworkToCloneFromIndex = Config.s_settings.AI.NumberOfAICarsToCreate - 1 - offset; // top rocket

                    NeuralNetwork.CopyFromTo(array[neuralNetworkToCloneFromIndex], array[worstNeuralNetworkIndex]); // copy

                    offset++;
                    if (offset >= NumberToPreserve) offset = 0;

                    array[worstNeuralNetworkIndex].Mutate(1, 0.1F * 20 * 1 / ((float)s_generation / 500)); // mutate
                }

            }

            // unsort, restoring the order of car to neural network i.e [x]=id of "x".
            Dictionary<int, NeuralNetwork> unsortedNetworksDictionary = new();

            for (int carIndex = 0; carIndex < Config.s_settings.AI.NumberOfAICarsToCreate; carIndex++)
            {
                var neuralNetwork = NeuralNetwork.s_networks[carIndex];

                unsortedNetworksDictionary[neuralNetwork.Id] = neuralNetwork;
            }

            NeuralNetwork.s_networks = unsortedNetworksDictionary;
        }


        /// <summary>
        /// Move all the cars (occurs when the timer fires)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MoveCars_Tick(object? sender, EventArgs e)
        {
            if (s_timerForCarMove.Interval == 1) s_timerForCarMove.Stop(); // we're doing it via a loop.

            bool loop = true;

            while (loop)
            {
                // interval 1 means move in a loop, yielding occasionally, it's quicker than waiting 1ms for the car timer.
                if (s_timerForCarMove.Interval != 1 || s_quietLearn == false) loop = false;

                ++NumberOfMovesMadeByCars;

                bool allCarsHaveBeenEliminated = !VehicleDrivenByAI.c_hardCodedBrain;

                int maxGate = 0;

                MoveAllCars(ref allCarsHaveBeenEliminated, ref maxGate, out bool lappedTwice);

                // we enable a caller to attach a monitor to visualise what's happening.
                // if this is violated it is referencing an id that doesn't exist.           
                if (s_lastLapBestCarId > -1 && s_lastLapBestCarId < NeuralNetwork.s_networks.Count) NeuralNetwork.s_networks[s_lastLapBestCarId].Visualise(); // updates the visualiser

                if (!LearningAndRaceManager.SilentLearning) FormMain.s_trackCanvas?.ForceRepaintOfTrackAndCars(); // track is drawn, with cars overlaid

                // has this car gone around the lap twice, if so switch to "optimised" mode (clone top car)
                if (lappedTwice && s_learningRaceMode == LearningRaceMode.random50pct)
                {
                    s_learningRaceMode = LearningRaceMode.optimise;
                    s_movesToCountBetweenEachMutation = NumberOfMovesMadeByCars + 2; // keeps it real
                }

                if (allCarsHaveBeenEliminated) NotifyAllCrashed(); // all cars have collided, we can't continue, so we force a mutate

                // is it time to mutate?
                if ((lappedTwice) || (!s_stopMutation && --s_movesLeftBeforeNextMutation < 1))
                {
                    TelemetryOfBestCarAfterLaps = s_currentBestCarId < 0 || !s_telemetryPerCar.ContainsKey(s_currentBestCarId) ? s_telemetryOfBestCarAfterLaps : s_telemetryPerCar[s_currentBestCarId];
                    TimeToMutate();
                }

                if (s_timerForCarMove.Interval == 1 && NumberOfMovesMadeByCars % 200 == 0) Application.DoEvents();
            }
        }

        /// <summary>
        /// Iterate over all the cars, handling gates.
        /// </summary>
        /// <param name="allCarsHaveBeenEliminated"></param>
        /// <param name="maxGate"></param>
        private static void MoveAllCars(ref bool allCarsHaveBeenEliminated, ref int maxGate, out bool lappedTwice)
        {
            const bool c_moveCarsInParallel = true;
            
            lappedTwice = false;

            if (c_moveCarsInParallel)
            {
                // all cars are independent, each one has a neural network and LIDAR attached, this therefore
                // is a candidate for parallelism.
                Parallel.ForEach(s_cars.Keys, id =>
                    {
                        MoveCar(id);
                    });
            }
            else
            {
                foreach (int id in s_cars.Keys)
                {
                    MoveCar(id);
                }
            }

            // after parallelism, we need to do this to determine best car etc.
            foreach (VehicleDrivenByAI car in s_cars.Values)
            {            
                if (car.HasBeenEliminated) continue;

                if (car.HasLapped)
                {
                    // car has lapped, we store the telemetry for it.
                    if (!s_telemetryPerCar.ContainsKey(car.id))
                        s_telemetryPerCar.Add(car.id, new List<TelemetryData>(car.TelemetryData)); // none for this id, so add
                    else
                        s_telemetryPerCar[car.id] = new List<TelemetryData>(car.TelemetryData); // replace existing telemetry

                    s_lastLapBestCarId = s_currentBestCarId;
                }

                lappedTwice = lappedTwice || car.HasLappedTwice;

                allCarsHaveBeenEliminated = false; // at least one car has not crashed

                if (car.CurrentGate > maxGate)
                {
                    maxGate = car.CurrentGate;
                    s_currentBestCarId = car.id;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        private static void MoveCar(int id)
        {
            VehicleDrivenByAI car = s_cars[id];

            car.Move(out bool carHasLapped);
            car.HasLapped = carHasLapped;

            if (car.lap >= 2) car.HasLappedTwice = true;

            if (!VehicleDrivenByAI.c_hardCodedBrain)
            {
                if (NumberOfMovesMadeByCars > 200 && (car.CurrentGate < 1 || car.CarImplementation.Speed == 0))
                {
                    car.CurrentGate = -3;
                    car.Eliminate(VehicleDrivenByAI.EliminationReasons.moves100);
                }
            }
        }

        /// <summary>
        /// We measure fitness of the neural network based on how many gates the cars went thru. The less gates, the worse the AI.
        /// </summary>
        /// <param name="car"></param>
        /// <param name="gateCarIsAt"></param>
        /// <returns></returns>
        internal static bool CarIsAtGate(VehicleDrivenByAI car, out int gateCarIsAt, out bool lapped)
        {
            int position = car.CurrentGate % s_gatesHalf1.Length;
            int lastGateOnTracks = s_gatesHalf1.Length - 1;

            for (int gate = 0; gate < s_gatesHalf1.Length; gate++)
            {
                if (!MathUtils.IsPointOnLine(s_gatesHalf1[gate], s_gatesHalf2[gate], car.CarImplementation.LocationOnTrack)) continue;

                if (gate == 0 && position == lastGateOnTracks)
                {
                    gateCarIsAt = car.CurrentGate + 1;

                    lapped = true;// car has lapped, increment laps
                    car.lap++;

                    return true;
                }

                // don't lose the wrap around
                gateCarIsAt = car.CurrentGate - position + gate;

                lapped = false;
                return true;
            }

            // car has neither lapped or is at a different gate
            gateCarIsAt = car.CurrentGate;
            lapped = false;

            return false;
        }

        /// <summary>
        /// Saves the track as a CSV without header of the point coordinates.
        /// </summary>
        /// <param name="fileName"></param>
        internal static void SaveTrack(string fileName)
        {
            _ = TrackAndBackgroundCache.GetTrackFromCache();
            Application.DoEvents();

            string imageName = Path.ChangeExtension(fileName, ".png");
            if (!File.Exists(imageName))
            {
                Bitmap thumbnail = new(TrackAndBackgroundCache.CachedImageOfTrack);
                using Image thumbnail2 = ImageUtils.ResizeImage(thumbnail, 230, 148);

                imageName = Path.GetFullPath(imageName);

                thumbnail2.Save(imageName, ImageFormat.Png);
            }

            if (FormMain.trackImageGridController != null && FormMain.trackImageGridController.Gates1.Count > 1)
            {
                FormMain.trackImageGridController.SaveTrack(fileName);
                return;
            }

            StringBuilder serialisedTrack = new();
            serialisedTrack.AppendLine("Painter");

            foreach (PointF p in s_trackSegments)
            {
                serialisedTrack.AppendLine(p.X + "," + p.Y);
            }

            File.WriteAllText(fileName, serialisedTrack.ToString());
        }

        /// <summary>
        /// Loads the track from a CSV without header of the point coordinates.
        /// </summary>
        /// <param name="fileName"></param>
        internal static void LoadTrack(string fileName)
        {
            RemoveAllTrackSegments();

            string[] lines = File.ReadAllLines(fileName);

            TrackAndBackgroundCache.Clear();

            if (lines[0] == "Editor")
            {
                FormMain.trackImageGridController?.LoadTrack(fileName);
                return;
            }

            VehicleDrivenByAI.Heading = 0;

            List<PointF> points = new();

            foreach (string line in lines)
            {
                if (!line.Contains(',')) continue; // skip header
                string[] data = line.Split(','); // "x,y"
                Point p = new((int)float.Parse(data[0]), (int)float.Parse(data[1]));
                points.Add(p);
            }

            points.RemoveAt(points.Count - 1);

            s_trackSegments.AddRange(points);

            FormMain.s_trackCanvas?.ForceRepaintOfTrackAndCars();

            InitialiseCustomTracks();
        }
    }
}