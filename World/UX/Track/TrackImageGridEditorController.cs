using System.Diagnostics;
using System.Text;
using AICarTrack;
using AICarTrack.World.UX.Track;

namespace CarsAndTanks.World.UX.Track;

/// <summary>
/// Shapes supported: represent the 2 connection points of the track (l=left,r=right,t=top,b=bottom). e.g. l2r is left to right, r2t is right to top.
/// </summary>
internal enum TrackShapes
{
    LeftToRight, LeftToTop, LeftToBottom,
    TopToBottom, TopToLeft, TopToRight,
    RightToLeft, RightToBottom, RightToTop,
    BottomToTop, BottomToLeft, BottomToRight
};

/// <summary>
/// Controller for track image grid editor.
/// </summary>
internal class TrackImageGridEditorController
{
    /// <summary>
    /// Call back function when the track is complete
    /// </summary>
    internal delegate void TrackImageGridComplete();

    /// <summary>
    /// Contains the sillouette of the track drawn to it on completion.
    /// </summary>
    internal Bitmap? TrackSillouette;

    /// <summary>
    /// Contains the track image drawn to it on completion
    /// </summary>
    internal Bitmap? TrackImage;

    /// <summary>
    /// The start point on the image/sillouette.
    /// </summary>
    internal Point StartingPointOfCars = new(0, 0);

    /// <summary>
    /// Heading from start line derived. Convention is the order the track tiles are laid determines direction the cars travel.
    /// </summary>
    internal int HeadingFromStartBasedOnOrderTracksWereLaidOut = 0;

    /// <summary>
    /// One half of the gate point.
    /// </summary>
    internal List<Point> Gates1 = new();

    /// <summary>
    /// Other half of the gate point.
    /// </summary>
    internal List<Point> Gates2 = new();

    /// <summary>
    /// Width of the grid area, based on tile size.
    /// </summary>
    internal int widthOfCanvas = 0;

    /// <summary>
    /// Height of the grid area, based on tile size.
    /// </summary>
    internal int heightOfCanvas = 0;

    /// <summary>
    /// This is the pen used to draw the grid lines.
    /// </summary>
    private readonly Pen penGridLines = new(Color.FromArgb(255, 220, 220, 220));

    /// <summary>
    /// This is the colour of squares that can be clicked.
    /// </summary>
    private readonly Pen penToShowASquareIsClickable = new(Color.Magenta, 2);

    /// <summary>
    /// This is the colour used to highlight the square last added (current).
    /// </summary>
    private readonly Pen penToHighlightCurrentSquare = new(Color.Black, 2);

    /// <summary>
    /// Convert a mouse position into a coordinate.
    /// </summary>
    /// <param name="mousePos"></param>
    /// <returns></returns>
    private Point CoordinateFromMousePos(Point mousePos)
    {
        return new Point(mousePos.X / TrackImagesCache.c_sizeOfTile, mousePos.Y / TrackImagesCache.c_sizeOfTile);
    }

    /// <summary>
    /// This is the canvas we are drawing to (a picture box).
    /// </summary>
    private PictureBox Canvas { get; set; }

    /// <summary>
    /// This contains the track parts.
    /// </summary>
    private readonly List<Point> trackParts = new();

    /// <summary>
    /// This contains squares that can be clicked.
    /// </summary>
    private readonly List<Point> clickableSquares = new();

    /// <summary>
    /// This is called when the track is complete.
    /// </summary>
    private readonly TrackImageGridComplete callBackTrackComplete;

    /// <summary>
    /// This is the control containing the editor picture box..
    /// </summary>
    private readonly Control containerControl;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="control"></param>
    /// <param name="callback"></param>
    /// <param name="canvasWidth"></param>
    /// <param name="canvasHeight"></param>
    internal TrackImageGridEditorController(Control control, TrackImageGridComplete callback, int canvasWidth, int canvasHeight)
    {
        callBackTrackComplete = callback;
        containerControl = control;

        widthOfCanvas = canvasWidth;
        heightOfCanvas = canvasHeight;

        int tileSize = TrackImagesCache.c_sizeOfTile;

        widthOfCanvas = widthOfCanvas / tileSize * tileSize + 1; // +1 for border
        heightOfCanvas = heightOfCanvas / tileSize * tileSize + 1; // +1 for border

        penGridLines.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

        // canvas is a "picture box", so when attached to a container control it renders without hooking into "paint" handler.
        PictureBox pictureBox = new()
        {
            Dock = DockStyle.Fill
        };
        Canvas = pictureBox;

        // allow user to click where they want track
        Canvas.MouseClick += TrackGridEditor_MouseClick;

        // allow user to click [Esc] to finish, or [Del] to remove track
    }

    /// <summary>
    /// Saves the track for this editor to a file.
    /// </summary>
    /// <param name="filename"></param>
    internal void SaveTrack(string filename)
    {
        // don't allow anything other than .track files to be written
        if (Path.GetExtension(filename).ToLower() != ".track") return;

        StringBuilder serialisedTrack = new();
        serialisedTrack.AppendLine("Editor"); // this allows the "load" to distinguish between random/user painted vs. editor tracks

        foreach (Point p in trackParts)
        {
            serialisedTrack.AppendLine($"{p.X},{p.Y}");
        }

        File.WriteAllText(filename, serialisedTrack.ToString());
    }

