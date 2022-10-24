namespace CarsAndTanks.UX.Forms.Settings
{
    partial class FormConfigureVision
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
            this.labelDistance = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.sliderDepth = new AICarTrack.FluentSlider();
            this.numericUpDownSamplePoints = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownFieldOfVisionStopInDegrees = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownFieldOfVisionStartInDegrees = new System.Windows.Forms.NumericUpDown();
            this.pictureBoxWorldRepresentation = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSamplePoints)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFieldOfVisionStopInDegrees)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFieldOfVisionStartInDegrees)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWorldRepresentation)).BeginInit();
            this.SuspendLayout();
            // 
            // labelDistance
            // 
            this.labelDistance.Location = new System.Drawing.Point(356, 32);
            this.labelDistance.Name = "labelDistance";
            this.labelDistance.Size = new System.Drawing.Size(42, 17);
            this.labelDistance.TabIndex = 100;
            this.labelDistance.Text = "0";
            this.labelDistance.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI Variable Display Semib", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label4.Location = new System.Drawing.Point(146, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(118, 21);
            this.label4.TabIndex = 99;
            this.label4.Text = "LIDAR SENSOR";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(196, 338);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 15);
            this.label3.TabIndex = 98;
            this.label3.Text = "Samples";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(349, 310);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 15);
            this.label2.TabIndex = 97;
            this.label2.Text = "Stop angle";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 310);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 15);
            this.label1.TabIndex = 96;
            this.label1.Text = "Start angle";
            // 
            // sliderDepth
            // 
            this.sliderDepth.BackColor = System.Drawing.Color.Transparent;
            this.sliderDepth.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(94)))), ((int)(((byte)(110)))));
            this.sliderDepth.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(60)))), ((int)(((byte)(74)))));
            this.sliderDepth.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.sliderDepth.ColorSchema = AICarTrack.FluentSlider.ColorSchemas.GreenColors;
            this.sliderDepth.Cursor = System.Windows.Forms.Cursors.Hand;
            this.sliderDepth.ElapsedInnerColor = System.Drawing.SystemColors.ControlDark;
            this.sliderDepth.ElapsedPenColorBottom = System.Drawing.SystemColors.ControlDark;
            this.sliderDepth.ElapsedPenColorTop = System.Drawing.SystemColors.ControlDark;
            this.sliderDepth.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.sliderDepth.ForeColor = System.Drawing.Color.Black;
            this.sliderDepth.LargeChange = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.sliderDepth.Location = new System.Drawing.Point(353, 59);
            this.sliderDepth.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.sliderDepth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.sliderDepth.Name = "sliderDepth";
            this.sliderDepth.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.sliderDepth.ScaleDivisions = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.sliderDepth.ScaleSubDivisions = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.sliderDepth.ShowDivisionsText = false;
            this.sliderDepth.ShowSmallScale = false;
            this.sliderDepth.Size = new System.Drawing.Size(48, 198);
            this.sliderDepth.SmallChange = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.sliderDepth.TabIndex = 95;
            this.sliderDepth.Text = "fluentSliderSize";
            this.sliderDepth.ThumbInnerColor = System.Drawing.SystemColors.ControlDark;
            this.sliderDepth.ThumbOuterColor = System.Drawing.SystemColors.ControlDark;
            this.sliderDepth.ThumbPenColor = System.Drawing.SystemColors.ControlDarkDark;
            this.sliderDepth.ThumbRoundRectSize = new System.Drawing.Size(16, 16);
            this.sliderDepth.ThumbSize = new System.Drawing.Size(16, 16);
            this.sliderDepth.TickAdd = 2F;
            this.sliderDepth.TickColor = System.Drawing.Color.Black;
            this.sliderDepth.TickDivide = 1F;
            this.sliderDepth.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // numericUpDownSamplePoints
            // 
            this.numericUpDownSamplePoints.Cursor = System.Windows.Forms.Cursors.Hand;
            this.numericUpDownSamplePoints.Location = new System.Drawing.Point(198, 310);
            this.numericUpDownSamplePoints.Maximum = new decimal(new int[] {
            21,
            0,
            0,
            0});
            this.numericUpDownSamplePoints.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownSamplePoints.Name = "numericUpDownSamplePoints";
            this.numericUpDownSamplePoints.Size = new System.Drawing.Size(57, 23);
            this.numericUpDownSamplePoints.TabIndex = 92;
            this.numericUpDownSamplePoints.Tag = "7";
            this.numericUpDownSamplePoints.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numericUpDownFieldOfVisionStopInDegrees
            // 
            this.numericUpDownFieldOfVisionStopInDegrees.Cursor = System.Windows.Forms.Cursors.Hand;
            this.numericUpDownFieldOfVisionStopInDegrees.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownFieldOfVisionStopInDegrees.Location = new System.Drawing.Point(353, 282);
            this.numericUpDownFieldOfVisionStopInDegrees.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.numericUpDownFieldOfVisionStopInDegrees.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.numericUpDownFieldOfVisionStopInDegrees.Name = "numericUpDownFieldOfVisionStopInDegrees";
            this.numericUpDownFieldOfVisionStopInDegrees.Size = new System.Drawing.Size(57, 23);
            this.numericUpDownFieldOfVisionStopInDegrees.TabIndex = 93;
            this.numericUpDownFieldOfVisionStopInDegrees.Tag = "7";
            // 
            // numericUpDownFieldOfVisionStartInDegrees
            // 
            this.numericUpDownFieldOfVisionStartInDegrees.Cursor = System.Windows.Forms.Cursors.Hand;
            this.numericUpDownFieldOfVisionStartInDegrees.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownFieldOfVisionStartInDegrees.Location = new System.Drawing.Point(9, 282);
            this.numericUpDownFieldOfVisionStartInDegrees.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.numericUpDownFieldOfVisionStartInDegrees.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
            this.numericUpDownFieldOfVisionStartInDegrees.Name = "numericUpDownFieldOfVisionStartInDegrees";
            this.numericUpDownFieldOfVisionStartInDegrees.Size = new System.Drawing.Size(57, 23);
            this.numericUpDownFieldOfVisionStartInDegrees.TabIndex = 91;
            this.numericUpDownFieldOfVisionStartInDegrees.Tag = "7";
            // 
            // pictureBoxWorldRepresentation
            // 
            this.pictureBoxWorldRepresentation.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBoxWorldRepresentation.Location = new System.Drawing.Point(71, 31);
            this.pictureBoxWorldRepresentation.Name = "pictureBoxWorldRepresentation";
            this.pictureBoxWorldRepresentation.Size = new System.Drawing.Size(277, 276);
            this.pictureBoxWorldRepresentation.TabIndex = 94;
            this.pictureBoxWorldRepresentation.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(42, 369);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(328, 21);
            this.label5.TabIndex = 101;
            this.label5.Text = "Important: any change will reset learning.";
            // 
            // FormConfigureVision
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(425, 403);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.labelDistance);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.sliderDepth);
            this.Controls.Add(this.numericUpDownSamplePoints);
            this.Controls.Add(this.numericUpDownFieldOfVisionStopInDegrees);
            this.Controls.Add(this.numericUpDownFieldOfVisionStartInDegrees);
            this.Controls.Add(this.pictureBoxWorldRepresentation);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigureVision";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure > Vision";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormAIVisionSettings_FormClosing);
            this.Load += new System.EventHandler(this.FormAIVisionSettings_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormConfigureVision_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSamplePoints)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFieldOfVisionStopInDegrees)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownFieldOfVisionStartInDegrees)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWorldRepresentation)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label labelDistance;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private AICarTrack.FluentSlider sliderDepth;
        private NumericUpDown numericUpDownSamplePoints;
        private NumericUpDown numericUpDownFieldOfVisionStopInDegrees;
        private NumericUpDown numericUpDownFieldOfVisionStartInDegrees;
        private PictureBox pictureBoxWorldRepresentation;
        private Label label5;
    }
}