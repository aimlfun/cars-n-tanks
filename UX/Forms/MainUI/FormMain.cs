using CarsAndTanks.Learn;
using CarsAndTanks.AI;
using CarsAndTanks.Settings;
using CarsAndTanks.UX.Forms.Settings;
using CarsAndTanks.UX.MoveablePanels;
using CarsAndTanks.World;
using CarsAndTanks.World.Car;
using CarsAndTanks.World.UX.Track;
using System.Drawing.Imaging;

namespace CarsAndTanks.UX.Forms.MainUI
{
    public partial class FormMain : Form
    {
        /// <summary>
        /// This contains the location and name for the model files. They are saved one per id of network.
        /// </summary>
        private static readonly string s_filePathCarsNTanksAIModel = Path.Combine(Program.applicationUserAIModels, "blobs-n-cars-n-tanks{{id}}.ai");

        /// <summary>
        /// Track the moveable panels.
        /// </summary>
        internal static Dictionary<string, MoveableSemiTransparentPanel> s_moveablePanels = new();

        /// <summary>
        /// This is our "config" (we load settings into)
        /// </summary>
        internal static Config? s_config;

        /// <summary>
        /// This is the canvas we're using (panelTrack to be precise)
        /// </summary>
        internal static FormMain? s_trackCanvas;

        internal Rectangle rectCars = new();

        /// <summary>
        /// Constructor.
        /// </summary>
        public FormMain()
        {
            InitializeComponent(); // standard WinForm, this positions controls

            // make the track panel owner draw double buffered etc.
            panelTrack.GetType().GetMethod("SetStyle", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)?.Invoke(panelTrack, new object[]
            { ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint , true });

            panelTrack.Paint += PanelTrack_Paint;
            this.KeyPreview = true;

            KeyPress += new KeyPressEventHandler(Form1_KeyPress);
            PreviewKeyDown += new PreviewKeyDownEventHandler(Form1_PreviewKeyDown);

            s_config = new Config();
            CreateGlassPanelsAtSavedPosition();

            s_trackCanvas = this;

            panelTrack.Cursor = Cursors.Arrow;
            panelTrack.MouseDown += FormMain_MouseDownGlassPanelCheck;
        }

        /// <summary>
        /// Called when the form loads. It initialises the world-manager and track-image-cache.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_Load(object sender, EventArgs e)
        {
            WorldManager.Initialise();

            TrackAndBackgroundCache.Canvas = panelTrack;

            trackImageGridController = new TrackImageGridEditorController(panelTrack, CallBackTrackImageGridComplete, panelTrack.Width, panelTrack.Height);

            TrackImagesCache.Initialise();
        }

        #region TRACK EDITOR

        internal static TrackImageGridEditorController? trackImageGridController;

        /// <summary>
        /// Callback. User has edited the track. Use the track pattern / sillouette.
        /// </summary>
        private void CallBackTrackImageGridComplete()
        {
            if (trackImageGridController is null || trackImageGridController.TrackImage is null || trackImageGridController.TrackSillouette is null) throw new Exception("Callback from grid failed to finalise images of track and sillouette");

            Bitmap bitmapTrackImage = trackImageGridController.TrackImage;  // used for pretty visuals
            Bitmap bitmapTrackSillouette = trackImageGridController.TrackSillouette;  // used for collision detection

            Point startPoint = trackImageGridController.StartingPointOfCars; // where the cars start wrt to the track
            VehicleDrivenByAI.Heading = trackImageGridController.HeadingFromStartBasedOnOrderTracksWereLaidOut; // direction cars should head (because track has gates numbered sequentially)

            TrackAndBackgroundCache.CachedImageOfTrack = bitmapTrackImage;
            TrackAndBackgroundCache.CachedSillouetteOfTrack = bitmapTrackSillouette;

            // paint the track
            panelTrack.Invalidate();
            Application.DoEvents();

            // initialise the learning with gates and start point
            LearningAndRaceManager.AssignGates(trackImageGridController.Gates1, trackImageGridController.Gates2);
            LearningAndRaceManager.s_startPoint = startPoint;
            LearningAndRaceManager.StartLearning();
        }