    /// <summary>
    /// Loads the track for this editor from a file.
    /// </summary>
    /// <param name="filename"></param>
    internal void LoadTrack(string filename)
    {
        string[] lines = File.ReadAllLines(filename);

        trackParts.Clear();

        LearningAndRaceManager.s_cars.Clear();

        foreach (string line in lines)
        {
            if (!line.Contains(',')) continue; // skip header

            string[] data = line.Split(','); // "x,y"
            Point p = new(int.Parse(data[0]), int.Parse(data[1]));
            trackParts.Add(p);
        }

        // fake what happens when users do it. This renders the required images and invokes the callback.
        CompleteTrackIfStartAndEndAreNearlyTouching();
    }

    /// <summary>
    /// Used has picked "Editor", we attach events for the UI (click/keydown)
    /// </summary>
    /// <param name="control"></param>
    internal void StartTrackConstructor()
    {
        containerControl.Controls.Add(Canvas);

        Draw();

        // picturebox > trackPanel > panelContent > form            
        Canvas.Parent.Parent.KeyDown += TrackGridEditor_KeyDown;

        Canvas.Cursor = Cursors.Hand;
    }

    /// <summary>
    /// Handle [Escape] and [Delete].
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TrackGridEditor_KeyDown(object? sender, KeyEventArgs e)
    {
        if (trackParts.Count == 0) return;

        Point p = trackParts[^1];

        Point delta = new(0, 0);
        switch (e.KeyCode)
        {
            case Keys.Escape:
                CompleteTrackIfStartAndEndAreNearlyTouching(); break;

            case Keys.Delete:
                DeleteLastTrackPart();
                return;

            // don't currently run
            case Keys.Up:
                delta.Y = -1;
                break;

            case Keys.Down:
                delta.Y = 1;
                break;

            case Keys.Left:
                delta.X = -1;
                break;

            case Keys.Right:
                delta.X = 1;
                break;
        }

        p.X += delta.X;
        p.Y += delta.Y;

        bool found = false;

        foreach (Point cs in clickableSquares)
        {
            if (cs.X == p.X && cs.Y == p.Y)
            {
                found = true;
                break;
            }
        }

        if (!found) return; // direction was not a clickable direction.

        trackParts.Add(p);
        ComputeAvailableSquaresThatCanBeAddedToTrackBaseOn(p); // determine what squares are clickable now.
        Draw();
    }

    /// <summary>
    /// User clicked [Del]. If are parts of track laid out on screen (1 or more), we remove the last square track laid and repaint UI.
    /// </summary>
    private void DeleteLastTrackPart()
    {
        trackParts.RemoveAt(trackParts.Count - 1);

        if (trackParts.Count > 0)
        {
            ComputeAvailableSquaresThatCanBeAddedToTrackBaseOn(trackParts[^1]); // determine what squares are clickable now.
        }
        else
        {
            clickableSquares.Clear(); // no clickable squares
        }

        Draw();
    }

    /// <summary>
    /// Join start and end of track to "finish" the editing. It invokes the callback to say "we're done, cars can drive".
    /// </summary>
    private void CompleteTrackIfStartAndEndAreNearlyTouching()
    {
        Point[] trackPartsArray = trackParts.ToArray();

        // join last with first track when it makes sense to. Less than 3 does not!
        if (trackPartsArray.Length <= 3) return;

        Point pStart = trackPartsArray[0];
        Point pEnd = trackPartsArray[^1];

        if (Math.Abs(pStart.X - pEnd.X) + Math.Abs(pStart.Y - pEnd.Y) == 1)
        {
            trackParts.Add(pStart);
        }
        else
        {
            if (Canvas.Parent != null) return; // ignore [Esc], we cannot join the track
        }

        // draw the track and sillouette for our "caller"

        clickableSquares.Clear();
        Draw(showGrid: false, sillouette: true);

        TrackSillouette = new Bitmap(Canvas.Image);

        Draw(showGrid: false, sillouette: false);
        TrackImage = new Bitmap(Canvas.Image);

        // we need to remove this, as it's attached to the form, not our editor
        // picturebox > trackPanel > panelContent > form            
        if (Canvas.Parent != null) Canvas.Parent.Parent.KeyDown -= TrackGridEditor_KeyDown;

        containerControl.Controls.Remove(Canvas);
        callBackTrackComplete.Invoke();
    }

