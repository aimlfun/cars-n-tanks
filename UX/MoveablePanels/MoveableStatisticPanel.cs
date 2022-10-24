using AICarTrack;
using CarsAndTanks.Settings;

namespace UX.MoveablePanels
{
    internal class MoveableStatisticPanel : MoveableSemiTransparentPanel
    {
        private string text = "";

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pb"></param>
        /// <param name="r"></param>
        internal MoveableStatisticPanel(Panel pb, Rectangle r) : base(pb, r)
        {
        }

        /// <summary>
        /// Overriden. Used to draw the contents of the panel. (statistics)
        /// </summary>
        /// <param name="g"></param>
        internal override void Draw(Graphics g)
        {
            if (!Visible) return;

            string debug =
                $"Generation: {LearningAndRaceManager.s_generation}\n" +
                $"Quiet Mode: {(LearningAndRaceManager.SilentLearning ? "YES" : "NO")}\n" +
                $"Moves before mutation: {LearningAndRaceManager.s_movesLeftBeforeNextMutation}\n" +
                text;

            using Font f = new("Courier New", 7);

            SizeF s = g.MeasureString(debug.ToString(), f);

            PanelSize.Width = (int)s.Width + 20;
            PanelSize.Height = (int)s.Height + 20;
            base.Draw(g);


            g.DrawString(debug.ToString(), f, Brushes.White, Location.X + 10, Location.Y + 10);
        }

        /// <summary>
        /// Save the location (invoked on mouse-up).
        /// </summary>
        protected override void Save()
        {
            Config.s_settings.Display.LocationOfMoveableStatisicsPane = Location;
            Config.Save();
        }

        internal void SetText(string txt)
        {
            text = txt;
        }
    }
}
