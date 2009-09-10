namespace TestApp
{
    partial class frmSteeringDemo
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
            this.pnlViewPort = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkFixedTime = new System.Windows.Forms.CheckBox();
            this.cmboSamples = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cmboUpdate = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkPenetrate = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.spinCohesion = new System.Windows.Forms.NumericUpDown();
            this.spinAlignment = new System.Windows.Forms.NumericUpDown();
            this.spinSeparation = new System.Windows.Forms.NumericUpDown();
            this.spinMaxSpeed = new System.Windows.Forms.NumericUpDown();
            this.spinMaxForce = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.cmboChaseMode = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkSpacePart = new System.Windows.Forms.CheckBox();
            this.chkRenderAides = new System.Windows.Forms.CheckBox();
            this.chkObstacles = new System.Windows.Forms.CheckBox();
            this.chkWalls = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinCohesion)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinAlignment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinSeparation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinMaxSpeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinMaxForce)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlViewPort
            // 
            this.pnlViewPort.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlViewPort.AutoSize = true;
            this.pnlViewPort.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlViewPort.Cursor = System.Windows.Forms.Cursors.Cross;
            this.pnlViewPort.Location = new System.Drawing.Point(0, 0);
            this.pnlViewPort.Name = "pnlViewPort";
            this.pnlViewPort.Size = new System.Drawing.Size(624, 566);
            this.pnlViewPort.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Beige;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(626, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(166, 566);
            this.panel1.TabIndex = 2;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkFixedTime);
            this.groupBox3.Controls.Add(this.cmboSamples);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.cmboUpdate);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.ForeColor = System.Drawing.Color.Black;
            this.groupBox3.Location = new System.Drawing.Point(2, 428);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(159, 131);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "AI / Render Updates";
            // 
            // chkFixedTime
            // 
            this.chkFixedTime.AutoSize = true;
            this.chkFixedTime.Checked = true;
            this.chkFixedTime.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkFixedTime.Location = new System.Drawing.Point(8, 108);
            this.chkFixedTime.Name = "chkFixedTime";
            this.chkFixedTime.Size = new System.Drawing.Size(136, 17);
            this.chkFixedTime.TabIndex = 6;
            this.chkFixedTime.Text = "Framerate Averager On";
            this.chkFixedTime.UseVisualStyleBackColor = true;
            // 
            // cmboSamples
            // 
            this.cmboSamples.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboSamples.FormattingEnabled = true;
            this.cmboSamples.Items.AddRange(new object[] {
            "None",
            "2",
            "5",
            "10",
            "15",
            "20"});
            this.cmboSamples.Location = new System.Drawing.Point(8, 81);
            this.cmboSamples.Name = "cmboSamples";
            this.cmboSamples.Size = new System.Drawing.Size(141, 21);
            this.cmboSamples.TabIndex = 4;
            this.cmboSamples.SelectedIndexChanged += new System.EventHandler(this.cmboSamples_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(5, 65);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(128, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "Num Smoothing Samples:";
            // 
            // cmboUpdate
            // 
            this.cmboUpdate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboUpdate.FormattingEnabled = true;
            this.cmboUpdate.Items.AddRange(new object[] {
            "None",
            "10",
            "30",
            "60",
            "100"});
            this.cmboUpdate.Location = new System.Drawing.Point(8, 41);
            this.cmboUpdate.Name = "cmboUpdate";
            this.cmboUpdate.Size = new System.Drawing.Size(141, 21);
            this.cmboUpdate.TabIndex = 2;
            this.cmboUpdate.SelectedIndexChanged += new System.EventHandler(this.cmboUpdate_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 25);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(122, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Update (in Milliseconds):";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkPenetrate);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.spinCohesion);
            this.groupBox2.Controls.Add(this.spinAlignment);
            this.groupBox2.Controls.Add(this.spinSeparation);
            this.groupBox2.Controls.Add(this.spinMaxSpeed);
            this.groupBox2.Controls.Add(this.spinMaxForce);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.cmboChaseMode);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.Black;
            this.groupBox2.Location = new System.Drawing.Point(2, 133);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(158, 289);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Steering Forces";
            // 
            // chkPenetrate
            // 
            this.chkPenetrate.AutoSize = true;
            this.chkPenetrate.Checked = true;
            this.chkPenetrate.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPenetrate.Location = new System.Drawing.Point(9, 69);
            this.chkPenetrate.Name = "chkPenetrate";
            this.chkPenetrate.Size = new System.Drawing.Size(120, 17);
            this.chkPenetrate.TabIndex = 14;
            this.chkPenetrate.Text = "Non-Penetration On";
            this.chkPenetrate.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(6, 177);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(58, 13);
            this.label10.TabIndex = 13;
            this.label10.Text = "All Entities:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(6, 94);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(77, 13);
            this.label9.TabIndex = 12;
            this.label9.Text = "\"Shark\" Entity:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 254);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Cohesion:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 228);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Alignment:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 202);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Separation:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 146);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Max Speed:";
            // 
            // spinCohesion
            // 
            this.spinCohesion.Location = new System.Drawing.Point(88, 252);
            this.spinCohesion.Name = "spinCohesion";
            this.spinCohesion.Size = new System.Drawing.Size(61, 20);
            this.spinCohesion.TabIndex = 7;
            // 
            // spinAlignment
            // 
            this.spinAlignment.Location = new System.Drawing.Point(88, 226);
            this.spinAlignment.Name = "spinAlignment";
            this.spinAlignment.Size = new System.Drawing.Size(61, 20);
            this.spinAlignment.TabIndex = 6;
            // 
            // spinSeparation
            // 
            this.spinSeparation.Location = new System.Drawing.Point(88, 200);
            this.spinSeparation.Name = "spinSeparation";
            this.spinSeparation.Size = new System.Drawing.Size(61, 20);
            this.spinSeparation.TabIndex = 5;
            // 
            // spinMaxSpeed
            // 
            this.spinMaxSpeed.DecimalPlaces = 2;
            this.spinMaxSpeed.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.spinMaxSpeed.Location = new System.Drawing.Point(89, 144);
            this.spinMaxSpeed.Name = "spinMaxSpeed";
            this.spinMaxSpeed.Size = new System.Drawing.Size(61, 20);
            this.spinMaxSpeed.TabIndex = 4;
            // 
            // spinMaxForce
            // 
            this.spinMaxForce.Location = new System.Drawing.Point(89, 118);
            this.spinMaxForce.Name = "spinMaxForce";
            this.spinMaxForce.Size = new System.Drawing.Size(61, 20);
            this.spinMaxForce.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 120);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Max Force:";
            // 
            // cmboChaseMode
            // 
            this.cmboChaseMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmboChaseMode.FormattingEnabled = true;
            this.cmboChaseMode.Items.AddRange(new object[] {
            "Pursuit",
            "Fixed Target",
            "Path Following"});
            this.cmboChaseMode.Location = new System.Drawing.Point(9, 41);
            this.cmboChaseMode.Name = "cmboChaseMode";
            this.cmboChaseMode.Size = new System.Drawing.Size(141, 21);
            this.cmboChaseMode.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Chase Mode:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkSpacePart);
            this.groupBox1.Controls.Add(this.chkRenderAides);
            this.groupBox1.Controls.Add(this.chkObstacles);
            this.groupBox1.Controls.Add(this.chkWalls);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.Black;
            this.groupBox1.Location = new System.Drawing.Point(3, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(158, 117);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Toggle";
            // 
            // chkSpacePart
            // 
            this.chkSpacePart.AutoSize = true;
            this.chkSpacePart.Location = new System.Drawing.Point(7, 92);
            this.chkSpacePart.Name = "chkSpacePart";
            this.chkSpacePart.Size = new System.Drawing.Size(112, 17);
            this.chkSpacePart.TabIndex = 3;
            this.chkSpacePart.Text = "Space Partitioning";
            this.chkSpacePart.UseVisualStyleBackColor = true;
            // 
            // chkRenderAides
            // 
            this.chkRenderAides.AutoSize = true;
            this.chkRenderAides.Location = new System.Drawing.Point(7, 68);
            this.chkRenderAides.Name = "chkRenderAides";
            this.chkRenderAides.Size = new System.Drawing.Size(83, 17);
            this.chkRenderAides.TabIndex = 2;
            this.chkRenderAides.Text = "Visual Aides";
            this.chkRenderAides.UseVisualStyleBackColor = true;
            // 
            // chkObstacles
            // 
            this.chkObstacles.AutoSize = true;
            this.chkObstacles.Checked = true;
            this.chkObstacles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkObstacles.Location = new System.Drawing.Point(7, 44);
            this.chkObstacles.Name = "chkObstacles";
            this.chkObstacles.Size = new System.Drawing.Size(73, 17);
            this.chkObstacles.TabIndex = 1;
            this.chkObstacles.Text = "Obstacles";
            this.chkObstacles.UseVisualStyleBackColor = true;
            // 
            // chkWalls
            // 
            this.chkWalls.AutoSize = true;
            this.chkWalls.Checked = true;
            this.chkWalls.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWalls.Location = new System.Drawing.Point(7, 20);
            this.chkWalls.Name = "chkWalls";
            this.chkWalls.Size = new System.Drawing.Size(52, 17);
            this.chkWalls.TabIndex = 0;
            this.chkWalls.Text = "Walls";
            this.chkWalls.UseVisualStyleBackColor = true;
            // 
            // frmSteeringDemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 566);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pnlViewPort);
            this.Name = "frmSteeringDemo";
            this.Text = "ThinkSharp Steering";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinCohesion)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinAlignment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinSeparation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinMaxSpeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spinMaxForce)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlViewPort;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkWalls;
        private System.Windows.Forms.CheckBox chkSpacePart;
        private System.Windows.Forms.CheckBox chkRenderAides;
        private System.Windows.Forms.CheckBox chkObstacles;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cmboChaseMode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown spinCohesion;
        private System.Windows.Forms.NumericUpDown spinAlignment;
        private System.Windows.Forms.NumericUpDown spinSeparation;
        private System.Windows.Forms.NumericUpDown spinMaxSpeed;
        private System.Windows.Forms.NumericUpDown spinMaxForce;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox cmboSamples;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cmboUpdate;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox chkFixedTime;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox chkPenetrate;
    }
}

