namespace TestApp
{
    partial class frmPathFinderDemo
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
            this.pnlViewPort = new System.Windows.Forms.Panel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnAlgoDF = new System.Windows.Forms.Button();
            this.btnAlgoAStar = new System.Windows.Forms.Button();
            this.btnAlgoBF = new System.Windows.Forms.Button();
            this.btnBrushNormal = new System.Windows.Forms.Button();
            this.btnAlgoDj = new System.Windows.Forms.Button();
            this.btnBrushWater = new System.Windows.Forms.Button();
            this.btnBrushMud = new System.Windows.Forms.Button();
            this.btnBrushObstacle = new System.Windows.Forms.Button();
            this.btnBrushTarget = new System.Windows.Forms.Button();
            this.btnBrushSource = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.MenuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuNew = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuView = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuGraph = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuTiles = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
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
            this.pnlViewPort.Location = new System.Drawing.Point(3, 27);
            this.pnlViewPort.Name = "pnlViewPort";
            this.pnlViewPort.Size = new System.Drawing.Size(528, 427);
            this.pnlViewPort.TabIndex = 1;
            this.pnlViewPort.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlViewPort_Paint);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 492);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(534, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(39, 17);
            this.toolStripStatusLabel1.Text = "Status";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.btnAlgoDF);
            this.panel1.Controls.Add(this.btnAlgoAStar);
            this.panel1.Controls.Add(this.btnAlgoBF);
            this.panel1.Controls.Add(this.btnBrushNormal);
            this.panel1.Controls.Add(this.btnAlgoDj);
            this.panel1.Controls.Add(this.btnBrushWater);
            this.panel1.Controls.Add(this.btnBrushMud);
            this.panel1.Controls.Add(this.btnBrushObstacle);
            this.panel1.Controls.Add(this.btnBrushTarget);
            this.panel1.Controls.Add(this.btnBrushSource);
            this.panel1.Location = new System.Drawing.Point(0, 460);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(534, 32);
            this.panel1.TabIndex = 4;
            // 
            // btnAlgoDF
            // 
            this.btnAlgoDF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAlgoDF.FlatAppearance.BorderColor = System.Drawing.Color.DarkSlateGray;
            this.btnAlgoDF.FlatAppearance.BorderSize = 2;
            this.btnAlgoDF.Location = new System.Drawing.Point(368, 2);
            this.btnAlgoDF.Name = "btnAlgoDF";
            this.btnAlgoDF.Size = new System.Drawing.Size(34, 25);
            this.btnAlgoDF.TabIndex = 6;
            this.btnAlgoDF.Text = "DF";
            this.toolTip1.SetToolTip(this.btnAlgoDF, "Depth First");
            this.btnAlgoDF.UseVisualStyleBackColor = false;
            this.btnAlgoDF.Click += new System.EventHandler(this.btnAlgo_Click);
            // 
            // btnAlgoAStar
            // 
            this.btnAlgoAStar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAlgoAStar.FlatAppearance.BorderColor = System.Drawing.Color.DarkSlateGray;
            this.btnAlgoAStar.FlatAppearance.BorderSize = 2;
            this.btnAlgoAStar.Location = new System.Drawing.Point(488, 2);
            this.btnAlgoAStar.Name = "btnAlgoAStar";
            this.btnAlgoAStar.Size = new System.Drawing.Size(34, 25);
            this.btnAlgoAStar.TabIndex = 9;
            this.btnAlgoAStar.Text = "A*";
            this.toolTip1.SetToolTip(this.btnAlgoAStar, "A Star");
            this.btnAlgoAStar.UseVisualStyleBackColor = false;
            this.btnAlgoAStar.Click += new System.EventHandler(this.btnAlgo_Click);
            // 
            // btnAlgoBF
            // 
            this.btnAlgoBF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAlgoBF.FlatAppearance.BorderColor = System.Drawing.Color.DarkSlateGray;
            this.btnAlgoBF.FlatAppearance.BorderSize = 2;
            this.btnAlgoBF.Location = new System.Drawing.Point(408, 2);
            this.btnAlgoBF.Name = "btnAlgoBF";
            this.btnAlgoBF.Size = new System.Drawing.Size(34, 25);
            this.btnAlgoBF.TabIndex = 7;
            this.btnAlgoBF.Text = "BF";
            this.toolTip1.SetToolTip(this.btnAlgoBF, "Breadth First");
            this.btnAlgoBF.UseVisualStyleBackColor = false;
            this.btnAlgoBF.Click += new System.EventHandler(this.btnAlgo_Click);
            // 
            // btnBrushNormal
            // 
            this.btnBrushNormal.BackColor = System.Drawing.Color.White;
            this.btnBrushNormal.FlatAppearance.BorderColor = System.Drawing.Color.DarkSlateGray;
            this.btnBrushNormal.FlatAppearance.BorderSize = 2;
            this.btnBrushNormal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrushNormal.Location = new System.Drawing.Point(148, 3);
            this.btnBrushNormal.Name = "btnBrushNormal";
            this.btnBrushNormal.Size = new System.Drawing.Size(23, 23);
            this.btnBrushNormal.TabIndex = 5;
            this.toolTip1.SetToolTip(this.btnBrushNormal, "Normal Brush");
            this.btnBrushNormal.UseVisualStyleBackColor = false;
            this.btnBrushNormal.Click += new System.EventHandler(this.btnBrush_Click);
            // 
            // btnAlgoDj
            // 
            this.btnAlgoDj.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAlgoDj.FlatAppearance.BorderColor = System.Drawing.Color.DarkSlateGray;
            this.btnAlgoDj.FlatAppearance.BorderSize = 2;
            this.btnAlgoDj.Location = new System.Drawing.Point(448, 2);
            this.btnAlgoDj.Name = "btnAlgoDj";
            this.btnAlgoDj.Size = new System.Drawing.Size(34, 25);
            this.btnAlgoDj.TabIndex = 8;
            this.btnAlgoDj.Text = "Dj";
            this.toolTip1.SetToolTip(this.btnAlgoDj, "Dijkstra");
            this.btnAlgoDj.UseVisualStyleBackColor = false;
            this.btnAlgoDj.Click += new System.EventHandler(this.btnAlgo_Click);
            // 
            // btnBrushWater
            // 
            this.btnBrushWater.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnBrushWater.FlatAppearance.BorderColor = System.Drawing.Color.DarkSlateGray;
            this.btnBrushWater.FlatAppearance.BorderSize = 2;
            this.btnBrushWater.Location = new System.Drawing.Point(119, 3);
            this.btnBrushWater.Name = "btnBrushWater";
            this.btnBrushWater.Size = new System.Drawing.Size(23, 23);
            this.btnBrushWater.TabIndex = 4;
            this.toolTip1.SetToolTip(this.btnBrushWater, "Water Brush");
            this.btnBrushWater.UseVisualStyleBackColor = false;
            this.btnBrushWater.Click += new System.EventHandler(this.btnBrush_Click);
            // 
            // btnBrushMud
            // 
            this.btnBrushMud.BackColor = System.Drawing.Color.Brown;
            this.btnBrushMud.FlatAppearance.BorderColor = System.Drawing.Color.DarkSlateGray;
            this.btnBrushMud.FlatAppearance.BorderSize = 2;
            this.btnBrushMud.Location = new System.Drawing.Point(90, 3);
            this.btnBrushMud.Name = "btnBrushMud";
            this.btnBrushMud.Size = new System.Drawing.Size(23, 23);
            this.btnBrushMud.TabIndex = 3;
            this.toolTip1.SetToolTip(this.btnBrushMud, "Mud Brush");
            this.btnBrushMud.UseVisualStyleBackColor = false;
            this.btnBrushMud.Click += new System.EventHandler(this.btnBrush_Click);
            // 
            // btnBrushObstacle
            // 
            this.btnBrushObstacle.BackColor = System.Drawing.Color.Black;
            this.btnBrushObstacle.FlatAppearance.BorderColor = System.Drawing.Color.DarkSlateGray;
            this.btnBrushObstacle.FlatAppearance.BorderSize = 2;
            this.btnBrushObstacle.Location = new System.Drawing.Point(61, 3);
            this.btnBrushObstacle.Name = "btnBrushObstacle";
            this.btnBrushObstacle.Size = new System.Drawing.Size(23, 23);
            this.btnBrushObstacle.TabIndex = 2;
            this.toolTip1.SetToolTip(this.btnBrushObstacle, "Obstacle Brush");
            this.btnBrushObstacle.UseVisualStyleBackColor = false;
            this.btnBrushObstacle.Click += new System.EventHandler(this.btnBrush_Click);
            // 
            // btnBrushTarget
            // 
            this.btnBrushTarget.BackColor = System.Drawing.Color.Red;
            this.btnBrushTarget.FlatAppearance.BorderColor = System.Drawing.Color.DarkSlateGray;
            this.btnBrushTarget.FlatAppearance.BorderSize = 2;
            this.btnBrushTarget.Location = new System.Drawing.Point(32, 3);
            this.btnBrushTarget.Name = "btnBrushTarget";
            this.btnBrushTarget.Size = new System.Drawing.Size(23, 23);
            this.btnBrushTarget.TabIndex = 1;
            this.toolTip1.SetToolTip(this.btnBrushTarget, "Target Brush");
            this.btnBrushTarget.UseVisualStyleBackColor = false;
            this.btnBrushTarget.Click += new System.EventHandler(this.btnBrush_Click);
            // 
            // btnBrushSource
            // 
            this.btnBrushSource.BackColor = System.Drawing.Color.LightGreen;
            this.btnBrushSource.FlatAppearance.BorderColor = System.Drawing.Color.DarkSlateGray;
            this.btnBrushSource.FlatAppearance.BorderSize = 2;
            this.btnBrushSource.Location = new System.Drawing.Point(3, 3);
            this.btnBrushSource.Name = "btnBrushSource";
            this.btnBrushSource.Size = new System.Drawing.Size(23, 23);
            this.btnBrushSource.TabIndex = 0;
            this.toolTip1.SetToolTip(this.btnBrushSource, "Source Brush");
            this.btnBrushSource.UseVisualStyleBackColor = false;
            this.btnBrushSource.Click += new System.EventHandler(this.btnBrush_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuFile,
            this.MenuView});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(534, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // MenuFile
            // 
            this.MenuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuNew,
            this.MenuLoad,
            this.MenuSaveAs});
            this.MenuFile.Name = "MenuFile";
            this.MenuFile.Size = new System.Drawing.Size(37, 20);
            this.MenuFile.Text = "File";
            // 
            // MenuNew
            // 
            this.MenuNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.MenuNew.Name = "MenuNew";
            this.MenuNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.N)));
            this.MenuNew.Size = new System.Drawing.Size(150, 22);
            this.MenuNew.Text = "New";
            this.MenuNew.Click += new System.EventHandler(this.MenuToolStrip_Click);
            // 
            // MenuLoad
            // 
            this.MenuLoad.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.MenuLoad.Name = "MenuLoad";
            this.MenuLoad.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.L)));
            this.MenuLoad.Size = new System.Drawing.Size(150, 22);
            this.MenuLoad.Text = "Load";
            this.MenuLoad.Click += new System.EventHandler(this.MenuToolStrip_Click);
            // 
            // MenuSaveAs
            // 
            this.MenuSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.MenuSaveAs.Name = "MenuSaveAs";
            this.MenuSaveAs.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.S)));
            this.MenuSaveAs.Size = new System.Drawing.Size(150, 22);
            this.MenuSaveAs.Text = "Save As";
            this.MenuSaveAs.Click += new System.EventHandler(this.MenuToolStrip_Click);
            // 
            // MenuView
            // 
            this.MenuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuGraph,
            this.MenuTiles});
            this.MenuView.Name = "MenuView";
            this.MenuView.Size = new System.Drawing.Size(44, 20);
            this.MenuView.Text = "View";
            // 
            // MenuGraph
            // 
            this.MenuGraph.CheckOnClick = true;
            this.MenuGraph.Name = "MenuGraph";
            this.MenuGraph.Size = new System.Drawing.Size(106, 22);
            this.MenuGraph.Text = "Graph";
            this.MenuGraph.Click += new System.EventHandler(this.MenuToolStrip_Click);
            // 
            // MenuTiles
            // 
            this.MenuTiles.CheckOnClick = true;
            this.MenuTiles.Name = "MenuTiles";
            this.MenuTiles.Size = new System.Drawing.Size(106, 22);
            this.MenuTiles.Text = "Tiles";
            this.MenuTiles.Click += new System.EventHandler(this.MenuToolStrip_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "map";
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "pathfinder files (*.map)|*.map";
            this.openFileDialog1.Title = "Open pathfinder map files";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.DefaultExt = "map";
            this.saveFileDialog1.Filter = "pathfinder files (*.map)|*.map";
            this.saveFileDialog1.Title = "Save pathfinder map files";
            // 
            // frmPathFinderDemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(534, 514);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.pnlViewPort);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmPathFinderDemo";
            this.Text = "Path Finder Demo";
            this.Load += new System.EventHandler(this.frmPathFinderDemo_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlViewPort;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem MenuFile;
        private System.Windows.Forms.ToolStripMenuItem MenuNew;
        private System.Windows.Forms.ToolStripMenuItem MenuLoad;
        private System.Windows.Forms.ToolStripMenuItem MenuSaveAs;
        private System.Windows.Forms.ToolStripMenuItem MenuView;
        private System.Windows.Forms.ToolStripMenuItem MenuGraph;
        private System.Windows.Forms.ToolStripMenuItem MenuTiles;
        private System.Windows.Forms.Button btnBrushSource;
        private System.Windows.Forms.Button btnAlgoDF;
        private System.Windows.Forms.Button btnAlgoBF;
        private System.Windows.Forms.Button btnAlgoDj;
        private System.Windows.Forms.Button btnAlgoAStar;
        private System.Windows.Forms.Button btnBrushNormal;
        private System.Windows.Forms.Button btnBrushWater;
        private System.Windows.Forms.Button btnBrushMud;
        private System.Windows.Forms.Button btnBrushObstacle;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnBrushTarget;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}