    /// <summary>
    /// Draws the track editor canvas.
    /// </summary>
    private void Draw(bool showGrid = true, bool sillouette = false)
    {
        int tileSize = TrackImagesCache.c_sizeOfTile;

        Canvas.Width = widthOfCanvas;
        Canvas.Height = heightOfCanvas;

        Bitmap bmpTrackBeingRendered = new(widthOfCanvas, heightOfCanvas);

        using Graphics g = Graphics.FromImage(bmpTrackBeingRendered);

        if (!sillouette) DrawGrass(tileSize, g); else g.FillRectangle(new SolidBrush(Color.White), 0, 0, bmpTrackBeingRendered.Width, bmpTrackBeingRendered.Height);

        Point[] trackPartsArray = trackParts.ToArray();

        OverlayTrackTilesAndCalculateGateCoordinates(tileSize, g, trackPartsArray, sillouette);

        if (!sillouette) DrawCheckeredFlag(g); // we don't want white blobs (grass) messing up grass detection as it is part of the flag.

        if (trackPartsArray.Length > 3)
        {
            Point pStart = trackPartsArray[0];
            Point pEnd = trackPartsArray[^1];

            if (pStart.X == pEnd.X && pStart.Y == pEnd.Y)
            {
                clickableSquares.Clear();
                showGrid = false;
            }
        }

        if (showGrid && !sillouette)
        {
            OverlayGrid(bmpTrackBeingRendered, g);
            OverlayClickableSquares(tileSize, g);

            string text = "";

            if (trackPartsArray.Length == 0)
            {
                text = "Click the square indicating the start point of the track";
            }
            else
            {
                text = "Click on a pink square to choose where to place the next track piece.";

                Point p1 = trackParts[0];
                Point p2 = trackParts[^1];
                int dx = Math.Abs(p1.X - p2.X);
                int dy = Math.Abs(p1.Y - p2.Y);

                if (trackParts.Count > 3 && (dx == 1 && dy == 0 || dx == 0 && dy == 1))
                {
                    text += "\nPress [Escape] to complete the loop.";
                }
            }

            using Font font = new("Arial", 18);
            SizeF size = g.MeasureString(text, font);

            g.DrawString(text, font, Brushes.White, 10, 10);
        }

        // before replacing the image, we should get rid of the previous one.
        if (Canvas.Image != null) Canvas.Image.Dispose();

        Canvas.Image = bmpTrackBeingRendered;
    }

    /// <summary>
    /// Overlay the squares that can be clicked on to the track.
    /// </summary>
    /// <param name="tileSize"></param>
    /// <param name="g"></param>
    private void OverlayClickableSquares(int tileSize, Graphics g)
    {
        if (trackParts.Count == 0) return; // no track parts

        Point[] trackPartsArray = trackParts.ToArray();

        Point lastPointClicked = trackPartsArray[^1];

        int x = lastPointClicked.X * tileSize;
        int y = lastPointClicked.Y * tileSize;

        g.DrawRectangle(penToHighlightCurrentSquare, x + 2, y + 2, tileSize - 4, tileSize - 4);

        foreach (Point p in clickableSquares)
        {
            g.DrawRectangle(penToShowASquareIsClickable, p.X * tileSize, p.Y * tileSize, tileSize, tileSize);
        }
    }

    /// <summary>
    /// Draw grass squares.
    /// </summary>
    /// <param name="tileSize"></param>
    /// <param name="g"></param>
    private void DrawGrass(int tileSize, Graphics g)
    {
        // draw the "grass" on all squares
        for (int x = 0; x <= widthOfCanvas + 1; x += tileSize)
        {
            for (int y = 0; y <= heightOfCanvas; y += tileSize)
            {
                g.DrawImage(TrackImagesCache.s_TileableBitmapOfGrass, new Rectangle(x, y, tileSize, tileSize));
            }
        }
    }

    /// <summary>
    /// Draw track tiles.
    /// </summary>
    /// <param name="tileSize"></param>
    /// <param name="g"></param>
    /// <param name="trackPartsArray"></param>
    /// <param name="sillouette"></param>
    private void OverlayTrackTilesAndCalculateGateCoordinates(int tileSize, Graphics g, Point[] trackPartsArray, bool sillouette = false)
    {
        Gates1.Clear();
        Gates2.Clear();

        // draw track shapes
        for (int i = 0; i < trackPartsArray.Length; i++)
        {
            Point part = trackPartsArray[i];
            TrackShapes s = GetShape(trackPartsArray, i);

            Bitmap b = GetShapeBitmapFromShape(s, sillouette);

            int x = part.X * tileSize;
            int y = part.Y * tileSize;

            if (i < trackPartsArray.Length - 1) // start repeated at end mean -1
                AddGatesBasedOnShape(s, x, y, i);

            g.DrawImage(b, new Rectangle(x, y, b.Width, b.Height));
        }
    }

