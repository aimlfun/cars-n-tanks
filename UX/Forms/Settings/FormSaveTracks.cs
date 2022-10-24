using CarsAndTanks.Learn;

namespace CarsAndTanks.UX.Forms.Settings
{
    public partial class FormSaveTracks : Form
    {
        public FormSaveTracks()
        {
            InitializeComponent();
        }


        private bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(textBoxTrackName.Text))
            {
                textBoxTrackName.Focus();
                MessageBox.Show("Please enter a track name.");
                return false;
            }

            if (textBoxTrackName.Text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                textBoxTrackName.Focus();
                MessageBox.Show("Please enter a valid track name.");
                return false;
            }

            if (textBoxTrackName.Text.Length > 40)
            {
                textBoxTrackName.Focus();
                MessageBox.Show("Please enter a shorter track name (less than 40 characters).");
                return false;
            }

            if (textBoxTrackName.Text.Contains("."))
            {
                textBoxTrackName.Focus();
                MessageBox.Show("Please enter a valid track name; they may not contain a dot.");
                return false;
            }
            return true;
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            if (!IsValid()) return;

            LearningAndRaceManager.SaveTrack(Path.Combine(Program.applicationUserTracks, textBoxTrackName.Text + ".track"));
        }
    }
}
