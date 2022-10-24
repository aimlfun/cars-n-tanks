using CarsAndTanks.UX.Controls;

namespace CarsAndTanks.UX.Forms.Settings
{
    /// <summary>
    /// This is a very simple dialog showing each track as a clickable image.
    /// </summary>
    public partial class FormLoadTracks : Form
    {
        /// <summary>
        /// Used in the OnLoad to output all the tracks.
        /// </summary>
        readonly private string? locationWhereTrackFilesAreStored;

        /// <summary>
        /// null -> no track chosen by user.
        /// not null -> contains the file-path of the track file chosen by user.
        /// </summary>
        private string? filePathOfChosenTrack = null;

        /// <summary>
        /// Caller can use this to determine which track was chosen.
        /// </summary>
        internal string FilePathOfChosenTrack
        {
            get
            {
                if (filePathOfChosenTrack is null) throw new Exception("This property should not be retrieved unless a track was chosen.");

                return filePathOfChosenTrack;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="trackPath">Where the tracks are stored.</param>
        public FormLoadTracks(string? trackPath = null)
        {
            InitializeComponent();
            BackColor = FluentStyle.BackgroundColor;
            Dock = DockStyle.Fill;

            // onload uses this to find track files
            locationWhereTrackFilesAreStored = trackPath;
        }

        /// <summary>
        /// Populates the UI with a clickable item for each track the user has saved. 
        /// We store an "image" of the track to make it easier for them to find.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormLoadTracks_Load(object sender, EventArgs e)
        {
            // no path, no tracks displayed
            if (locationWhereTrackFilesAreStored is null) return;

            // tracks are stored as text with extension of ".track"
            IEnumerable<string> trackFiles = Directory.EnumerateFiles(locationWhereTrackFilesAreStored, "*.track");

            // add each track to the flow-layout-panel
            foreach (string file in trackFiles)
            {
                UserControlLoadTrackItem item = new(trackFilename: file, callback: OnTrackSelectedEventHandler);
                Tracks.Controls.Add(item);
            }

            // no point in showing an empty dialog. If no tracks exist give an error and close the dialog.
            if (Tracks.Controls.Count == 0)
            {
                MessageBox.Show("No pre-saved tracks were found.");

                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        /// <summary>
        /// This is called when the user clicks on a track.
        /// </summary>
        /// <param name="filename"></param>
        private void OnTrackSelectedEventHandler(string filename)
        {
            // enable the caller to know what was selected.
            filePathOfChosenTrack = filename;

            // close the dialog, as we're done selecting. We return "OK" to indicate "selected"
            DialogResult = DialogResult.OK;

            Close();
        }
    }
}