    /// <summary>
    /// Adds the required gates based on the track shape. 
    /// Curves have 3 gates, horiz/vert have multiple gates. 
    /// 
    /// Direction *matters* too as they need to be in sequence ascending order to work.
    /// </summary>
    /// <param name="trackShape"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="indexInTrackParts"></param>
    /// <exception cref="ArgumentException"></exception>
    private void AddGatesBasedOnShape(TrackShapes trackShape, int x, int y, int indexInTrackParts)
    {
        double signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder;

        int roadwidth = Settings.Config.s_settings.World.RoadWidthInPixels;

        int radius = TrackImagesCache.c_sizeOfTile / 2;
        int innerRadius = radius - (roadwidth / 2 + 1);
        int outerRadius = radius + roadwidth / 2;

        switch (trackShape)
        {
            /* 
             *  --+   | 
             *    |   +--
             */
            case TrackShapes.LeftToBottom:
            case TrackShapes.BottomToLeft:
                signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder = (trackShape == TrackShapes.BottomToLeft ? -1 : 1) * 22.5;

                // x,y = top left [working]
                Gates1.Add(new Point(x + (int)(0.5F + Math.Cos(MathUtils.DegreesInRadians(-signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder - 45)) * innerRadius),
                                     y + TrackImagesCache.c_sizeOfTile + (int)(0.5F + Math.Sin(MathUtils.DegreesInRadians(-signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder - 45)) * innerRadius))); //     /
                Gates2.Add(new Point(x + (int)(0.5F + Math.Cos(MathUtils.DegreesInRadians(-signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder - 45)) * outerRadius),
                                     y + TrackImagesCache.c_sizeOfTile + (int)(0.5F + Math.Sin(MathUtils.DegreesInRadians(-signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder - 45)) * outerRadius))); //    /

                Gates1.Add(new Point(x + (int)(0.5F + Math.Cos(MathUtils.DegreesInRadians(-45)) * innerRadius),
                                     y + TrackImagesCache.c_sizeOfTile + (int)(0.5F + Math.Sin(MathUtils.DegreesInRadians(-45)) * innerRadius))); //     /
                Gates2.Add(new Point(x + (int)(0.5F + Math.Cos(MathUtils.DegreesInRadians(-45)) * outerRadius),
                                     y + TrackImagesCache.c_sizeOfTile + (int)(0.5F + Math.Sin(MathUtils.DegreesInRadians(-45)) * outerRadius))); //    /

                Gates1.Add(new Point(x + (int)(0.5F + Math.Cos(MathUtils.DegreesInRadians(-45 + signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder)) * innerRadius),
                                     y + TrackImagesCache.c_sizeOfTile + (int)(0.5F + Math.Sin(MathUtils.DegreesInRadians(-45 + signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder)) * innerRadius))); //     /
                Gates2.Add(new Point(x + (int)(0.5F + Math.Cos(MathUtils.DegreesInRadians(-45 + signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder)) * outerRadius),
                                     y + TrackImagesCache.c_sizeOfTile + (int)(0.5F + Math.Sin(MathUtils.DegreesInRadians(-45 + signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder)) * outerRadius))); //    /
                break;

            case TrackShapes.RightToTop:
            case TrackShapes.TopToRight:
                // x,y = top left [working]            
                signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder = (trackShape == TrackShapes.TopToRight ? -1 : 1) * 22.5;

                Gates1.Add(new Point(x + TrackImagesCache.c_sizeOfTile - (int)(0.5F + Math.Cos(MathUtils.DegreesInRadians(45 + signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder)) * innerRadius),
                                     y + (int)(0.5F + Math.Sin(MathUtils.DegreesInRadians(45 + signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder)) * innerRadius))); //     /
                Gates2.Add(new Point(x + TrackImagesCache.c_sizeOfTile - (int)(0.5F + Math.Cos(MathUtils.DegreesInRadians(45 + signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder)) * outerRadius),
                                     y + (int)(0.5F + Math.Sin(MathUtils.DegreesInRadians(45 + signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder)) * outerRadius))); //    /                    

                Gates1.Add(new Point(x + TrackImagesCache.c_sizeOfTile - (int)(0.5F + Math.Cos(MathUtils.DegreesInRadians(45)) * innerRadius),
                                     y + (int)(0.5F + Math.Sin(MathUtils.DegreesInRadians(45)) * innerRadius))); //     /
                Gates2.Add(new Point(x + TrackImagesCache.c_sizeOfTile - (int)(0.5F + Math.Cos(MathUtils.DegreesInRadians(45)) * outerRadius),
                                     y + (int)(0.5F + Math.Sin(MathUtils.DegreesInRadians(45)) * outerRadius))); //    /                    

                Gates1.Add(new Point(x + TrackImagesCache.c_sizeOfTile - (int)(0.5F + Math.Cos(MathUtils.DegreesInRadians(45 - signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder)) * innerRadius),
                                     y + (int)(0.5F + Math.Sin(MathUtils.DegreesInRadians(45 - signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder)) * innerRadius))); //     /
                Gates2.Add(new Point(x + TrackImagesCache.c_sizeOfTile - (int)(0.5F + Math.Cos(MathUtils.DegreesInRadians(45 - signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder)) * outerRadius),
                                     y + (int)(0.5F + Math.Sin(MathUtils.DegreesInRadians(45 - signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder)) * outerRadius))); //    /                    
                break;

            /*
             *   ---
             * 
             */
            case TrackShapes.LeftToRight:
                if (indexInTrackParts == 0) HeadingFromStartBasedOnOrderTracksWereLaidOut = 0;

                for (int a = 0; a < TrackImagesCache.c_sizeOfTile; a += Settings.Config.s_settings.World.GateThresholdInPixels)
                {
                    if (indexInTrackParts == 0 && a < TrackImagesCache.c_sizeOfTile / 2) continue;

                    Gates1.Add(new Point(x + a, y + TrackImagesCache.c_sizeOfTile / 2 - roadwidth / 2 - 1));
                    Gates2.Add(new Point(x + a, y + TrackImagesCache.c_sizeOfTile / 2 + roadwidth / 2));
                }

                break;

            case TrackShapes.RightToLeft:
                if (indexInTrackParts == 0) HeadingFromStartBasedOnOrderTracksWereLaidOut = -180;

                for (int a = TrackImagesCache.c_sizeOfTile; a >= 0; a -= Settings.Config.s_settings.World.GateThresholdInPixels)
                {
                    if (indexInTrackParts == 0 && a + 1 > TrackImagesCache.c_sizeOfTile / 2) continue;

                    Gates1.Add(new Point(x + a, y + TrackImagesCache.c_sizeOfTile / 2 - roadwidth / 2 - 1));
                    Gates2.Add(new Point(x + a, y + TrackImagesCache.c_sizeOfTile / 2 + roadwidth / 2));
                }

                break;

            case TrackShapes.TopToBottom:
                if (indexInTrackParts == 0) HeadingFromStartBasedOnOrderTracksWereLaidOut = 90;

                for (int a = 0; a < TrackImagesCache.c_sizeOfTile; a += Settings.Config.s_settings.World.GateThresholdInPixels)
                {
                    if (indexInTrackParts == 0 && a < TrackImagesCache.c_sizeOfTile / 2) continue;

                    Gates1.Add(new Point(x + TrackImagesCache.c_sizeOfTile / 2 - roadwidth / 2 - 1, y + a));
                    Gates2.Add(new Point(x + TrackImagesCache.c_sizeOfTile / 2 + roadwidth / 2, y + a));
                }

                break;

            case TrackShapes.BottomToTop:
                if (indexInTrackParts == 0) HeadingFromStartBasedOnOrderTracksWereLaidOut = -90;

                for (int a = TrackImagesCache.c_sizeOfTile; a >= 0; a -= Settings.Config.s_settings.World.GateThresholdInPixels)
                {
                    if (indexInTrackParts == 0 && a > TrackImagesCache.c_sizeOfTile / 2) continue;
                    Gates1.Add(new Point(x + TrackImagesCache.c_sizeOfTile / 2 - roadwidth / 2 - 1, y + a));
                    Gates2.Add(new Point(x + TrackImagesCache.c_sizeOfTile / 2 + roadwidth / 2, y + a));
                }

                break;

            /*         
             *   l2t   r2b
             *     |   +---
             *   --+   |  
             */
            case TrackShapes.LeftToTop:
            case TrackShapes.TopToLeft:
                // x,y = top left ***
                signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder = (trackShape == TrackShapes.LeftToTop ? -1 : 1) * 22.5;

                Gates1.Add(new Point(x + (int)(0.5F + Math.Cos(MathUtils.DegreesInRadians(45 - signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder)) * innerRadius),
                                     y + (int)(0.5F + Math.Sin(MathUtils.DegreesInRadians(45 - signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder)) * innerRadius))); //     /
                Gates2.Add(new Point(x + (int)(0.5F + Math.Cos(MathUtils.DegreesInRadians(45 - signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder)) * outerRadius),
                                     y + (int)(0.5F + Math.Sin(MathUtils.DegreesInRadians(45 - signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder)) * outerRadius))); //    /

                Gates1.Add(new Point(x + (int)(0.5F + Math.Cos(MathUtils.DegreesInRadians(45)) * innerRadius),
                                     y + (int)(0.5F + Math.Sin(MathUtils.DegreesInRadians(45)) * innerRadius))); //     /
                Gates2.Add(new Point(x + (int)(0.5F + Math.Cos(MathUtils.DegreesInRadians(45)) * outerRadius),
                                     y + (int)(0.5F + Math.Sin(MathUtils.DegreesInRadians(45)) * outerRadius))); //    /

                Gates1.Add(new Point(x + (int)(0.5F + Math.Cos(MathUtils.DegreesInRadians(45 + signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder)) * innerRadius),
                                     y + (int)(0.5F + Math.Sin(MathUtils.DegreesInRadians(45 + signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder)) * innerRadius))); //     /
                Gates2.Add(new Point(x + (int)(0.5F + Math.Cos(MathUtils.DegreesInRadians(45 + signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder)) * outerRadius),
                                     y + (int)(0.5F + Math.Sin(MathUtils.DegreesInRadians(45 + signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder)) * outerRadius))); //    /

                break;

            case TrackShapes.RightToBottom:
            case TrackShapes.BottomToRight:
                // x,y = top left [working]
                signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder = (trackShape == TrackShapes.RightToBottom ? -1 : 1) * 22.5; // one code for either direction

                Gates1.Add(new Point(x + (int)(0.5F + TrackImagesCache.c_sizeOfTile - Math.Cos(MathUtils.DegreesInRadians(45 - signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder)) * innerRadius),
                                     y + TrackImagesCache.c_sizeOfTile - (int)(0.5F + Math.Sin(MathUtils.DegreesInRadians(45 - signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder)) * innerRadius))); //     /
                Gates2.Add(new Point(x + (int)(0.5F + TrackImagesCache.c_sizeOfTile - Math.Cos(MathUtils.DegreesInRadians(45 - signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder)) * outerRadius),
                                     y + TrackImagesCache.c_sizeOfTile - (int)(0.5F + Math.Sin(MathUtils.DegreesInRadians(45 - signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder)) * outerRadius))); //    /


                Gates1.Add(new Point(x + (int)(0.5F + TrackImagesCache.c_sizeOfTile - Math.Cos(MathUtils.DegreesInRadians(45)) * innerRadius),
                                     y + TrackImagesCache.c_sizeOfTile - (int)(0.5F + Math.Sin(MathUtils.DegreesInRadians(45)) * innerRadius))); //     /
                Gates2.Add(new Point(x + (int)(0.5F + TrackImagesCache.c_sizeOfTile - Math.Cos(MathUtils.DegreesInRadians(45)) * outerRadius),
                                     y + TrackImagesCache.c_sizeOfTile - (int)(0.5F + Math.Sin(MathUtils.DegreesInRadians(45)) * outerRadius))); //    /

                Gates1.Add(new Point(x + (int)(0.5F + TrackImagesCache.c_sizeOfTile - Math.Cos(MathUtils.DegreesInRadians(45 + signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder)) * innerRadius),
                                     y + TrackImagesCache.c_sizeOfTile - (int)(0.5F + Math.Sin(MathUtils.DegreesInRadians(45 + signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder)) * innerRadius))); //     /
                Gates2.Add(new Point(x + (int)(0.5F + TrackImagesCache.c_sizeOfTile - Math.Cos(MathUtils.DegreesInRadians(45 + signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder)) * outerRadius),
                                     y + TrackImagesCache.c_sizeOfTile - (int)(0.5F + Math.Sin(MathUtils.DegreesInRadians(45 + signAdjustmentToEnsureCurveGatesAreInTheCorrectOrder)) * outerRadius))); //    /

                break;

            default: throw new ArgumentException(nameof(trackShape) + " unsupported shape.");
        }
    }

