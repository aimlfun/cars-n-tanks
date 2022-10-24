using System;
using System.Windows.Forms;

namespace AICarTrack
{
    partial class FormMain
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
            this.components = new System.ComponentModel.Container();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trackNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trackNewRandomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trackNewDrawToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trackNewConstructorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trackSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trackLoadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configureWorldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configureVisionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AIModelConfigureToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.AIModelSaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AIModelLoadToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PanelsNeuralNetworkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PanelsRankingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PanelsSpeedometerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PanelsVisionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PanelsTelemetryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PanelsInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelTrack = new System.Windows.Forms.Panel();
            this.mainMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.trackToolStripMenuItem,
            this.configureToolStripMenuItem,
            this.aIToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(1372, 24);
            this.mainMenuStrip.TabIndex = 8;
            this.mainMenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileExitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F)));
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // fileExitToolStripMenuItem
            // 
            this.fileExitToolStripMenuItem.Name = "fileExitToolStripMenuItem";
            this.fileExitToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
            this.fileExitToolStripMenuItem.Text = "Exit";
            this.fileExitToolStripMenuItem.ToolTipText = "Exits the application.";
            this.fileExitToolStripMenuItem.Click += new System.EventHandler(this.FileExitToolStripMenuItem_Click);
            // 
            // trackToolStripMenuItem
            // 
            this.trackToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.trackNewToolStripMenuItem,
            this.trackSaveToolStripMenuItem,
            this.trackLoadToolStripMenuItem});
            this.trackToolStripMenuItem.Name = "trackToolStripMenuItem";
            this.trackToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.T)));
            this.trackToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.trackToolStripMenuItem.Text = "&Track";
            // 
            // trackNewToolStripMenuItem
            // 
            this.trackNewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.trackNewRandomToolStripMenuItem,
            this.trackNewDrawToolStripMenuItem,
            this.trackNewConstructorToolStripMenuItem});
            this.trackNewToolStripMenuItem.Name = "trackNewToolStripMenuItem";
            this.trackNewToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.trackNewToolStripMenuItem.Text = "New";
            // 
            // trackNewRandomToolStripMenuItem
            // 
            this.trackNewRandomToolStripMenuItem.Name = "trackNewRandomToolStripMenuItem";
            this.trackNewRandomToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.trackNewRandomToolStripMenuItem.Text = "Random";
            this.trackNewRandomToolStripMenuItem.ToolTipText = "Discards any existing track and replaces it with a distorted polygon track.";
            this.trackNewRandomToolStripMenuItem.Click += new System.EventHandler(this.TrackNewRandomToolStripMenuItem_Click);
            // 
            // trackNewDrawToolStripMenuItem
            // 
            this.trackNewDrawToolStripMenuItem.Name = "trackNewDrawToolStripMenuItem";
            this.trackNewDrawToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.trackNewDrawToolStripMenuItem.Text = "Draw";
            this.trackNewDrawToolStripMenuItem.ToolTipText = "Discards any existing track and allows you to draw the track using discreet lines" +
    " (not curves).";
            this.trackNewDrawToolStripMenuItem.Click += new System.EventHandler(this.TrackNewDrawToolStripMenuItem_Click);
            // 
            // trackNewConstructorToolStripMenuItem
            // 
            this.trackNewConstructorToolStripMenuItem.Name = "trackNewConstructorToolStripMenuItem";
            this.trackNewConstructorToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.trackNewConstructorToolStripMenuItem.Text = "Constructor";
            this.trackNewConstructorToolStripMenuItem.ToolTipText = "Discards any existing track and allows you to construct a track using prebuilt cu" +
    "rves and lines.";
            this.trackNewConstructorToolStripMenuItem.Click += new System.EventHandler(this.TrackNewConstructorToolStripMenuItem_Click);
            // 
            // trackSaveToolStripMenuItem
            // 
            this.trackSaveToolStripMenuItem.Name = "trackSaveToolStripMenuItem";
            this.trackSaveToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.trackSaveToolStripMenuItem.Text = "Save...";
            this.trackSaveToolStripMenuItem.ToolTipText = "Saves the current track to %appdata% for loading at a future point.";
            this.trackSaveToolStripMenuItem.Click += new System.EventHandler(this.TrackSaveToolStripMenuItem_Click);
            // 
            // trackLoadToolStripMenuItem
            // 
            this.trackLoadToolStripMenuItem.Name = "trackLoadToolStripMenuItem";
            this.trackLoadToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.trackLoadToolStripMenuItem.Text = "Load...";
            this.trackLoadToolStripMenuItem.ToolTipText = "Provides a list of previously saved tracks for you to load.";
            this.trackLoadToolStripMenuItem.Click += new System.EventHandler(this.TrackLoadToolStripMenuItem_Click);
            // 
            // configureToolStripMenuItem
            // 
            this.configureToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configureWorldToolStripMenuItem,
            this.configureVisionToolStripMenuItem});
            this.configureToolStripMenuItem.Name = "configureToolStripMenuItem";
            this.configureToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.C)));
            this.configureToolStripMenuItem.Size = new System.Drawing.Size(72, 20);
            this.configureToolStripMenuItem.Text = "&Configure";
            // 
            // configureWorldToolStripMenuItem
            // 
            this.configureWorldToolStripMenuItem.Name = "configureWorldToolStripMenuItem";
            this.configureWorldToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.configureWorldToolStripMenuItem.Text = "World...";
            this.configureWorldToolStripMenuItem.ToolTipText = "Configure how the virtual world looks.";
            this.configureWorldToolStripMenuItem.Click += new System.EventHandler(this.ConfigureWorldToolStripMenuItem_Click);
            // 
            // configureVisionToolStripMenuItem
            // 
            this.configureVisionToolStripMenuItem.Name = "configureVisionToolStripMenuItem";
            this.configureVisionToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.configureVisionToolStripMenuItem.Text = "Vision...";
            this.configureVisionToolStripMenuItem.ToolTipText = "Configure granularity of the LIDAR and how far it sees.";
            this.configureVisionToolStripMenuItem.Click += new System.EventHandler(this.ConfigureVisionToolStripMenuItem_Click);
            // 
            // aIToolStripMenuItem
            // 
            this.aIToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AIModelConfigureToolStripMenuItem1,
            this.AIModelSaveToolStripMenuItem,
            this.AIModelLoadToolStripMenuItem1});
            this.aIToolStripMenuItem.Name = "aIToolStripMenuItem";
            this.aIToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.A)));
            this.aIToolStripMenuItem.Size = new System.Drawing.Size(67, 20);
            this.aIToolStripMenuItem.Text = "&AI Model";
            // 
            // AIModelConfigureToolStripMenuItem1
            // 
            this.AIModelConfigureToolStripMenuItem1.Name = "AIModelConfigureToolStripMenuItem1";
            this.AIModelConfigureToolStripMenuItem1.Size = new System.Drawing.Size(136, 22);
            this.AIModelConfigureToolStripMenuItem1.Text = "Configure...";
            this.AIModelConfigureToolStripMenuItem1.ToolTipText = "Configure the neurons, mutation, output amplification etc.";
            this.AIModelConfigureToolStripMenuItem1.Click += new System.EventHandler(this.AIModelConfigureToolStripMenuItem1_Click);
            // 
            // AIModelSaveToolStripMenuItem
            // 
            this.AIModelSaveToolStripMenuItem.Name = "AIModelSaveToolStripMenuItem";
            this.AIModelSaveToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.AIModelSaveToolStripMenuItem.Text = "Save";
            this.AIModelSaveToolStripMenuItem.ToolTipText = "Saves the AI model to %appdata%.";
            this.AIModelSaveToolStripMenuItem.Click += new System.EventHandler(this.AIModelSaveToolStripMenuItem_Click);
            // 
            // AIModelLoadToolStripMenuItem1
            // 
            this.AIModelLoadToolStripMenuItem1.Name = "AIModelLoadToolStripMenuItem1";
            this.AIModelLoadToolStripMenuItem1.Size = new System.Drawing.Size(136, 22);
            this.AIModelLoadToolStripMenuItem1.Text = "Load";
            this.AIModelLoadToolStripMenuItem1.ToolTipText = "Loads the AI model from %appdata%, if present. Ignores \"Load\" if it finds no mode" +
    "l to load.";
            this.AIModelLoadToolStripMenuItem1.Click += new System.EventHandler(this.AIModelLoadToolStripMenuItem1_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Checked = true;
            this.viewToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PanelsNeuralNetworkToolStripMenuItem,
            this.PanelsRankingsToolStripMenuItem,
            this.PanelsSpeedometerToolStripMenuItem,
            this.PanelsVisionToolStripMenuItem,
            this.PanelsTelemetryToolStripMenuItem,
            this.PanelsInfoToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(53, 20);
            this.viewToolStripMenuItem.Text = "&Panels";
            // 
            // PanelsNeuralNetworkToolStripMenuItem
            // 
            this.PanelsNeuralNetworkToolStripMenuItem.Name = "PanelsNeuralNetworkToolStripMenuItem";
            this.PanelsNeuralNetworkToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.PanelsNeuralNetworkToolStripMenuItem.Text = "Neural Network";
            this.PanelsNeuralNetworkToolStripMenuItem.ToolTipText = "Show/hide the neural network panel.";
            this.PanelsNeuralNetworkToolStripMenuItem.Click += new System.EventHandler(this.PanelsToggleNeuralNetworkToolStripMenuItem_Click);
            // 
            // PanelsRankingsToolStripMenuItem
            // 
            this.PanelsRankingsToolStripMenuItem.Name = "PanelsRankingsToolStripMenuItem";
            this.PanelsRankingsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.PanelsRankingsToolStripMenuItem.Text = "Rankings";
            this.PanelsRankingsToolStripMenuItem.ToolTipText = "Show/hide the rankings panel.";
            this.PanelsRankingsToolStripMenuItem.Click += new System.EventHandler(this.PanelsToggleRankingsToolStripMenuItem_Click);
            // 
            // PanelsSpeedometerToolStripMenuItem
            // 
            this.PanelsSpeedometerToolStripMenuItem.Name = "PanelsSpeedometerToolStripMenuItem";
            this.PanelsSpeedometerToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.PanelsSpeedometerToolStripMenuItem.Text = "Speedometer";
            this.PanelsSpeedometerToolStripMenuItem.ToolTipText = "Show/hide the speedometer panel.";
            this.PanelsSpeedometerToolStripMenuItem.Click += new System.EventHandler(this.PanelsToggleSpeedometerToolStripMenuItem_Click);
            // 
            // PanelsVisionToolStripMenuItem
            // 
            this.PanelsVisionToolStripMenuItem.Name = "PanelsVisionToolStripMenuItem";
            this.PanelsVisionToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.PanelsVisionToolStripMenuItem.Text = "Vision";
            this.PanelsVisionToolStripMenuItem.ToolTipText = "Show/hide the vision panel.";
            this.PanelsVisionToolStripMenuItem.Click += new System.EventHandler(this.PanelsToggleVisionToolStripMenuItem_Click);
            // 
            // PanelsTelemetryToolStripMenuItem
            // 
            this.PanelsTelemetryToolStripMenuItem.Name = "PanelsTelemetryToolStripMenuItem";
            this.PanelsTelemetryToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.PanelsTelemetryToolStripMenuItem.Text = "Telemetry";
            this.PanelsTelemetryToolStripMenuItem.ToolTipText = "Show/hide the telemetry panel.";
            this.PanelsTelemetryToolStripMenuItem.Click += new System.EventHandler(this.PanelsToggleTelemetryToolStripMenuItem_Click);
            // 
            // PanelsInfoToolStripMenuItem
            // 
            this.PanelsInfoToolStripMenuItem.Checked = true;
            this.PanelsInfoToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.PanelsInfoToolStripMenuItem.Name = "PanelsInfoToolStripMenuItem";
            this.PanelsInfoToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.PanelsInfoToolStripMenuItem.Text = "Info";
            this.PanelsInfoToolStripMenuItem.ToolTipText = "Show/hide the info panel.";
            this.PanelsInfoToolStripMenuItem.Click += new System.EventHandler(this.PanelsInfoToolStripMenuItem_Click);
            // 
            // panelTrack
            // 
            this.panelTrack.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.panelTrack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.panelTrack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTrack.Location = new System.Drawing.Point(0, 24);
            this.panelTrack.Name = "panelTrack";
            this.panelTrack.Size = new System.Drawing.Size(1372, 1027);
            this.panelTrack.TabIndex = 10;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.CausesValidation = false;
            this.ClientSize = new System.Drawing.Size(1372, 1051);
            this.Controls.Add(this.panelTrack);
            this.Controls.Add(this.mainMenuStrip);
            this.Font = new System.Drawing.Font("Segoe UI Variable Text", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormMain";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Blobs, Cars and Tanks - AI Test Bed";
            this.TransparencyKey = System.Drawing.Color.LightPink;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormMain_KeyDown);
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private ToolTip toolTip1;
        private MenuStrip mainMenuStrip;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem fileExitToolStripMenuItem;
        private ToolStripMenuItem trackToolStripMenuItem;
        private ToolStripMenuItem trackNewToolStripMenuItem;
        private ToolStripMenuItem trackNewRandomToolStripMenuItem;
        private ToolStripMenuItem trackNewDrawToolStripMenuItem;
        private ToolStripMenuItem trackNewConstructorToolStripMenuItem;
        private ToolStripMenuItem trackSaveToolStripMenuItem;
        private ToolStripMenuItem trackLoadToolStripMenuItem;
        private ToolStripMenuItem configureToolStripMenuItem;
        private Panel panelTrack;
        private ToolStripMenuItem configureWorldToolStripMenuItem;
        private ToolStripMenuItem configureVisionToolStripMenuItem;
        private ToolStripMenuItem aIToolStripMenuItem;
        private ToolStripMenuItem AIModelConfigureToolStripMenuItem1;
        private ToolStripMenuItem AIModelSaveToolStripMenuItem;
        private ToolStripMenuItem AIModelLoadToolStripMenuItem1;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem PanelsNeuralNetworkToolStripMenuItem;
        private ToolStripMenuItem PanelsRankingsToolStripMenuItem;
        private ToolStripMenuItem PanelsSpeedometerToolStripMenuItem;
        private ToolStripMenuItem PanelsVisionToolStripMenuItem;
        private ToolStripMenuItem PanelsTelemetryToolStripMenuItem;
        private ToolStripMenuItem PanelsInfoToolStripMenuItem;
    }
}