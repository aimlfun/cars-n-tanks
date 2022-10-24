namespace AICarTrack.UX.Controls
{
    partial class UserControlLoadTrackItem
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.roundedPanel1 = new AICarTrack.RoundedPanel();
            this.labelTrackName = new System.Windows.Forms.Label();
            this.pictureBoxTrack = new System.Windows.Forms.PictureBox();
            this.roundedPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTrack)).BeginInit();
            this.SuspendLayout();
            // 
            // roundedPanel1
            // 
            this.roundedPanel1.Controls.Add(this.labelTrackName);
            this.roundedPanel1.Controls.Add(this.pictureBoxTrack);
            this.roundedPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.roundedPanel1.Location = new System.Drawing.Point(0, 3);
            this.roundedPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.roundedPanel1.Name = "roundedPanel1";
            this.roundedPanel1.PanelBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.roundedPanel1.PanelBottomColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.roundedPanel1.PanelColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(243)))), ((int)(((byte)(243)))));
            this.roundedPanel1.PanelSplit = 20;
            this.roundedPanel1.Radius = 10;
            this.roundedPanel1.Size = new System.Drawing.Size(264, 151);
            this.roundedPanel1.TabIndex = 0;
            this.roundedPanel1.MouseEnter += new System.EventHandler(this.RoundedPanel1_MouseEnter);
            this.roundedPanel1.MouseLeave += new System.EventHandler(this.RoundedPanel1_MouseLeave);
            // 
            // labelTrackName
            // 
            this.labelTrackName.AutoEllipsis = true;
            this.labelTrackName.BackColor = System.Drawing.Color.Transparent;
            this.labelTrackName.Font = new System.Drawing.Font("Segoe UI Variable Display Semib", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.labelTrackName.Location = new System.Drawing.Point(0, 127);
            this.labelTrackName.Margin = new System.Windows.Forms.Padding(0);
            this.labelTrackName.Name = "labelTrackName";
            this.labelTrackName.Padding = new System.Windows.Forms.Padding(2);
            this.labelTrackName.Size = new System.Drawing.Size(251, 24);
            this.labelTrackName.TabIndex = 5;
            this.labelTrackName.Text = "(track name)";
            this.labelTrackName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelTrackName.MouseEnter += new System.EventHandler(this.RoundedPanel1_MouseEnter);
            this.labelTrackName.MouseLeave += new System.EventHandler(this.RoundedPanel1_MouseLeave);
            // 
            // pictureBoxTrack
            // 
            this.pictureBoxTrack.BackColor = System.Drawing.Color.Transparent;
            this.pictureBoxTrack.Location = new System.Drawing.Point(6, 4);
            this.pictureBoxTrack.Margin = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.pictureBoxTrack.Name = "pictureBoxTrack";
            this.pictureBoxTrack.Size = new System.Drawing.Size(254, 134);
            this.pictureBoxTrack.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBoxTrack.TabIndex = 4;
            this.pictureBoxTrack.TabStop = false;
            this.pictureBoxTrack.MouseEnter += new System.EventHandler(this.RoundedPanel1_MouseEnter);
            this.pictureBoxTrack.MouseLeave += new System.EventHandler(this.RoundedPanel1_MouseLeave);
            // 
            // UserControlLoadTrackItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.roundedPanel1);
            this.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "UserControlLoadTrackItem";
            this.Padding = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.Size = new System.Drawing.Size(267, 154);
            this.roundedPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxTrack)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private RoundedPanel roundedPanel1;
        private Label labelTrackName;
        private PictureBox pictureBoxTrack;
    }
}
