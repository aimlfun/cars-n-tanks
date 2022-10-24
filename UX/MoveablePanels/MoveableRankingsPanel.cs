using CarsAndTanks.Settings;

namespace UX.MoveablePanels;

/// <summary>
/// Used for the "ranking" panel. It differs in that this specific one write the rankings (gates etc).
/// </summary>
internal class MoveableRankingsPanel : MoveableSemiTransparentPanel
{
    string debug = "";

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="pb"></param>
    /// <param name="r"></param>
    internal MoveableRankingsPanel(Panel pb, Rectangle r) : base(pb, r)
    {
    }

    /// <summary>
    /// A mechanism for the controller to provide output for this panel.
    /// </summary>
    /// <param name="text"></param>
    internal void SetData(string text)
    {
        debug = text;
    }

    /// <summary>
    /// Overriden. Used to draw the contents of the panel. (Ranking).
    /// </summary>
    /// <param name="g"></param>
    internal override void Draw(Graphics g)
    {
        if (!Visible) return;

        if (string.IsNullOrWhiteSpace(debug)) return;

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
        Config.s_settings.Display.LocationOfMoveableRankingsPane = Location;
        Config.Save();
    }
}