    /// <summary>
    /// Draw the grid on top of the track.
    /// </summary>
    /// <param name="tileSize"></param>
    /// <param name="bmp"></param>
    /// <param name="g"></param>
    private void OverlayGrid(Bitmap bmp, Graphics g)
    {
        // overlay grid
        for (int x = 0; x < Canvas.Width; x += TrackImagesCache.c_sizeOfTile)
        {
            g.DrawLine(penGridLines, new Point(x, 0), new Point(x, bmp.Height));
        }

        for (int y = 0; y < Canvas.Height; y += TrackImagesCache.c_sizeOfTile)
        {
            g.DrawLine(penGridLines, new Point(0, y), new Point(bmp.Width, y));
        }
    }

    /// <summary>
    /// Determine the shape for element [i], based on the shape before and after.
    /// </summary>
    /// <param name="trackPartsArray"></param>
    /// <param name="i"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private TrackShapes GetShape(Point[] trackPartsArray, int i)
    {
        if (trackPartsArray.Length == 1) return TrackShapes.LeftToRight;

        TrackShapes s;

        // last square
        if (i >= trackPartsArray.Length - 1)
        {
            // -.
            //  |                
            Point p0 = trackPartsArray[i - 1];
            Point p1 = trackPartsArray[i];

            int deltaX10 = p0.X - p1.X;
            int deltaY10 = p0.Y - p1.Y;

            return (deltaX10, deltaY10) switch
            {
                // ---
                (-1, 0) => TrackShapes.LeftToRight,
                (1, 0) => TrackShapes.RightToLeft,

                // |
                // |
                (0, -1) => TrackShapes.BottomToTop,
                (0, 1) => TrackShapes.TopToBottom,

                _ => throw new ArgumentException(nameof(trackPartsArray)),
            };

        }

        if (i > 0 && i < trackPartsArray.Length - 1)
        {
            // we need 3 points to determine the shape for [i]. We're doing it one behind the current square as it's
            // not until we have it, do we know whether we need a corner connector.
            Point p0 = trackPartsArray[i - 1];
            Point p1 = trackPartsArray[i]; // we're calculating this one.
            Point p2 = trackPartsArray[i + 1];

            int deltaX10 = p1.X - p0.X;
            int deltaY10 = p1.Y - p0.Y;

            int deltaX21 = p2.X - p1.X;
            int deltaY21 = p2.Y - p1.Y;

            // based on how much square 1 is offset from square 0, and square 2 from square 1 we
            // can determine accurately the orientation/shape of trackParts[i].
            s = (deltaX21, deltaY21, deltaX10, deltaY10) switch
            {
                //   __   
                //  /
                //  |  
                (1, 0, 0, -1) => TrackShapes.BottomToRight,
                (0, 1, -1, 0) => TrackShapes.RightToBottom,

                // __
                //   \
                //   |
                (-1, 0, 0, -1) => TrackShapes.BottomToLeft,
                (0, 1, 1, 0) => TrackShapes.LeftToBottom,

                // |
                // \__
                (1, 0, 0, 1) => TrackShapes.TopToRight,
                (0, -1, -1, 0) => TrackShapes.RightToTop,

                //    |
                // __/
                (-1, 0, 0, 1) => TrackShapes.TopToLeft,
                (0, -1, 1, 0) => TrackShapes.LeftToTop,

                // ----
                (-1, 0, -1, 0) => TrackShapes.RightToLeft,
                (1, 0, 1, 0) => TrackShapes.LeftToRight,

                // |
                // |
                (0, 1, 0, 1) => TrackShapes.TopToBottom,
                (0, -1, 0, -1) => TrackShapes.BottomToTop,

                // impossible scenarios.
                (0, 1, 0, -1) or (-1, 0, 1, 0) or (1, 0, -1, 0) or (0, -1, 0, 1) => throw new Exception("track double backs on itself"),

                _ => throw new ArgumentException(nameof(trackPartsArray)),
            };
        }
        else
        {
            Point p0 = trackPartsArray[i];
            Point p1 = trackPartsArray[i + 1];

            int deltaX10 = p1.X - p0.X;
            int deltaY10 = p1.Y - p0.Y;

            s = (deltaX10, deltaY10) switch
            {
                (-1, 0) => TrackShapes.RightToLeft,
                (1, 0) => TrackShapes.LeftToRight,
                (0, -1) => TrackShapes.BottomToTop,
                (0, 1) => TrackShapes.TopToBottom,
                _ => throw new ArgumentException(nameof(trackPartsArray)),
            };
        }

        return s;
    }