        #endregion

        #region TRACK PAINTER
        /// <summary>
        /// 
        /// </summary>
        int lastMouseDownX = -1;

        /// <summary>
        /// 
        /// </summary>
        int lastMouseDownY = -1;

        /// <summary>
        /// 
        /// </summary>
        private void RepaintAfterTrackSegmentChange()
        {
            TrackAndBackgroundCache.Clear();

            panelTrack.Invalidate();
            Application.DoEvents();
        }

        /// <summary>
        /// We're in the editor, and the user pressed a key. 
        /// If it's [ESC] we've finished.
        /// If it's [DEL] we remove the last track segment.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Escape && e.KeyCode != Keys.Delete) return;

            if (e.KeyCode == Keys.Delete)
            {
                // remove the current segment, if one exists
                if (LearningAndRaceManager.s_trackSegments.Count > 0)
                {
                    LearningAndRaceManager.s_trackSegments.Remove(LearningAndRaceManager.s_trackSegments[^1]);
                    RepaintAfterTrackSegmentChange();
                }

                return;
            }

            EndTrackEditing();
        }

        /// <summary>
        /// End the track edit, and start the cars.
        /// </summary>
        private void EndTrackEditing()
        {
            panelTrack.Cursor = Cursors.Arrow;

            // detach handlers as we're done with them

            panelTrack.MouseDown -= Form1_MouseDown;
            panelTrack.MouseMove -= Form1_MouseMove;

            KeyDown -= Form1_KeyDown;

            LearningAndRaceManager.AddTrackSegment(LearningAndRaceManager.s_trackSegments[0]);

            LearningAndRaceManager.InitialiseCustomTracks();

            RepaintAfterTrackSegmentChange();

            LearningAndRaceManager.StartLearning();
        }

        /// <summary>
        /// We're in track painting mode, and user moved mouse.
        /// Draw a line from last point to cursor position.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseMove(object? sender, MouseEventArgs e)
        {
            // ^1 = last segment. We change its X,Y so rubber banding occurs
            LearningAndRaceManager.s_trackSegments[^1] = new Point(e.X, e.Y);

            RepaintAfterTrackSegmentChange();
        }

        /// <summary>
        /// We're in track painting mode, and user clicked mouse left button.
        /// Add track segment from last position to cursor position.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseDown(object? sender, MouseEventArgs e)
        {
            TrackAndBackgroundCache.Clear();

            if (LearningAndRaceManager.s_trackSegments.Count == 0)
            {
                panelTrack.MouseMove += Form1_MouseMove;
            }

            lastMouseDownX = e.X;
            lastMouseDownY = e.Y;

            // point the last one where the mouse is down
            if (LearningAndRaceManager.s_trackSegments.Count > 0) LearningAndRaceManager.s_trackSegments[^1] = new Point(lastMouseDownX, lastMouseDownY);

            // create new one to drag
            LearningAndRaceManager.AddTrackSegment(new Point(lastMouseDownX, lastMouseDownY));

            RepaintAfterTrackSegmentChange();
        }
        #endregion

        /// <summary>
        /// Updates the info and repaints the track
        /// </summary>
        internal void UpdateProgress()
        {
            UpdateInfoPanel();

            panelTrack.Invalidate();
        }

        /// <summary>
        /// Updates info panel with progress.
        /// </summary>
        private static void UpdateInfoPanel()
        {
            string txt = "";

            // show progress.
            if (LearningAndRaceManager.s_currentBestCarId > -1)
            {
                if (LearningAndRaceManager.s_gatesHalf1.Length > 0)
                    txt += $"\nGate: {LearningAndRaceManager.s_cars[LearningAndRaceManager.s_currentBestCarId].CurrentGate} out of {LearningAndRaceManager.s_gatesHalf1.Length}";

                // "*" means it has managed to go a full lap (visual in quiet mode).
                if (!(LearningAndRaceManager.TelemetryOfBestCarAfterLaps is null || LearningAndRaceManager.TelemetryOfBestCarAfterLaps.Count == 0 || TrackAndBackgroundCache.CachedSillouetteOfTrack is null))
                    txt += "\n--lapped--";
            }

            (s_moveablePanels["statistics"] as MoveableStatisticPanel)?.SetText(txt);
        }

