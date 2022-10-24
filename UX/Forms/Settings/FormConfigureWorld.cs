using AICarTrack.Settings;
using AICarTrack.World.UX.Track;
using CarsAndTanks.Settings;
using CarsAndTanks.World;

namespace AICarTrack;

/// <summary>
/// Displays the world settings and allows user to change them.
/// </summary>
public partial class FormConfigureWorld : Form
{
    /// <summary>
    /// Easy access to the world configuration.
    /// </summary>
    readonly ConfigWorld worldConf = Config.s_settings.World;

    /// <summary>
    /// Easy access to the display configuration.
    /// </summary>
    readonly ConfigDisplay displayConf = Config.s_settings.Display;

    /// <summary>
    /// Stops repaint during initialisation.
    /// </summary>
    bool initialising = true;

    /// <summary>
    /// Constructor.
    /// </summary>
    public FormConfigureWorld()
    {
        InitializeComponent();
        Config.s_settings.ConfigChangedTheCarsAndNeuralNetworkAreInvalid = false;
    }

    /// <summary>
    /// OnLoad() copies the config to the UI INPUTs.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void FormWorldSettings_Load(object sender, EventArgs e)
    {
        initialising = true;

        // rb (radio buttons) depend on belonging to a panel, so one of them is selected only.

        rbPhysicsReal.Checked = (worldConf.UsingRealWorldPhysics);
        rbPhysicsReal.CheckedChanged += RbPhysics_CheckedChanged;

        rbPhysicsBasic.Checked = (!worldConf.UsingRealWorldPhysics);
        rbPhysicsBasic.CheckedChanged += RbPhysics_CheckedChanged;

        rbShapeBlob.Checked = (displayConf.VehicleShape == ConfigDisplay.VehicleSupportedShapes.Blob);
        rbShapeBlob.CheckedChanged += RbShape_CheckedChanged;

        rbShapeKart.Checked = (displayConf.VehicleShape == ConfigDisplay.VehicleSupportedShapes.GoKart);
        rbShapeKart.CheckedChanged += RbShape_CheckedChanged;

        rbShapeTank.Checked = (displayConf.VehicleShape == ConfigDisplay.VehicleSupportedShapes.Tank);
        rbShapeTank.CheckedChanged += RbShape_CheckedChanged;

        sliderCarSize.Value = (worldConf.DiameterOfCarInPixels);
        sliderCarSize.ValueChanged += SliderCarSize_ValueChanged;

        sliderRoadWidth.Value = (worldConf.RoadWidthInPixels);
        sliderRoadWidth.ValueChanged += SliderRoadWidth_ValueChanged;

        sliderGateThreshold.Value = (worldConf.GateThresholdInPixels);

        rbTelemetryOff.Checked = !displayConf.ShowTelemetry;
        rbTelemetryOff.CheckedChanged += RbTelemetry_CheckedChanged;
        rbTelemetryOn.Checked = displayConf.ShowTelemetry;
        rbTelemetryOn.CheckedChanged += RbTelemetry_CheckedChanged;

        rbCarLabelOff.Checked = (displayConf.DisplayCarNumber == CarNumberDisplayModes.None);
        rbCarLabelOff.CheckedChanged += RbCarLabel_CheckedChanged;
        rbCarLabelMiddle.Checked = (displayConf.DisplayCarNumber == CarNumberDisplayModes.WithinVehicle);
        rbCarLabelMiddle.CheckedChanged += RbCarLabel_CheckedChanged;
        rbCarLabelOutside.Checked = (displayConf.DisplayCarNumber == CarNumberDisplayModes.OutsideOfVehicle);
        rbCarLabelOutside.CheckedChanged += RbCarLabel_CheckedChanged;

        rbDisplayGatesOff.Checked = (!displayConf.DisplayGates);
        rbDisplayGatesOff.CheckedChanged += RbDisplayGates_CheckedChanged;
        rbDisplayGatesOn.Checked = (displayConf.DisplayGates);
        rbDisplayGatesOn.CheckedChanged += RbDisplayGates_CheckedChanged;

        rbFlashAtGatesOff.Checked = (!displayConf.FlashCarAsItGoesOverAGate);
        rbFlashAtGatesOff.CheckedChanged += RbFlashAtGates_CheckedChanged;
        rbFlashAtGatesOn.Checked = (displayConf.FlashCarAsItGoesOverAGate);
        rbFlashAtGatesOn.CheckedChanged += RbFlashAtGates_CheckedChanged;

        rbShowHitPointOn.Checked = (displayConf.ShowHitPointsOnCar);
        rbShowHitPointOn.CheckedChanged += RbShowHitPoint_CheckedChanged;
        rbShowHitPointOff.Checked = (!displayConf.ShowHitPointsOnCar);
        rbShowHitPointOff.CheckedChanged += RbShowHitPoint_CheckedChanged;

        rbRandomColorsOn.Checked = (displayConf.CarsAreRandomColours);
        rbRandomColorsOn.CheckedChanged += RbRandomColors_CheckedChanged;
        rbRandomColorsOff.Checked = (!displayConf.CarsAreRandomColours);
        rbRandomColorsOff.CheckedChanged += RbRandomColors_CheckedChanged;

        rbDisplayEliminatedCarsOn.Checked = displayConf.BrieflyDisplayGhostOfEliminatedCars;
        rbDisplayEliminatedCarsOn.CheckedChanged += RbDisplayEliminatedCarsOn_CheckedChanged;
        rbDisplayEliminatedCarsOff.Checked = !displayConf.BrieflyDisplayGhostOfEliminatedCars;
        rbDisplayEliminatedCarsOff.CheckedChanged += RbDisplayEliminatedCarsOn_CheckedChanged;

        numericUpDownEliminatedMoves.Value = displayConf.HowLongToShowGhostOfEliminatedCars;
        numericUpDownEliminatedMoves.ValueChanged += NumericUpDownEliminatedMoves_ValueChanged;

        initialising = false;

        DisplayTrackWithCar();
    }