    /// <summary>
    /// Converts a "shape" into an image.
    /// </summary>
    /// <param name="trackShape"></param>
    /// <returns></returns>
    private Bitmap GetShapeBitmapFromShape(TrackShapes trackShape, bool sillouette)
    {
        if (sillouette)
        {
            return trackShape switch
            {
                TrackShapes.LeftToBottom => TrackImagesCache.s_leftToBottomTrackBitmapSillouette,
                TrackShapes.BottomToLeft => TrackImagesCache.s_leftToBottomTrackBitmapSillouette,

                TrackShapes.LeftToRight => TrackImagesCache.s_leftToRightTrackBitmapSillouette,
                TrackShapes.RightToLeft => TrackImagesCache.s_leftToRightTrackBitmapSillouette,

                TrackShapes.TopToBottom => TrackImagesCache.s_topToBottomTrackBitmapSillouette,
                TrackShapes.BottomToTop => TrackImagesCache.s_topToBottomTrackBitmapSillouette,

                TrackShapes.LeftToTop => TrackImagesCache.s_leftToTopTrackBitmapSillouette,
                TrackShapes.TopToLeft => TrackImagesCache.s_leftToTopTrackBitmapSillouette,

                TrackShapes.RightToBottom => TrackImagesCache.s_rightToBottomTrackBitmapSillouette,
                TrackShapes.BottomToRight => TrackImagesCache.s_rightToBottomTrackBitmapSillouette,

                TrackShapes.RightToTop => TrackImagesCache.s_topToRightTrackBitmapSillouette,
                TrackShapes.TopToRight => TrackImagesCache.s_topToRightTrackBitmapSillouette,

                _ => throw new ArgumentException(nameof(trackShape) + " unsupported shape."),
            };
        }
        else
        {
            return trackShape switch
            {
                TrackShapes.LeftToBottom => TrackImagesCache.s_leftToBottomTrackBitmap,
                TrackShapes.BottomToLeft => TrackImagesCache.s_leftToBottomTrackBitmap,

                TrackShapes.LeftToRight => TrackImagesCache.s_leftToRightTrackBitmap,
                TrackShapes.RightToLeft => TrackImagesCache.s_leftToRightTrackBitmap,

                TrackShapes.TopToBottom => TrackImagesCache.s_topToBottomTrackBitmap,
                TrackShapes.BottomToTop => TrackImagesCache.s_topToBottomTrackBitmap,

                TrackShapes.LeftToTop => TrackImagesCache.s_leftToTopTrackBitmap,
                TrackShapes.TopToLeft => TrackImagesCache.s_leftToTopTrackBitmap,

                TrackShapes.RightToBottom => TrackImagesCache.s_rightToBottomTrackBitmap,
                TrackShapes.BottomToRight => TrackImagesCache.s_rightToBottomTrackBitmap,

                TrackShapes.RightToTop => TrackImagesCache.s_topToRightTrackBitmap,
                TrackShapes.TopToRight => TrackImagesCache.s_topToRightTrackBitmap,

                _ => throw new ArgumentException(nameof(trackShape) + " unsupported shape."),
            };
        }
    }