        /// <summary>
        /// Makes the track re-paint.
        /// </summary>
        internal void ForceRepaintOfTrackAndCars()
        {
            UpdateInfoPanel();

            // in quiet mode, nothing to paint
            if (LearningAndRaceManager.SilentLearning) return;

            // we're not in quiet mode, so force a paint
            if (rectCars.Width == 0)
            {
                panelTrack.Invalidate();
            }
            else
            {
                panelTrack.Invalidate();// new Region(rectCars));
            }
        }

        /// <summary>
        /// All cars / tracks are drawn to "panelTrack" in the "paint" method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelTrack_Paint(object? sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            //e.Graphics.Clip = new Region(e.ClipRectangle);

            rectCars = WorldManager.DrawCarsAndTrack(e.Graphics, e.ClipRectangle);

            // paint each moveable panel
            foreach (MoveableSemiTransparentPanel panel in s_moveablePanels.Values)
            {
                panel.Draw(e.Graphics);
            }
        }

        /// <summary>
        /// We override, because we need to handle keyboard for drawing.
        /// </summary>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool IsInputKey(Keys keyData)
        {
            bool result;

            if ((keyData & Keys.Right) == Keys.Right | (keyData & Keys.Left) == Keys.Left | (keyData & Keys.Up) == Keys.Up | (keyData & Keys.Down) == Keys.Down)
            {
                result = true;
            }
            else
            {
                result = base.IsInputKey(keyData);
            }

            return result;
        }

        // PreviewKeyDown is where you preview the key.
        // Do not put any logic here, instead use the
        // KeyDown event after setting IsInputKey to true.
        private void Form1_PreviewKeyDown(object? sender, PreviewKeyDownEventArgs e)
        {
            e.IsInputKey = IsInputKey(e.KeyData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyPress(object? sender, KeyPressEventArgs e)
        {
            e.Handled = false;
        }

        /// <summary>
        /// As usual with my apps, we have key presses that do things.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Q:
                    // user pressed "Q" we need to toggle quiet learning mode
                    LearningAndRaceManager.ToggleQuietMode();
                    break;

                case Keys.M:
                    // user pressed "M" to force mutation immediately.
                    LearningAndRaceManager.ForceMutate();
                    break;

                case Keys.D:
                    // User pressed "D" we need to enable/disable mutation.
                    LearningAndRaceManager.s_stopMutation = !LearningAndRaceManager.s_stopMutation;
                    break;

                case Keys.F:
                    // user pressed "F", we output the formula if possible
                    // saved to c:\temp\formula.txt
                    OutputFormulaForBestCar();
                    break;
            }
        }

        /// <summary>
        /// Ensure the form closes. It is prevented if running in quiet mode.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            LearningAndRaceManager.StopImmediate();
        }

        /// <summary>
        /// If in the process of learning, restart it.
        /// </summary>
        private static void RestartLearning()
        {
            if (LearningAndRaceManager.InLearning) LearningAndRaceManager.StopLearning();
            LearningAndRaceManager.StartLearning();
        }

        #region TOP LEVEL MENU ITEM HANDLERS

        // F I L E

        // F I L E  >  E X I T

        /// <summary>
        /// User clicked File -> Exit.
        /// Closes the app.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LearningAndRaceManager.StopImmediate(); // quiet learning will be unresponsive and not close the app without this
            Close();
        }


        // T R A C K

        // T R A C K  >  N E W 

        // T R A C K  >  N E W  >  R A N D O M

        /// <summary>
        /// User clicked menu item: Track > New > Random.
        /// It creates a random distorted polygon track, replacing an existing one.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrackNewRandomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopLearning();

            RandomPolygonTrack.Create();

