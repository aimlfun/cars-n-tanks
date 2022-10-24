using AICarTrack.Settings;
using AICarTrack.World.UX.Track;
using CarsAndTanks.Settings;
using CarsAndTanks.World;

namespace CarsAndTanks.UX.Forms.Settings
{
    public partial class FormConfigureVision : Form
    {
        /// <summary>
        /// Access to the AI configuration.
        /// </summary>
        readonly ConfigAI aiConf = Config.s_settings.AI;

        /// <summary>
        /// 
        /// </summary>
        public FormConfigureVision()
        {
            InitializeComponent();

            DisplayVisionVisualisation();
            DialogResult = DialogResult.OK;
        }

        /// <summary>
        /// 
        /// </summary>
        private void DisplayVisionVisualisation()
        {
            if (TrackImagesCache.s_topToBottomTrackBitmap is null) return;

            Bitmap bmp = new(pictureBoxWorldRepresentation.Width, pictureBoxWorldRepresentation.Height);

            labelDistance.Text = Config.s_settings.AI.DepthOfVisionInPixels.ToString();

            Graphics g = Graphics.FromImage(bmp);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            for (int y = 0; y < bmp.Height; y += TrackImagesCache.s_topToBottomTrackBitmap.Height - 1)
            {
                g.DrawImage(TrackImagesCache.s_topToBottomTrackBitmap, new Point(bmp.Width / 2 - TrackImagesCache.s_topToBottomTrackBitmap.Width / 2, y));
            }

            VehicleDrivenByAI c = new(0);
            WorldManager.LoadAndSizeCar();

            c.CarImplementation.AngleVehicleIsPointingInDegrees = 270;
            c.CarImplementation.LocationOnTrack = new Point(bmp.Width / 2, bmp.Height / 2);
            c.Renderer.Render(g, c);

            Config.s_settings.AI.VisionSystem.SetLIDAR(0, new double[Config.s_settings.AI.SamplePoints]);

            Config.s_settings.AI.VisionSystem.DrawSensorToImage(0, g, bmp, new Point(bmp.Width / 2, bmp.Height / 2));

            if (pictureBoxWorldRepresentation.Image != null) pictureBoxWorldRepresentation.Image.Dispose();
            pictureBoxWorldRepresentation.Image = bmp;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormAIVisionSettings_Load(object sender, EventArgs e)
        {
            sliderDepth.Value = aiConf.DepthOfVisionInPixels;
            sliderDepth.ValueChanged += InputsWereChanged;

            numericUpDownFieldOfVisionStartInDegrees.Value = aiConf.FieldOfVisionStartInDegrees;
            numericUpDownFieldOfVisionStartInDegrees.ValueChanged += InputsWereChanged;
            numericUpDownFieldOfVisionStopInDegrees.Value = aiConf.FieldOfVisionStopInDegrees;
            numericUpDownFieldOfVisionStopInDegrees.ValueChanged += InputsWereChanged;
            numericUpDownSamplePoints.Value = aiConf.SamplePoints;
            numericUpDownSamplePoints.ValueChanged += InputsWereChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InputsWereChanged(object? sender, EventArgs e)
        {
            aiConf.DepthOfVisionInPixels = (int)sliderDepth.Value;

            aiConf.FieldOfVisionStartInDegrees = (int)numericUpDownFieldOfVisionStartInDegrees.Value;
            aiConf.FieldOfVisionStopInDegrees = (int)numericUpDownFieldOfVisionStopInDegrees.Value;

            if (aiConf.FieldOfVisionStartInDegrees > aiConf.FieldOfVisionStopInDegrees)
            {
                aiConf.FieldOfVisionStartInDegrees = aiConf.FieldOfVisionStopInDegrees - 1;
                numericUpDownFieldOfVisionStartInDegrees.Value = aiConf.FieldOfVisionStartInDegrees;
            }

            aiConf.SamplePoints = (int)numericUpDownSamplePoints.Value;

            if (aiConf.Layers[0] != aiConf.SamplePoints) aiConf.Layers[0] = aiConf.SamplePoints;

            Config.s_settings.ConfigChangedTheCarsAndNeuralNetworkAreInvalid = true;

            DisplayVisionVisualisation();
        }

        /// <summary>
        /// Saves on closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormAIVisionSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            Config.Save();
        }

        /// <summary>
        /// When the user presses [Escape] close the dialog.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormConfigureVision_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) Close();
        }
    }
}