    /// <summary>
    /// User clicked on a square of the grid.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TrackGridEditor_MouseClick(object? sender, MouseEventArgs e)
    {
        Point p = CoordinateFromMousePos(new Point(e.X, e.Y));

        if (clickableSquares.Count == 0 && trackParts.Count > 3) return; // track finished, don't allow click.

        // 0 => you can click anywhere.
        if (clickableSquares.Count > 0)
        {
            bool allowed = false;

            // is the point clicked one of the clickable squares?
            foreach (Point ps in clickableSquares)
            {
                if (ps.X == p.X && ps.Y == p.Y)
                {
                    allowed = true;
                    break;
                }
            }

            if (!allowed) return; // not allowed to click on any of the clickable squares (would cross a track).
        }

        trackParts.Add(p); // add this part of the track

        ComputeAvailableSquaresThatCanBeAddedToTrackBaseOn(p); // determine what squares are clickable now.

        //CompleteTheLoop();

        Draw(); // repaint the grid.
    }

    /// <summary>
    /// Automagic connecting the loop, when adjacent.
    /// </summary>
    private void CompleteTheLoop()
    {
        Point p1 = trackParts[0];
        Point p2 = trackParts[^1];
        int dx = Math.Abs(p1.X - p2.X);
        int dy = Math.Abs(p1.Y - p2.Y);

        if (trackParts.Count > 3 && (dx == 1 && dy == 0 || dx == 0 && dy == 1))
        {
            //CompleteTrackIfStartAndEndAreNearlyTouching();
        }
    }

