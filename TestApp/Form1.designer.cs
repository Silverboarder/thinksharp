namespace TestApp
{
    partial class SteeringTestForm
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
            this.SuspendLayout();
            // 
            // pnlViewPort
            // 
            this.pnlViewPort.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlViewPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlViewPort.Location = new System.Drawing.Point(0, 0);
            this.pnlViewPort.Name = "pnlViewPort";
            this.pnlViewPort.Size = new System.Drawing.Size(584, 564);
            this.pnlViewPort.TabIndex = 0;
            // 
            // SteeringTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 564);
            this.Controls.Add(this.pnlViewPort);
            this.Name = "SteeringTestForm";
            this.Text = "ThinkSharp Steering";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlViewPort;
    }
}

