namespace CarsAndTanks.UX.Forms.Settings
{
    partial class FormLoadTracks
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Tracks = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // Tracks
            // 
            this.Tracks.AutoScroll = true;
            this.Tracks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Tracks.Location = new System.Drawing.Point(10, 10);
            this.Tracks.Name = "Tracks";
            this.Tracks.Padding = new System.Windows.Forms.Padding(1);
            this.Tracks.Size = new System.Drawing.Size(828, 456);
            this.Tracks.TabIndex = 6;
            // 
            // FormLoadTracks
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(848, 476);
            this.Controls.Add(this.Tracks);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MdiChildrenMinimizedAnchorBottom = false;
            this.MinimizeBox = false;
            this.Name = "FormLoadTracks";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Choose Track to Load";
            this.Load += new System.EventHandler(this.FormLoadTracks_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private FlowLayoutPanel Tracks;
    }
}