    /// <summary>
    /// Determine which squares are ok for the user to click on.
    /// Rule: offer left, right, above, below except if that square is already occupied.
    /// 
    ///                                        A    B    C    D    E    F 
    /// +----+----+----+----+----+----+      +----+----+----+----+----+----+
    /// |    |    |    |    |    |    |      |    |    |    |    |    |    | 1
    /// +----+----+----+----+----+----+      +----+----+----+----+----+----+
    /// |    |    | ok |    |    |    |      |    |    |    |    |    |    | 2
    /// +----+----+----+----+----+----+      +----+----+----+----+----+----+
    /// |    | ok |====| ok |    |    |      |    |    |====|=\\ |    |    | 3
    /// +----+----+----+----+----+----+      +----+----+----+----+----+----+
    /// |    |    | ok |    |    |    |      |    |    | ok | || | ok |    | 4
    /// +----+----+----+----+----+----+      +----+----+----+----+----+----+
    /// |    |    |    |    |    |    |      |    |    |    | ok |    |    | 5
    /// +----+----+----+----+----+----+      +----+----+----+----+----+----+
    /// </summary>
    /// <param name="currentSquare">The square last added.</param>
    private void ComputeAvailableSquaresThatCanBeAddedToTrackBaseOn(Point currentSquare)
    {
        clickableSquares.Clear(); // assume NONE are clickable

        // clickable squares are above, below, left or right of the point
        Point[] squaresIn4Directions = { new Point(currentSquare.X - 1, currentSquare.Y),   // <
                                         new Point(currentSquare.X + 1, currentSquare.Y),   // >
                                         new Point(currentSquare.X, currentSquare.Y + 1),   // /\
                                         new Point(currentSquare.X, currentSquare.Y - 1) }; // \/

        // test each of the directions
        foreach (Point squareToCheck in squaresIn4Directions)
        {
            // outside the grid?
            if (squareToCheck.X < 0 || squareToCheck.Y < 0) continue;
            if (squareToCheck.X > widthOfCanvas / TrackImagesCache.c_sizeOfTile || squareToCheck.Y > heightOfCanvas / TrackImagesCache.c_sizeOfTile) continue;

            bool squareAlreadyOccupiedOnTrackGrid = false;

            // have we already got this square in use?
            foreach (Point tp in trackParts)
            {
                if (tp.X == squareToCheck.X && tp.Y == squareToCheck.Y) // e.g square D3 in above diagram
                {
                    squareAlreadyOccupiedOnTrackGrid = true; // yes
                    break;
                }
            }

            // if square is available, add to clickable squares
            if (!squareAlreadyOccupiedOnTrackGrid) clickableSquares.Add(squareToCheck);
        }
    }

    /// <summary>
    /// Draws the checkered flag on the road depending on top-down, left-right.
    /// 
    /// ------------------
    /// [##][  ][##][  ][##][  ][##][  ][##]
    /// [  ][##][  ][##][  ][##][  ][##][  ]
    /// [##][  ][##][  ][##][  ][##][  ][##]
    /// ------------------  
    /// </summary>
    /// <param name="graphics"></param>
    internal void DrawCheckeredFlag(Graphics graphics)
    {
        if (trackParts.Count < 2) return; // insufficient squares to determine direction of start line.

        Point[] trackPartsArray = trackParts.ToArray();

        Point pStart = trackPartsArray[0];

        int xOfCenterOfStartLine = pStart.X * TrackImagesCache.c_sizeOfTile + TrackImagesCache.c_sizeOfTile / 2;
        int yOfCenterOfStartLine = pStart.Y * TrackImagesCache.c_sizeOfTile + TrackImagesCache.c_sizeOfTile / 2;

        // store the start point
        StartingPointOfCars.X = xOfCenterOfStartLine;
        StartingPointOfCars.Y = yOfCenterOfStartLine;

        Point pEnd = trackPartsArray[1];

        int sizeOfCheckerSquare = 3;

        using SolidBrush brushBlackPaint = new(Color.FromArgb(230, 0, 0, 0));
        using SolidBrush brushWhitePaint = new(Color.FromArgb(230, 255, 255, 255));

        int initialWhiteBlackToggle = 0;

        int halfroadwidth = Settings.Config.s_settings.World.RoadWidthInPixels / 2;

        bool drawingLeftToRight = pStart.X == pEnd.X;

        // [##][  ][##][  ][##][  ][##][  ][##]
        for (int indexAcrossRoad = -halfroadwidth + 1; indexAcrossRoad <= halfroadwidth - 3; indexAcrossRoad += sizeOfCheckerSquare)
        {
            // 1st column starts "xx"; 2nd column starts "  "; 3rd column starts "xx"...
            initialWhiteBlackToggle = 1 - initialWhiteBlackToggle;

            int whiteBlackToggle = 0;

            // ----    ----
            // [##] or [  ]
            // [  ]    [##]
            // [##]    [  ]
            // ----    ----
            for (int indexOfWidthOfCheckeredRegion = -sizeOfCheckerSquare * 2; indexOfWidthOfCheckeredRegion <= sizeOfCheckerSquare * 2; indexOfWidthOfCheckeredRegion += sizeOfCheckerSquare)
            {
                // we could be drawing a start line vertically or horizontally depending on the direction the user laid them out.
                if (drawingLeftToRight)
                {
                    // "left-right"
                    graphics.FillRectangle(whiteBlackToggle == initialWhiteBlackToggle ? brushWhitePaint : brushBlackPaint,
                                            new Rectangle(xOfCenterOfStartLine + indexAcrossRoad, yOfCenterOfStartLine + indexOfWidthOfCheckeredRegion, sizeOfCheckerSquare, sizeOfCheckerSquare));
                }
                else
                {
                    // "top-down";
                    graphics.FillRectangle(whiteBlackToggle == initialWhiteBlackToggle ? brushWhitePaint : brushBlackPaint,
                                            new Rectangle(xOfCenterOfStartLine + indexOfWidthOfCheckeredRegion, yOfCenterOfStartLine + indexAcrossRoad, sizeOfCheckerSquare, sizeOfCheckerSquare));
                }

                whiteBlackToggle = 1 - whiteBlackToggle; // next square is inverted colour - "xx" => "  " => "xx" => "  " ...
            }
        }
    }
}