    /// <summary>
    /// Show telemetry setting was changed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RbTelemetry_CheckedChanged(object? sender, EventArgs e)
    {
        displayConf.ShowTelemetry = rbTelemetryOn.Checked;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NumericUpDownEliminatedMoves_ValueChanged(object? sender, EventArgs e)
    {
        displayConf.HowLongToShowGhostOfEliminatedCars = (int)numericUpDownEliminatedMoves.Value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RbDisplayEliminatedCarsOn_CheckedChanged(object? sender, EventArgs e)
    {
        displayConf.BrieflyDisplayGhostOfEliminatedCars = rbDisplayEliminatedCarsOn.Checked;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RbRandomColors_CheckedChanged(object? sender, EventArgs e)
    {
        displayConf.CarsAreRandomColours = rbRandomColorsOn.Checked;
    }

    /// <summary>
    /// Show hit points was changed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RbShowHitPoint_CheckedChanged(object? sender, EventArgs e)
    {
        displayConf.ShowHitPointsOnCar = rbShowHitPointOn.Checked;

        DisplayTrackWithCar();
    }

    /// <summary>
    /// Display gates setting was changed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RbDisplayGates_CheckedChanged(object? sender, EventArgs e)
    {
        displayConf.DisplayGates = rbDisplayGatesOn.Checked;
        DisplayTrackWithCar();
    }

    /// <summary>
    /// Flash at gates setting was changed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RbFlashAtGates_CheckedChanged(object? sender, EventArgs e)
    {
        displayConf.FlashCarAsItGoesOverAGate = rbFlashAtGatesOn.Checked;
        DisplayTrackWithCar();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RbCarLabel_CheckedChanged(object? sender, EventArgs e)
    {
        if (rbCarLabelOff.Checked) displayConf.DisplayCarNumber = CarNumberDisplayModes.None;
        if (rbCarLabelMiddle.Checked) displayConf.DisplayCarNumber = CarNumberDisplayModes.WithinVehicle;
        if (rbCarLabelOutside.Checked) displayConf.DisplayCarNumber = CarNumberDisplayModes.OutsideOfVehicle;

        DisplayTrackWithCar();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RbShape_CheckedChanged(object? sender, EventArgs e)
    {
        displayConf.VehicleShape = ConfigDisplay.VehicleSupportedShapes.Blob;

        if (rbShapeKart.Checked) displayConf.VehicleShape = ConfigDisplay.VehicleSupportedShapes.GoKart;
        if (rbShapeTank.Checked) displayConf.VehicleShape = ConfigDisplay.VehicleSupportedShapes.Tank;

        DisplayTrackWithCar();
        Config.s_settings.ConfigChangedTheCarsAndNeuralNetworkAreInvalid = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SliderRoadWidth_ValueChanged(object? sender, EventArgs e)
    {
        worldConf.RoadWidthInPixels = (int)sliderRoadWidth.Value;
        DisplayTrackWithCar();
        Config.s_settings.ConfigChangedTheCarsAndNeuralNetworkAreInvalid = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void SliderCarSize_ValueChanged(object? sender, EventArgs e)
    {
        worldConf.DiameterOfCarInPixels = (int)sliderCarSize.Value;

        float minSize = (float)sliderCarSize.Value * 1.8F;
        if ((float)sliderRoadWidth.Value < minSize) sliderRoadWidth.Value = (decimal)minSize;

        sliderRoadWidth.Minimum = (decimal)minSize;

        DisplayTrackWithCar();
        Config.s_settings.ConfigChangedTheCarsAndNeuralNetworkAreInvalid = true;
    }

    /// <summary>
    /// Shows a car on a track, that both can be configured from the UI.
    /// </summary>
    private void DisplayTrackWithCar()
    {
        if (initialising) return;

        Bitmap bmp = new(TrackImagesCache.c_sizeOfTile, TrackImagesCache.c_sizeOfTile);

        Graphics g = Graphics.FromImage(bmp);

        // avoid pixellated display
        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

        // draw the track
        TrackImagesCache.Initialise();

        for (int y = 0; y < bmp.Height; y += TrackImagesCache.s_topToBottomTrackBitmap.Height - 1)
        {
            g.DrawImage(TrackImagesCache.s_topToBottomTrackBitmap, new Point(bmp.Width / 2 - TrackImagesCache.s_topToBottomTrackBitmap.Width / 2, y));
        }

        WorldManager.LoadAndSizeCar();

        // draw the car

        VehicleDrivenByAI c = new(0);
        c.CarImplementation.AngleVehicleIsPointingInDegrees = 270;
        c.CarImplementation.LocationOnTrack = new Point(bmp.Width / 2, bmp.Height / 2);
        c.Renderer.Render(g, c);

        if (pictureBoxWorldRepresentation.Image != null) pictureBoxWorldRepresentation.Image.Dispose(); // remove prior image
        pictureBoxWorldRepresentation.Image = bmp; // show the track-with car image
    }

    /// <summary>
    /// User changed the the real-world physics.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RbPhysics_CheckedChanged(object? sender, EventArgs e)
    {
        worldConf.UsingRealWorldPhysics = rbPhysicsReal.Checked;

        DisplayTrackWithCar();
        Config.s_settings.ConfigChangedTheCarsAndNeuralNetworkAreInvalid = true;
    }

    private void FormConfigureWorld_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }

    /// <summary>
    /// Save as it closes.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FormConfigureWorld_FormClosing(object sender, FormClosingEventArgs e)
    {
        Config.Save();
    }
}