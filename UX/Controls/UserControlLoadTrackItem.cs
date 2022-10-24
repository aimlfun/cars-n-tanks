namespace CarsAndTanks.UX.Controls;

public partial class UserControlLoadTrackItem : UserControl
{
    public delegate void CallbackSelect(string filename);

    readonly private CallbackSelect? callbackWhenTrackSelected;

    readonly private Control c;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="trackFilename"></param>
    /// <exception cref="ArgumentException"></exception>
    public UserControlLoadTrackItem(string trackFilename, CallbackSelect callback)
    {
        InitializeComponent();

        callbackWhenTrackSelected = callback;

        if (!File.Exists(trackFilename)) throw new ArgumentException(nameof(trackFilename) + " track doesn't exist");

        labelTrackName.Text = Path.GetFileNameWithoutExtension(trackFilename);
        string imageFileName = Path.ChangeExtension(trackFilename, ".png");

        if (File.Exists(imageFileName)) pictureBoxTrack.Image = new Bitmap(imageFileName);

        c = this;
        c.Tag = trackFilename;
        pictureBoxTrack.Click += Track_Click;
        labelTrackName.Click += Track_Click;
        roundedPanel1.Click += Track_Click;
    }

    private void Track_Click(object? sender, EventArgs e)
    {
        callbackWhenTrackSelected?.Invoke((string)c.Tag);
    }


    private void RoundedPanel1_MouseEnter(object sender, EventArgs e)
    {
        roundedPanel1.PanelBorderColor = Color.Black;
    }

    private void RoundedPanel1_MouseLeave(object sender, EventArgs e)
    {
        roundedPanel1.PanelBorderColor = Color.FromArgb(204, 204, 204);
    }
}
