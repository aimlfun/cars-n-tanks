namespace UX.MoveablePanels;

/// <summary>
/// A UI class that provides an alpha overlay over a region, that is draggable via the mouse and capable
/// of saving its location.
/// </summary>
internal class MoveableSemiTransparentPanel
{
    /// <summary>
    /// Top left corner of the panel
    /// </summary>
    internal Point Location;

    /// <summary>
    /// Width and height of the panel.
    /// </summary>
    internal Size PanelSize;

    /// <summary>
    /// Where the user clicked (mouse down). Used to enable us to know the offset so dragging doesn't
    /// make the panel jump on dragging.
    /// </summary>
    private Point mouseDown;

    /// <summary>
    /// Indicates we are dragging.
    /// </summary>
    private bool draggingInProgress = false;

    /// <summary>
    /// Canvas we are painting on (a panel).
    /// </summary>
    private readonly Panel canvas;

    internal bool Visible = false;

    /// <summary>
    /// Constructor.
    /// </summary>
    internal MoveableSemiTransparentPanel(Panel pb, Rectangle r)
    {
        canvas = pb;
        PanelSize = new Size(r.Width, r.Height);
        Location = new Point(r.Left, r.Top);
    }

    /// <summary>
    /// The minimum it does is draw an alpha transparency of the panel size on which visuals
    /// are drawn. Override and call base().
    /// </summary>
    /// <param name="g"></param>
    internal virtual void Draw(Graphics g)
    {
        if (!Visible) return;
        
        // expectation is that this is called after everything is painted (i.e. it is transparent on top)
        g.FillRectangle(new SolidBrush(Color.FromArgb(20, 255, 255, 255)), Location.X, Location.Y, PanelSize.Width, PanelSize.Height);
    }

    /// <summary>
    /// Called by the manager when mouse-down occurs. It needs to check if the point clicked is within the panel.
    /// If within the panel, it starts the dragging progress.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    internal bool MouseDown(MouseEventArgs e)
    {
        if (!Visible) return false;

        Rectangle r = new(Location.X, Location.Y, PanelSize.Width, PanelSize.Height);

        if (!r.Contains(e.Location)) return false;

        mouseDown = e.Location; // so everything is moved relative during drag.

        draggingInProgress = true;

        // attach handlers, which we remove upon stopping drag
        canvas.MouseMove += Canvas_MouseMove;
        canvas.MouseUp += Canvas_MouseUp;

        return true;
    }

    /// <summary>
    /// User released drag operation. It saves the location.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Canvas_MouseUp(object? sender, MouseEventArgs e)
    {
        Location.X += e.X - mouseDown.X;
        Location.Y += e.Y - mouseDown.Y;

        draggingInProgress = false;

        // detach handlers
        canvas.MouseMove -= Canvas_MouseMove;
        canvas.MouseUp -= Canvas_MouseUp;

        Save();
    }

    /// <summary>
    /// Add logic in inherited class to save the position.
    /// </summary>
    protected virtual void Save()
    {
        throw new Exception("Override this method"); // panel inheriting needs to save its coordinates.
    }

    /// <summary>
    /// User is dragging. We move the panel. It will get repainted with the rest of the UI paint.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Canvas_MouseMove(object? sender, MouseEventArgs e)
    {
        Location.X += e.X - mouseDown.X;
        Location.Y += e.Y - mouseDown.Y;

        // we store the position of the mouse, so next time we are moving by delta.
        mouseDown = e.Location;
    }
}