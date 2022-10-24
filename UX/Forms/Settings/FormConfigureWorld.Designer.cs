using CarsAndTanks.UX.Controls;

namespace CarsAndTanks.UX.Forms.Settings;

partial class FormConfigureWorld
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbPhysicsReal = new System.Windows.Forms.RadioButton();
            this.rbPhysicsBasic = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.panel9 = new System.Windows.Forms.Panel();
            this.rbTelemetryOff = new System.Windows.Forms.RadioButton();
            this.rbTelemetryOn = new System.Windows.Forms.RadioButton();
            this.label13 = new System.Windows.Forms.Label();
            this.panel8 = new System.Windows.Forms.Panel();
            this.label12 = new System.Windows.Forms.Label();
            this.numericUpDownEliminatedMoves = new System.Windows.Forms.NumericUpDown();
            this.rbDisplayEliminatedCarsOff = new System.Windows.Forms.RadioButton();
            this.rbDisplayEliminatedCarsOn = new System.Windows.Forms.RadioButton();
            this.label11 = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.rbRandomColorsOff = new System.Windows.Forms.RadioButton();
            this.rbRandomColorsOn = new System.Windows.Forms.RadioButton();
            this.label10 = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.rbShowHitPointOff = new System.Windows.Forms.RadioButton();
            this.rbShowHitPointOn = new System.Windows.Forms.RadioButton();
            this.label9 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.rbDisplayGatesOff = new System.Windows.Forms.RadioButton();
            this.rbDisplayGatesOn = new System.Windows.Forms.RadioButton();
            this.label8 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.rbFlashAtGatesOff = new System.Windows.Forms.RadioButton();
            this.rbFlashAtGatesOn = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.rbCarLabelOutside = new System.Windows.Forms.RadioButton();
            this.rbCarLabelMiddle = new System.Windows.Forms.RadioButton();
            this.rbCarLabelOff = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rbShapeTank = new System.Windows.Forms.RadioButton();
            this.rbShapeKart = new System.Windows.Forms.RadioButton();
            this.rbShapeBlob = new System.Windows.Forms.RadioButton();
            this.label6 = new System.Windows.Forms.Label();
            this.sliderGateThreshold = new CarsAndTanks.UX.Controls.FluentSlider();
            this.pictureBoxWorldRepresentation = new System.Windows.Forms.PictureBox();
            this.label5 = new System.Windows.Forms.Label();
            this.sliderRoadWidth = new CarsAndTanks.UX.Controls.FluentSlider();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.sliderCarSize = new CarsAndTanks.UX.Controls.FluentSlider();
            this.label2 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel9.SuspendLayout();
            this.panel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEliminatedMoves)).BeginInit();
            this.panel7.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWorldRepresentation)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbPhysicsReal);
            this.panel1.Controls.Add(this.rbPhysicsBasic);
            this.panel1.Location = new System.Drawing.Point(163, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(258, 29);
            this.panel1.TabIndex = 21;
            // 
            // rbPhysicsReal
            // 
            this.rbPhysicsReal.AutoSize = true;
            this.rbPhysicsReal.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbPhysicsReal.Location = new System.Drawing.Point(105, 4);
            this.rbPhysicsReal.Name = "rbPhysicsReal";
            this.rbPhysicsReal.Size = new System.Drawing.Size(47, 19);
            this.rbPhysicsReal.TabIndex = 2;
            this.rbPhysicsReal.Text = "Real";
            this.rbPhysicsReal.UseVisualStyleBackColor = true;
            // 
            // rbPhysicsBasic
            // 
            this.rbPhysicsBasic.AutoSize = true;
            this.rbPhysicsBasic.Checked = true;
            this.rbPhysicsBasic.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbPhysicsBasic.Location = new System.Drawing.Point(3, 4);
            this.rbPhysicsBasic.Name = "rbPhysicsBasic";
            this.rbPhysicsBasic.Size = new System.Drawing.Size(52, 19);
            this.rbPhysicsBasic.TabIndex = 1;
            this.rbPhysicsBasic.TabStop = true;
            this.rbPhysicsBasic.Text = "Basic";
            this.rbPhysicsBasic.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 15);
            this.label1.TabIndex = 20;
            this.label1.Text = "Physics:";
            // 
            // panel9
            // 
            this.panel9.Controls.Add(this.rbTelemetryOff);
            this.panel9.Controls.Add(this.rbTelemetryOn);
            this.panel9.Location = new System.Drawing.Point(163, 540);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(258, 29);
            this.panel9.TabIndex = 51;
            // 
            // rbTelemetryOff
            // 
            this.rbTelemetryOff.AutoSize = true;
            this.rbTelemetryOff.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbTelemetryOff.Location = new System.Drawing.Point(105, 4);
            this.rbTelemetryOff.Name = "rbTelemetryOff";
            this.rbTelemetryOff.Size = new System.Drawing.Size(42, 19);
            this.rbTelemetryOff.TabIndex = 21;
            this.rbTelemetryOff.Text = "Off";
            this.rbTelemetryOff.UseVisualStyleBackColor = true;
            // 
            // rbTelemetryOn
            // 
            this.rbTelemetryOn.AutoSize = true;
            this.rbTelemetryOn.Checked = true;
            this.rbTelemetryOn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbTelemetryOn.Location = new System.Drawing.Point(3, 4);
            this.rbTelemetryOn.Name = "rbTelemetryOn";
            this.rbTelemetryOn.Size = new System.Drawing.Size(41, 19);
            this.rbTelemetryOn.TabIndex = 20;
            this.rbTelemetryOn.TabStop = true;
            this.rbTelemetryOn.Text = "On";
            this.rbTelemetryOn.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(15, 547);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(93, 15);
            this.label13.TabIndex = 47;
            this.label13.Text = "Show Telemetry:";
            // 
            // panel8
            // 
            this.panel8.Controls.Add(this.label12);
            this.panel8.Controls.Add(this.numericUpDownEliminatedMoves);
            this.panel8.Controls.Add(this.rbDisplayEliminatedCarsOff);
            this.panel8.Controls.Add(this.rbDisplayEliminatedCarsOn);
            this.panel8.Location = new System.Drawing.Point(163, 321);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(258, 68);
            this.panel8.TabIndex = 55;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(109, 35);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(42, 15);
            this.label12.TabIndex = 8;
            this.label12.Text = "moves";
            // 
            // numericUpDownEliminatedMoves
            // 
            this.numericUpDownEliminatedMoves.Cursor = System.Windows.Forms.Cursors.Hand;
            this.numericUpDownEliminatedMoves.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownEliminatedMoves.Location = new System.Drawing.Point(25, 33);
            this.numericUpDownEliminatedMoves.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
            this.numericUpDownEliminatedMoves.Name = "numericUpDownEliminatedMoves";
            this.numericUpDownEliminatedMoves.Size = new System.Drawing.Size(77, 23);
            this.numericUpDownEliminatedMoves.TabIndex = 16;
            // 
            // rbDisplayEliminatedCarsOff
            // 
            this.rbDisplayEliminatedCarsOff.AutoSize = true;
            this.rbDisplayEliminatedCarsOff.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbDisplayEliminatedCarsOff.Location = new System.Drawing.Point(105, 4);
            this.rbDisplayEliminatedCarsOff.Name = "rbDisplayEliminatedCarsOff";
            this.rbDisplayEliminatedCarsOff.Size = new System.Drawing.Size(42, 19);
            this.rbDisplayEliminatedCarsOff.TabIndex = 15;
            this.rbDisplayEliminatedCarsOff.Text = "Off";
            this.rbDisplayEliminatedCarsOff.UseVisualStyleBackColor = true;
            // 
            // rbDisplayEliminatedCarsOn
            // 
            this.rbDisplayEliminatedCarsOn.AutoSize = true;
            this.rbDisplayEliminatedCarsOn.Checked = true;
            this.rbDisplayEliminatedCarsOn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbDisplayEliminatedCarsOn.Location = new System.Drawing.Point(3, 4);
            this.rbDisplayEliminatedCarsOn.Name = "rbDisplayEliminatedCarsOn";
            this.rbDisplayEliminatedCarsOn.Size = new System.Drawing.Size(41, 19);
            this.rbDisplayEliminatedCarsOn.TabIndex = 14;
            this.rbDisplayEliminatedCarsOn.TabStop = true;
            this.rbDisplayEliminatedCarsOn.Text = "On";
            this.rbDisplayEliminatedCarsOn.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(15, 328);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(131, 15);
            this.label11.TabIndex = 54;
            this.label11.Text = "Display eliminated cars:";
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.rbRandomColorsOff);
            this.panel7.Controls.Add(this.rbRandomColorsOn);
            this.panel7.Location = new System.Drawing.Point(163, 276);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(258, 29);
            this.panel7.TabIndex = 53;
            // 
            // rbRandomColorsOff
            // 
            this.rbRandomColorsOff.AutoSize = true;
            this.rbRandomColorsOff.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbRandomColorsOff.Location = new System.Drawing.Point(105, 4);
            this.rbRandomColorsOff.Name = "rbRandomColorsOff";
            this.rbRandomColorsOff.Size = new System.Drawing.Size(42, 19);
            this.rbRandomColorsOff.TabIndex = 13;
            this.rbRandomColorsOff.Text = "Off";
            this.rbRandomColorsOff.UseVisualStyleBackColor = true;
            // 
            // rbRandomColorsOn
            // 
            this.rbRandomColorsOn.AutoSize = true;
            this.rbRandomColorsOn.Checked = true;
            this.rbRandomColorsOn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbRandomColorsOn.Location = new System.Drawing.Point(3, 4);
            this.rbRandomColorsOn.Name = "rbRandomColorsOn";
            this.rbRandomColorsOn.Size = new System.Drawing.Size(41, 19);
            this.rbRandomColorsOn.TabIndex = 12;
            this.rbRandomColorsOn.TabStop = true;
            this.rbRandomColorsOn.Text = "On";
            this.rbRandomColorsOn.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(15, 283);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(116, 15);
            this.label10.TabIndex = 52;
            this.label10.Text = "Random colour cars:";
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.rbShowHitPointOff);
            this.panel6.Controls.Add(this.rbShowHitPointOn);
            this.panel6.Location = new System.Drawing.Point(163, 232);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(258, 29);
            this.panel6.TabIndex = 50;
            // 
            // rbShowHitPointOff
            // 
            this.rbShowHitPointOff.AutoSize = true;
            this.rbShowHitPointOff.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbShowHitPointOff.Location = new System.Drawing.Point(105, 4);
            this.rbShowHitPointOff.Name = "rbShowHitPointOff";
            this.rbShowHitPointOff.Size = new System.Drawing.Size(42, 19);
            this.rbShowHitPointOff.TabIndex = 11;
            this.rbShowHitPointOff.Text = "Off";
            this.rbShowHitPointOff.UseVisualStyleBackColor = true;
            // 
            // rbShowHitPointOn
            // 
            this.rbShowHitPointOn.AutoSize = true;
            this.rbShowHitPointOn.Checked = true;
            this.rbShowHitPointOn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbShowHitPointOn.Location = new System.Drawing.Point(3, 4);
            this.rbShowHitPointOn.Name = "rbShowHitPointOn";
            this.rbShowHitPointOn.Size = new System.Drawing.Size(41, 19);
            this.rbShowHitPointOn.TabIndex = 10;
            this.rbShowHitPointOn.TabStop = true;
            this.rbShowHitPointOn.Text = "On";
            this.rbShowHitPointOn.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(15, 239);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(92, 15);
            this.label9.TabIndex = 46;
            this.label9.Text = "Show hit points:";
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.rbDisplayGatesOff);
            this.panel5.Controls.Add(this.rbDisplayGatesOn);
            this.panel5.Location = new System.Drawing.Point(163, 448);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(258, 29);
            this.panel5.TabIndex = 49;
            // 
            // rbDisplayGatesOff
            // 
            this.rbDisplayGatesOff.AutoSize = true;
            this.rbDisplayGatesOff.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbDisplayGatesOff.Location = new System.Drawing.Point(105, 4);
            this.rbDisplayGatesOff.Name = "rbDisplayGatesOff";
            this.rbDisplayGatesOff.Size = new System.Drawing.Size(42, 19);
            this.rbDisplayGatesOff.TabIndex = 19;
            this.rbDisplayGatesOff.Text = "Off";
            this.rbDisplayGatesOff.UseVisualStyleBackColor = true;
            // 
            // rbDisplayGatesOn
            // 
            this.rbDisplayGatesOn.AutoSize = true;
            this.rbDisplayGatesOn.Checked = true;
            this.rbDisplayGatesOn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbDisplayGatesOn.Location = new System.Drawing.Point(3, 4);
            this.rbDisplayGatesOn.Name = "rbDisplayGatesOn";
            this.rbDisplayGatesOn.Size = new System.Drawing.Size(41, 19);
            this.rbDisplayGatesOn.TabIndex = 18;
            this.rbDisplayGatesOn.TabStop = true;
            this.rbDisplayGatesOn.Text = "On";
            this.rbDisplayGatesOn.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 455);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(80, 15);
            this.label8.TabIndex = 48;
            this.label8.Text = "Display Gates:";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.rbFlashAtGatesOff);
            this.panel4.Controls.Add(this.rbFlashAtGatesOn);
            this.panel4.Location = new System.Drawing.Point(163, 495);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(258, 29);
            this.panel4.TabIndex = 45;
            // 
            // rbFlashAtGatesOff
            // 
            this.rbFlashAtGatesOff.AutoSize = true;
            this.rbFlashAtGatesOff.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbFlashAtGatesOff.Location = new System.Drawing.Point(105, 4);
            this.rbFlashAtGatesOff.Name = "rbFlashAtGatesOff";
            this.rbFlashAtGatesOff.Size = new System.Drawing.Size(42, 19);
            this.rbFlashAtGatesOff.TabIndex = 21;
            this.rbFlashAtGatesOff.Text = "Off";
            this.rbFlashAtGatesOff.UseVisualStyleBackColor = true;
            // 
            // rbFlashAtGatesOn
            // 
            this.rbFlashAtGatesOn.AutoSize = true;
            this.rbFlashAtGatesOn.Checked = true;
            this.rbFlashAtGatesOn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbFlashAtGatesOn.Location = new System.Drawing.Point(3, 4);
            this.rbFlashAtGatesOn.Name = "rbFlashAtGatesOn";
            this.rbFlashAtGatesOn.Size = new System.Drawing.Size(41, 19);
            this.rbFlashAtGatesOn.TabIndex = 20;
            this.rbFlashAtGatesOn.TabStop = true;
            this.rbFlashAtGatesOn.Text = "On";
            this.rbFlashAtGatesOn.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 502);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(81, 15);
            this.label7.TabIndex = 44;
            this.label7.Text = "Flash at gates:";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.rbCarLabelOutside);
            this.panel3.Controls.Add(this.rbCarLabelMiddle);
            this.panel3.Controls.Add(this.rbCarLabelOff);
            this.panel3.Location = new System.Drawing.Point(163, 188);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(310, 30);
            this.panel3.TabIndex = 42;
            // 
            // rbCarLabelOutside
            // 
            this.rbCarLabelOutside.AutoSize = true;
            this.rbCarLabelOutside.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbCarLabelOutside.Location = new System.Drawing.Point(219, 4);
            this.rbCarLabelOutside.Name = "rbCarLabelOutside";
            this.rbCarLabelOutside.Size = new System.Drawing.Size(66, 19);
            this.rbCarLabelOutside.TabIndex = 9;
            this.rbCarLabelOutside.Text = "Outside";
            this.rbCarLabelOutside.UseVisualStyleBackColor = true;
            // 
            // rbCarLabelMiddle
            // 
            this.rbCarLabelMiddle.AutoSize = true;
            this.rbCarLabelMiddle.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbCarLabelMiddle.Location = new System.Drawing.Point(105, 4);
            this.rbCarLabelMiddle.Name = "rbCarLabelMiddle";
            this.rbCarLabelMiddle.Size = new System.Drawing.Size(62, 19);
            this.rbCarLabelMiddle.TabIndex = 8;
            this.rbCarLabelMiddle.Text = "Middle";
            this.rbCarLabelMiddle.UseVisualStyleBackColor = true;
            // 
            // rbCarLabelOff
            // 
            this.rbCarLabelOff.AutoSize = true;
            this.rbCarLabelOff.Checked = true;
            this.rbCarLabelOff.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbCarLabelOff.Location = new System.Drawing.Point(3, 4);
            this.rbCarLabelOff.Name = "rbCarLabelOff";
            this.rbCarLabelOff.Size = new System.Drawing.Size(42, 19);
            this.rbCarLabelOff.TabIndex = 7;
            this.rbCarLabelOff.TabStop = true;
            this.rbCarLabelOff.Text = "Off";
            this.rbCarLabelOff.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rbShapeTank);
            this.panel2.Controls.Add(this.rbShapeKart);
            this.panel2.Controls.Add(this.rbShapeBlob);
            this.panel2.Location = new System.Drawing.Point(163, 52);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(319, 31);
            this.panel2.TabIndex = 41;
            // 
            // rbShapeTank
            // 
            this.rbShapeTank.AutoSize = true;
            this.rbShapeTank.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbShapeTank.Location = new System.Drawing.Point(219, 4);
            this.rbShapeTank.Name = "rbShapeTank";
            this.rbShapeTank.Size = new System.Drawing.Size(49, 19);
            this.rbShapeTank.TabIndex = 5;
            this.rbShapeTank.Text = "Tank";
            this.rbShapeTank.UseVisualStyleBackColor = true;
            // 
            // rbShapeKart
            // 
            this.rbShapeKart.AutoSize = true;
            this.rbShapeKart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbShapeKart.Location = new System.Drawing.Point(105, 4);
            this.rbShapeKart.Name = "rbShapeKart";
            this.rbShapeKart.Size = new System.Drawing.Size(64, 19);
            this.rbShapeKart.TabIndex = 4;
            this.rbShapeKart.Text = "Go Kart";
            this.rbShapeKart.UseVisualStyleBackColor = true;
            // 
            // rbShapeBlob
            // 
            this.rbShapeBlob.AutoSize = true;
            this.rbShapeBlob.Checked = true;
            this.rbShapeBlob.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rbShapeBlob.Location = new System.Drawing.Point(3, 4);
            this.rbShapeBlob.Name = "rbShapeBlob";
            this.rbShapeBlob.Size = new System.Drawing.Size(49, 19);
            this.rbShapeBlob.TabIndex = 3;
            this.rbShapeBlob.TabStop = true;
            this.rbShapeBlob.Text = "Blob";
            this.rbShapeBlob.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 411);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 15);
            this.label6.TabIndex = 40;
            this.label6.Text = "Gate Threshold:";
            // 
            // sliderGateThreshold
            // 
            this.sliderGateThreshold.BackColor = System.Drawing.Color.Transparent;
            this.sliderGateThreshold.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(94)))), ((int)(((byte)(110)))));
            this.sliderGateThreshold.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(60)))), ((int)(((byte)(74)))));
            this.sliderGateThreshold.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.sliderGateThreshold.ColorSchema = CarsAndTanks.UX.Controls.FluentSlider.ColorSchemas.GreenColors;
            this.sliderGateThreshold.Cursor = System.Windows.Forms.Cursors.Hand;
            this.sliderGateThreshold.ElapsedInnerColor = System.Drawing.SystemColors.ControlDark;
            this.sliderGateThreshold.ElapsedPenColorBottom = System.Drawing.SystemColors.ControlDark;
            this.sliderGateThreshold.ElapsedPenColorTop = System.Drawing.SystemColors.ControlDark;
            this.sliderGateThreshold.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.sliderGateThreshold.ForeColor = System.Drawing.Color.Black;
            this.sliderGateThreshold.LargeChange = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.sliderGateThreshold.Location = new System.Drawing.Point(163, 395);
            this.sliderGateThreshold.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.sliderGateThreshold.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.sliderGateThreshold.Name = "sliderGateThreshold";
            this.sliderGateThreshold.ScaleDivisions = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.sliderGateThreshold.ScaleSubDivisions = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.sliderGateThreshold.ShowDivisionsText = false;
            this.sliderGateThreshold.ShowSmallScale = false;
            this.sliderGateThreshold.Size = new System.Drawing.Size(258, 48);
            this.sliderGateThreshold.SmallChange = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.sliderGateThreshold.TabIndex = 39;
            this.sliderGateThreshold.Text = "fluentSliderSize";
            this.sliderGateThreshold.ThumbInnerColor = System.Drawing.SystemColors.ControlDark;
            this.sliderGateThreshold.ThumbOuterColor = System.Drawing.SystemColors.ControlDark;
            this.sliderGateThreshold.ThumbPenColor = System.Drawing.SystemColors.ControlDarkDark;
            this.sliderGateThreshold.ThumbRoundRectSize = new System.Drawing.Size(16, 16);
            this.sliderGateThreshold.ThumbSize = new System.Drawing.Size(16, 16);
            this.sliderGateThreshold.TickAdd = 2F;
            this.sliderGateThreshold.TickColor = System.Drawing.Color.Black;
            this.sliderGateThreshold.TickDivide = 1F;
            this.sliderGateThreshold.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // pictureBoxWorldRepresentation
            // 
            this.pictureBoxWorldRepresentation.Location = new System.Drawing.Point(506, 60);
            this.pictureBoxWorldRepresentation.Name = "pictureBoxWorldRepresentation";
            this.pictureBoxWorldRepresentation.Size = new System.Drawing.Size(272, 174);
            this.pictureBoxWorldRepresentation.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBoxWorldRepresentation.TabIndex = 38;
            this.pictureBoxWorldRepresentation.TabStop = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 154);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 15);
            this.label5.TabIndex = 37;
            this.label5.Text = "Road Width:";
            // 
            // sliderRoadWidth
            // 
            this.sliderRoadWidth.BackColor = System.Drawing.Color.Transparent;
            this.sliderRoadWidth.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(94)))), ((int)(((byte)(110)))));
            this.sliderRoadWidth.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(60)))), ((int)(((byte)(74)))));
            this.sliderRoadWidth.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.sliderRoadWidth.ColorSchema = CarsAndTanks.UX.Controls.FluentSlider.ColorSchemas.GreenColors;
            this.sliderRoadWidth.Cursor = System.Windows.Forms.Cursors.Hand;
            this.sliderRoadWidth.ElapsedInnerColor = System.Drawing.SystemColors.ControlDark;
            this.sliderRoadWidth.ElapsedPenColorBottom = System.Drawing.SystemColors.ControlDark;
            this.sliderRoadWidth.ElapsedPenColorTop = System.Drawing.SystemColors.ControlDark;
            this.sliderRoadWidth.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.sliderRoadWidth.ForeColor = System.Drawing.Color.Black;
            this.sliderRoadWidth.LargeChange = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.sliderRoadWidth.Location = new System.Drawing.Point(163, 140);
            this.sliderRoadWidth.Maximum = new decimal(new int[] {
            130,
            0,
            0,
            0});
            this.sliderRoadWidth.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.sliderRoadWidth.Name = "sliderRoadWidth";
            this.sliderRoadWidth.ScaleDivisions = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.sliderRoadWidth.ScaleSubDivisions = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.sliderRoadWidth.ShowDivisionsText = false;
            this.sliderRoadWidth.ShowSmallScale = false;
            this.sliderRoadWidth.Size = new System.Drawing.Size(258, 48);
            this.sliderRoadWidth.SmallChange = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.sliderRoadWidth.TabIndex = 34;
            this.sliderRoadWidth.Text = "fluentSliderSize";
            this.sliderRoadWidth.ThumbInnerColor = System.Drawing.SystemColors.ControlDark;
            this.sliderRoadWidth.ThumbOuterColor = System.Drawing.SystemColors.ControlDark;
            this.sliderRoadWidth.ThumbPenColor = System.Drawing.SystemColors.ControlDarkDark;
            this.sliderRoadWidth.ThumbRoundRectSize = new System.Drawing.Size(16, 16);
            this.sliderRoadWidth.ThumbSize = new System.Drawing.Size(16, 16);
            this.sliderRoadWidth.TickAdd = 2F;
            this.sliderRoadWidth.TickColor = System.Drawing.Color.Black;
            this.sliderRoadWidth.TickDivide = 1F;
            this.sliderRoadWidth.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 196);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 15);
            this.label4.TabIndex = 36;
            this.label4.Text = "Car Label:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 109);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 15);
            this.label3.TabIndex = 35;
            this.label3.Text = "Car Size:";
            // 
            // sliderCarSize
            // 
            this.sliderCarSize.BackColor = System.Drawing.Color.Transparent;
            this.sliderCarSize.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(94)))), ((int)(((byte)(110)))));
            this.sliderCarSize.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(60)))), ((int)(((byte)(74)))));
            this.sliderCarSize.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.sliderCarSize.ColorSchema = CarsAndTanks.UX.Controls.FluentSlider.ColorSchemas.GreenColors;
            this.sliderCarSize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.sliderCarSize.ElapsedInnerColor = System.Drawing.SystemColors.ControlDark;
            this.sliderCarSize.ElapsedPenColorBottom = System.Drawing.SystemColors.ControlDark;
            this.sliderCarSize.ElapsedPenColorTop = System.Drawing.SystemColors.ControlDark;
            this.sliderCarSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.sliderCarSize.ForeColor = System.Drawing.Color.Black;
            this.sliderCarSize.LargeChange = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.sliderCarSize.Location = new System.Drawing.Point(163, 93);
            this.sliderCarSize.Maximum = new decimal(new int[] {
            90,
            0,
            0,
            0});
            this.sliderCarSize.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.sliderCarSize.Name = "sliderCarSize";
            this.sliderCarSize.ScaleDivisions = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.sliderCarSize.ScaleSubDivisions = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.sliderCarSize.ShowDivisionsText = false;
            this.sliderCarSize.ShowSmallScale = false;
            this.sliderCarSize.Size = new System.Drawing.Size(258, 48);
            this.sliderCarSize.SmallChange = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.sliderCarSize.TabIndex = 33;
            this.sliderCarSize.Text = "fluentSliderSize";
            this.sliderCarSize.ThumbInnerColor = System.Drawing.SystemColors.ControlDark;
            this.sliderCarSize.ThumbOuterColor = System.Drawing.SystemColors.ControlDark;
            this.sliderCarSize.ThumbPenColor = System.Drawing.SystemColors.ControlDarkDark;
            this.sliderCarSize.ThumbRoundRectSize = new System.Drawing.Size(16, 16);
            this.sliderCarSize.ThumbSize = new System.Drawing.Size(16, 16);
            this.sliderCarSize.TickAdd = 2F;
            this.sliderCarSize.TickColor = System.Drawing.Color.Black;
            this.sliderCarSize.TickDivide = 1F;
            this.sliderCarSize.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 15);
            this.label2.TabIndex = 32;
            this.label2.Text = "Shape:";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Segoe UI Variable Small", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label23.ForeColor = System.Drawing.Color.Red;
            this.label23.Location = new System.Drawing.Point(61, 22);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(12, 15);
            this.label23.TabIndex = 141;
            this.label23.Text = "*";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Segoe UI Variable Small", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label15.ForeColor = System.Drawing.Color.Red;
            this.label15.Location = new System.Drawing.Point(54, 63);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(12, 15);
            this.label15.TabIndex = 142;
            this.label15.Text = "*";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Segoe UI Variable Small", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label16.ForeColor = System.Drawing.Color.Red;
            this.label16.Location = new System.Drawing.Point(62, 112);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(12, 15);
            this.label16.TabIndex = 143;
            this.label16.Text = "*";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Segoe UI Variable Small", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label17.ForeColor = System.Drawing.Color.Red;
            this.label17.Location = new System.Drawing.Point(83, 157);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(12, 15);
            this.label17.TabIndex = 144;
            this.label17.Text = "*";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Segoe UI Variable Small", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label18.ForeColor = System.Drawing.Color.Red;
            this.label18.Location = new System.Drawing.Point(12, 603);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(279, 21);
            this.label18.TabIndex = 150;
            this.label18.Text = "* indicates change will reset learning.";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Segoe UI Variable Small", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label14.ForeColor = System.Drawing.Color.Black;
            this.label14.Location = new System.Drawing.Point(427, 410);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(198, 16);
            this.label14.TabIndex = 151;
            this.label14.Text = "(applies to new/loaded tracks only.)";
            // 
            // FormConfigureWorld
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(797, 646);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.panel9);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.panel8);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.sliderGateThreshold);
            this.Controls.Add(this.pictureBoxWorldRepresentation);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.sliderRoadWidth);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.sliderCarSize);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfigureWorld";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure > World";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormConfigureWorld_FormClosing);
            this.Load += new System.EventHandler(this.FormWorldSettings_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormConfigureWorld_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel9.ResumeLayout(false);
            this.panel9.PerformLayout();
            this.panel8.ResumeLayout(false);
            this.panel8.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownEliminatedMoves)).EndInit();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxWorldRepresentation)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private Panel panel1;
    private RadioButton rbPhysicsReal;
    private RadioButton rbPhysicsBasic;
    private Label label1;
    private Panel panel9;
    private RadioButton rbTelemetryOff;
    private RadioButton rbTelemetryOn;
    private Label label13;
    private Panel panel8;
    private Label label12;
    private NumericUpDown numericUpDownEliminatedMoves;
    private RadioButton rbDisplayEliminatedCarsOff;
    private RadioButton rbDisplayEliminatedCarsOn;
    private Label label11;
    private Panel panel7;
    private RadioButton rbRandomColorsOff;
    private RadioButton rbRandomColorsOn;
    private Label label10;
    private Panel panel6;
    private RadioButton rbShowHitPointOff;
    private RadioButton rbShowHitPointOn;
    private Label label9;
    private Panel panel5;
    private RadioButton rbDisplayGatesOff;
    private RadioButton rbDisplayGatesOn;
    private Label label8;
    private Panel panel4;
    private RadioButton rbFlashAtGatesOff;
    private RadioButton rbFlashAtGatesOn;
    private Label label7;
    private Panel panel3;
    private RadioButton rbCarLabelOutside;
    private RadioButton rbCarLabelMiddle;
    private RadioButton rbCarLabelOff;
    private Panel panel2;
    private RadioButton rbShapeTank;
    private RadioButton rbShapeKart;
    private RadioButton rbShapeBlob;
    private Label label6;
    private FluentSlider sliderGateThreshold;
    private PictureBox pictureBoxWorldRepresentation;
    private Label label5;
    private FluentSlider sliderRoadWidth;
    private Label label4;
    private Label label3;
    private FluentSlider sliderCarSize;
    private Label label2;
    private Label label23;
    private Label label15;
    private Label label16;
    private Label label17;
    private Label label18;
    private Label label14;
}