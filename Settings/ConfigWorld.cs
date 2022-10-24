namespace AICarTrack.Settings
{
    /// <summary>
    /// Configuration related to the world the AI lives in.
    /// NOTE: get/setters must be public for reflection to work, i.e. save.
    /// </summary>
    internal class ConfigWorld
    {
        /// <summary>
        /// True - we are simulating real world physics, rather than basic physics.
        /// </summary>
        public bool UsingRealWorldPhysics { get; set; } = false;

        /// <summary>
        /// Defines how wide the car is.
        /// </summary>
        public int DiameterOfCarInPixels { get; set; } = 50; // px

        /// <summary>
        /// Defines the width of the road.
        /// </summary>
        public int RoadWidthInPixels { get; set; } = 80; // px 

        /// <summary>
        /// Defines distance between gates. Complied with except where the vertex calculations
        /// cannot ensure no gates overlap (pointed corners)
        /// </summary>
        public int GateThresholdInPixels { get; set; } = 10;// px

        /// <summary>
        /// Used to ensure the polygon tracks do not touch the edge.
        /// </summary>
        public int OffsetOfGeneratedTrackFromEdgeInPixels { get; set; } = 50; // px

        /// <summary>
        /// Determines the base size of polygon track radius.
        /// </summary>
        public int PolygonRadiusOfGeneratedTracksInPixels { get; set; } = 300; // px

        /// <summary>
        /// Defines how much the polygon created track can wiggle.
        /// </summary>
        public int PolygonMaxWiggleInPixels { get; set; } = 40; // px                               
    }
}