#if usingCentrePoint
        RaceManager.InitTracks();
#else
            LearningAndRaceManager.s_trackSegments.Add(LearningAndRaceManager.s_trackSegments[0]);
            LearningAndRaceManager.InitialiseCustomTracks();
            TrackAndBackgroundCache.Clear();

#endif
            _ = TrackAndBackgroundCache.GetTrackFromCache();
            Application.DoEvents();
            LearningAndRaceManager.StartLearning();
        }

        /// <summary>
        /// Stops any learning.
        /// </summary>
        private static void StopLearning()
        {
            if (LearningAndRaceManager.InLearning)
            {
                LearningAndRaceManager.StopImmediate();
                LearningAndRaceManager.StopLearning();
            }
        }

        // T R A C K  >  N E W  >  D R A W

        /// <summary>
        /// User clicked menu item: Track > New > Draw.
        /// Initiates the track draw, so that the user can draw a track using lines rather than shapes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrackNewDrawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopLearning();

            panelTrack.MouseDown += Form1_MouseDown;
            KeyDown += Form1_KeyDown;

            TrackAndBackgroundCache.Clear();
            LearningAndRaceManager.s_cars.Clear();
            LearningAndRaceManager.RemoveAllTrackSegments();

            // for the start line to paint correctly, and safe place to start from it adds
            // a straight segment.
            LearningAndRaceManager.AddTrackSegment(new PointF(300, 500));
            LearningAndRaceManager.AddTrackSegment(new PointF(320, 500)); // 300-320,500
            LearningAndRaceManager.s_startPoint = LearningAndRaceManager.s_trackSegments[0];

            LearningAndRaceManager.AddTrackSegment(new PointF(320, 500)); // the segment the user moves, they are drawing from this point
            LearningAndRaceManager.s_initialised = true;

            RepaintAfterTrackSegmentChange();
            panelTrack.Cursor = Cursors.Cross;
        }

        // T R A C K  >  N E W  >  C O N S T R U C T O R

        /// <summary>
        /// User clicked menu item: Track > New > Constructor.
        /// Initiates the track constructor, so that the user can design a track using shapes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrackNewConstructorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LearningAndRaceManager.StopLearning();

            trackImageGridController?.StartTrackConstructor();
        }

        // T R A C K  >  S A V E

        /// <summary>
        /// User clicked menu item: Track > Save.
        /// Ask the race manager to save the current track. 
        /// User is prompted by a dialog choosing the filename before doing so.
        /// </summary>
        private void TrackSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // we should disable save when it's not applicable. We don't, so here we just ignore it
            if ((FormMain.trackImageGridController != null && FormMain.trackImageGridController.Gates1.Count == 0) && LearningAndRaceManager.s_trackSegments.Count == 0)
            {
                MessageBox.Show("No track available on screen to save.\n\nMaybe use the New > Track option?", "Error");
                return;
            }

            using FormSaveTracks formSaveTracks = new();
            formSaveTracks.ShowDialog();
        }

        // T R A C K  >  L O A D

        /// <summary>
        /// User clicked menu item: Track > Load.
        /// Discards any exist track, and loads the one the user picks.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrackLoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using FormLoadTracks formLoadTracks = new(Program.applicationUserTracks);

            // show the modal dialog that allows track selection
            if (formLoadTracks.ShowDialog() != DialogResult.OK) return;

            // "OK" indicates they clicked on a track

            LearningAndRaceManager.StopImmediate();

            LearningAndRaceManager.LoadTrack(formLoadTracks.FilePathOfChosenTrack);

            _ = TrackAndBackgroundCache.GetTrackFromCache();
            Application.DoEvents(); // ensure screen paints

            LearningAndRaceManager.StartLearning();
        }


        // C O N F I G U R E

        // C O N F I G U R E  >  W O R L D

        /// <summary>
        /// User clicked menu item: Configure > World.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfigureWorldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using FormConfigureWorld worldSettings = new();

            worldSettings.ShowDialog();

            if (Config.s_settings.ConfigChangedTheCarsAndNeuralNetworkAreInvalid == true && LearningAndRaceManager.InLearning)
            {
                // whatever we need to do.
                RestartLearning();
            }
        }

        // C O N F I G U R E  >  V I S I O N

        /// <summary>
        /// User clicked menu item: Configure > Vision.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfigureVisionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool inlearning = LearningAndRaceManager.InLearning;
            LearningAndRaceManager.StopLearning();

            using FormConfigureVision formAIVisionSettings = new();

            formAIVisionSettings.ShowDialog();

            if (inlearning) RestartLearning();
        }


        // A I   M O D E L

        // A I   M O D E L  >  C O N F I G U R E

        /// <summary>
        /// User clicked menu item: AI Model > Configure.
        /// We show a dialog that enables them to tweak the AI. Certain settings are incompatible with continuing
        /// training, so we have to restart it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AIModelConfigureToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            bool inlearning = LearningAndRaceManager.InLearning;

            LearningAndRaceManager.StopLearning();

            using FormAIModelConfigure formAIBrainSettings = new();

            // we only restart if certain things changed (that will cause unstable state, like changing the neural network size).
            if (formAIBrainSettings.ShowDialog() == DialogResult.OK && Config.s_settings.ConfigChangedTheCarsAndNeuralNetworkAreInvalid)
            {
                if (inlearning) RestartLearning();
            }
            else
            {
                if (inlearning) LearningAndRaceManager.ResumeLearning();
            }
        }

        // A I   M O D E L  >  S A V E

        /// <summary>
        /// User clicked menu item: AI Model > Save.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AIModelSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NeuralNetwork.s_networks.Count == 0)
            {
                MessageBox.Show("There is no active model loaded to save.", "Error");
                return;
            }

            LearningAndRaceManager.SaveNeuralNetworkStateForCars(s_filePathCarsNTanksAIModel);
        }

        // A I   M O D E L  >  L O A D

        /// <summary>
        /// User clicked menu item: AI Model > Load.
        /// This will load the model if present. It will ignore, if the files are not found (without warning).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AIModelLoadToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // before we blow away the neural network, we need to stop things.
            LearningAndRaceManager.StopLearning();

            // Attempt to load the pre-saved network from disk
            if (!LearningAndRaceManager.LoadNeuralNetworkStateForCars(s_filePathCarsNTanksAIModel))
            {
                MessageBox.Show($"No AI model files were found.\n\"{s_filePathCarsNTanksAIModel}\".", "Error");
                return;
            }

            OutputFormulaForBestCar();

            // now we're ready to have the cars run around the track but because we have to stop learning, and the
            // cars would be left in random locations, we initialise the cars (reset to start) before we resume the
            // learning process
            LearningAndRaceManager.InitialiseCars();
            LearningAndRaceManager.ResumeLearning();
        }

        /// <summary>
        /// Outputs the formula for the best car.
        /// </summary>
        internal static void OutputFormulaForBestCar()
        {
            // this isn't strictly necessary, but done to be consistent.
            // we output the formula for the best car.
            float best = -1;
            int bestid = 0;

            foreach (int id in NeuralNetwork.s_networks.Keys)
            {
                if (NeuralNetwork.s_networks[id].Fitness > best)
                {
                    best = NeuralNetwork.s_networks[id].Fitness;
                    bestid = id;
                }
            }

            // forces it to save the "formula" of the neural network as a file to c:\temp
            if (best != -1)
            {
                _ = NeuralNetwork.s_networks[bestid].Formula();
                MessageBox.Show(@"Neural network of best car saved to: c:\temp\formula.txt.", "Info");
            }
            else
            {
                MessageBox.Show("No best car found, unable to save formula.", "Info");
            }
        }
        #endregion

        #region GLASS PANELS
        /// <summary>   
        /// Registers the moveable panels, that will be painted during "onpaint".
        /// </summary>
        private void CreateGlassPanelsAtSavedPosition()
        {
            s_moveablePanels.Clear();

            // don't draw for hard-coded.
            if (!VehicleDrivenByAI.c_hardCodedBrain)
            {
                s_moveablePanels.Add("ranking", new MoveableRankingsPanel(panelTrack, new Rectangle(Config.s_settings.Display.LocationOfMoveableRankingsPane, new Size(260, 290))));
                s_moveablePanels.Add("neural", new MoveableNeuralNetwork(panelTrack, new Rectangle(Config.s_settings.Display.LocationOfMoveableNeuralNetworkPane, new Size(300, 210))));
                s_moveablePanels.Add("speedo", new MoveableSpeedDialPanel(panelTrack, new Rectangle(Config.s_settings.Display.LocationOfMoveableSpeedPane, new Size(260, 260))));
                s_moveablePanels.Add("vision", new MoveableVisionPanel(panelTrack, new Rectangle(Config.s_settings.Display.LocationOfMoveableVisionPane, new Size(210, 210))));
                s_moveablePanels.Add("telemetry", new MoveableTelemetryPanel(panelTrack, new Rectangle(Config.s_settings.Display.LocationOfMoveableTelemetryPane, new Size(150, 320))));
                s_moveablePanels.Add("statistics", new MoveableStatisticPanel(panelTrack, new Rectangle(Config.s_settings.Display.LocationOfMoveableStatisicsPane, new Size(10, 10))));
                s_moveablePanels["statistics"].Visible = true;
            }
        }

        /// <summary>
        /// User clicked on the track panel, check the transparent panes to see if they were clicked and if so, start dragging.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_MouseDownGlassPanelCheck(object? sender, MouseEventArgs e)
        {
            foreach (var x in s_moveablePanels.Values) if (x.MouseDown(e)) return;
        }

        /// <summary>
        /// Menu > Show/hide neural network glass panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelsToggleNeuralNetworkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            s_moveablePanels["neural"].Visible = !s_moveablePanels["neural"].Visible;

            PanelsNeuralNetworkToolStripMenuItem.Checked = s_moveablePanels["neural"].Visible;
        }

        /// <summary>
        /// Menu > Show/hide ranking glass panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelsToggleRankingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            s_moveablePanels["ranking"].Visible = !s_moveablePanels["ranking"].Visible;

            PanelsRankingsToolStripMenuItem.Checked = s_moveablePanels["ranking"].Visible;
        }

        /// <summary>
        /// Menu > Show/hide speedo glass panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelsToggleSpeedometerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            s_moveablePanels["speedo"].Visible = !s_moveablePanels["speedo"].Visible;

            PanelsSpeedometerToolStripMenuItem.Checked = s_moveablePanels["speedo"].Visible;
        }

        /// <summary>
        /// Menu > Show/hide vision glass panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelsToggleVisionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            s_moveablePanels["vision"].Visible = !s_moveablePanels["vision"].Visible;

            PanelsVisionToolStripMenuItem.Checked = s_moveablePanels["vision"].Visible;
        }

        /// <summary>
        /// Menu > Show/hide telemetry glass panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelsToggleTelemetryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            s_moveablePanels["telemetry"].Visible = !s_moveablePanels["telemetry"].Visible;

            PanelsTelemetryToolStripMenuItem.Checked = s_moveablePanels["telemetry"].Visible;
        }

        /// <summary>
        /// Menu > Show/hide statistics/info glass panel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelsInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            s_moveablePanels["statistics"].Visible = !s_moveablePanels["statistics"].Visible;

            PanelsInfoToolStripMenuItem.Checked = s_moveablePanels["statistics"].Visible;

        }
        #endregion
    }
}