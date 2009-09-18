using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ThinkSharp.PathFinding;
using ThinkSharp.Common;

namespace TestApp
{
    public partial class frmPathFinderDemo : Form
    {
        private PathFinder m_Pathfinder;
        private BufferedGraphicsContext mGraphContext;
        private BufferedGraphics mBuffer1;
        private bool m_blnIsLoading, m_blnMouseDown;
        private int m_intMouseGridIndex;

        public frmPathFinderDemo()
        {
            InitializeComponent();

            pnlViewPort.MouseDown += new MouseEventHandler(On_pnlViewPort_Down);
            pnlViewPort.MouseMove += new MouseEventHandler(On_pnlViewPort_Move);
            pnlViewPort.MouseUp += new MouseEventHandler(On_pnlViewPort_Up);
            pnlViewPort.MouseLeave += new EventHandler(On_pnlViewPort_Leave); 
            pnlViewPort.SizeChanged += new EventHandler(On_pnlViewPort_Size);
        }

        private void frmPathFinderDemo_Load(object sender, EventArgs e)
        {
            m_blnIsLoading = true;
            m_blnMouseDown = false;

            mGraphContext = BufferedGraphicsManager.Current;

            mBuffer1 = mGraphContext.Allocate(pnlViewPort.CreateGraphics(), pnlViewPort.DisplayRectangle);
            mBuffer1.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            mBuffer1.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            m_Pathfinder = new PathFinder();
            m_Pathfinder.InitialiseGraph(20, 20, pnlViewPort.Width, pnlViewPort.Height);
            m_Pathfinder.InitialiseSourceTargetIndexes();

            m_Pathfinder.ShowGraph = MenuGraph.Checked;
            m_Pathfinder.ShowTiles = MenuTiles.Checked;

            m_intMouseGridIndex = -1;
            ResetButtonAlgos();

            m_Pathfinder.CurrentTerrainBrush = GetButtonTerrainBrush();

            ReDraw();

            m_blnIsLoading = false;
        }

        private void ReDraw()
        {
            mBuffer1.Graphics.Clear(Color.White);

            m_Pathfinder.Render(mBuffer1.Graphics);

            mBuffer1.Render();
        }

        private void pnlViewPort_Paint(object sender, PaintEventArgs e)
        {
            if (!m_blnIsLoading)
            {
                ReDraw();
            }
        }

        private void On_pnlViewPort_Size(object sender, EventArgs e)
        {
            if (!m_blnIsLoading)
            {
                mBuffer1 = mGraphContext.Allocate(pnlViewPort.CreateGraphics(), pnlViewPort.DisplayRectangle);
                mBuffer1.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                MemoryStream memoryStream = new MemoryStream();
                StreamWriter memoryWriter = new StreamWriter(memoryStream);
                StreamReader memoryReader = new StreamReader(memoryStream);

                m_Pathfinder.Save(memoryWriter);

                memoryWriter.Flush();

                memoryStream.Seek(0, SeekOrigin.Begin);   // reset file pointer

                m_Pathfinder.Load(memoryReader, pnlViewPort.Width, pnlViewPort.Height);

                memoryStream.Close();

                ReDraw();
            }
        }

        private void On_pnlViewPort_Down(object sender, MouseEventArgs e)
        {
            if (!m_blnIsLoading && e.Button == MouseButtons.Left)
            {
                m_blnMouseDown = true;
                m_Pathfinder.Vector2DToIndex(new Vector2D(e.X, e.Y), ref m_intMouseGridIndex);

                m_Pathfinder.PaintTerrain(e.Location);
                ReDraw();
                updateGraphStatus();
            }
        }

        private void On_pnlViewPort_Move(object sender, MouseEventArgs e)
        {
            if (!m_blnIsLoading)
            {
                if (m_blnMouseDown)
                {
                    int TestIndex = -1;

                    if (m_Pathfinder.Vector2DToIndex(new Vector2D(e.X, e.Y), ref TestIndex) && (TestIndex != m_intMouseGridIndex))
                    {
                        m_Pathfinder.PaintTerrain(e.Location);
                        ReDraw();
                        updateGraphStatus();

                        m_intMouseGridIndex = TestIndex;
                    }
                }
            }
        }

        private void On_pnlViewPort_Leave(object sender, EventArgs e)
        {
            m_blnMouseDown = false;
        }

        private void On_pnlViewPort_Up(object sender, MouseEventArgs e)
        {
            m_blnMouseDown = false;
        } 

        private void SetButtonBrush(String btnName)
        {
            btnBrushSource.FlatStyle = ((btnName == btnBrushSource.Name) ? FlatStyle.Flat : FlatStyle.Standard);
            btnBrushTarget.FlatStyle = ((btnName == btnBrushTarget.Name) ? FlatStyle.Flat : FlatStyle.Standard);
            btnBrushObstacle.FlatStyle = ((btnName == btnBrushObstacle.Name) ? FlatStyle.Flat : FlatStyle.Standard);
            btnBrushMud.FlatStyle = ((btnName == btnBrushMud.Name) ? FlatStyle.Flat : FlatStyle.Standard);
            btnBrushWater.FlatStyle = ((btnName == btnBrushWater.Name) ? FlatStyle.Flat : FlatStyle.Standard);
            btnBrushNormal.FlatStyle = ((btnName == btnBrushNormal.Name) ? FlatStyle.Flat : FlatStyle.Standard);
        }

        private PathFinder.brush_type GetButtonTerrainBrush()
        {
            if (btnBrushSource.FlatStyle == FlatStyle.Flat) return PathFinder.brush_type.source;
            else if (btnBrushTarget.FlatStyle == FlatStyle.Flat) return PathFinder.brush_type.target;
            else if (btnBrushObstacle.FlatStyle == FlatStyle.Flat) return PathFinder.brush_type.obstacle;
            else if (btnBrushMud.FlatStyle == FlatStyle.Flat) return PathFinder.brush_type.mud;
            else if (btnBrushWater.FlatStyle == FlatStyle.Flat) return PathFinder.brush_type.water;
            else
            {
                if (btnBrushNormal.FlatStyle != FlatStyle.Flat) SetButtonBrush(btnBrushNormal.Name);
                return PathFinder.brush_type.normal;
            }
        }

        private void SetButtonAlgo(String btnName)
        {
            btnAlgoAStar.FlatStyle = ((btnName == btnAlgoAStar.Name) ? FlatStyle.Flat : FlatStyle.Standard);
            btnAlgoDj.FlatStyle = ((btnName == btnAlgoDj.Name) ? FlatStyle.Flat : FlatStyle.Standard);
            btnAlgoBF.FlatStyle = ((btnName == btnAlgoBF.Name) ? FlatStyle.Flat : FlatStyle.Standard);
            btnAlgoDF.FlatStyle = ((btnName == btnAlgoDF.Name) ? FlatStyle.Flat : FlatStyle.Standard);
        }

        private void ResetButtonAlgos()
        {
            btnAlgoAStar.FlatStyle = FlatStyle.Standard;
            btnAlgoDj.FlatStyle = FlatStyle.Standard;
            btnAlgoBF.FlatStyle = FlatStyle.Standard;
            btnAlgoDF.FlatStyle = FlatStyle.Standard;
        }

        private void MenuToolStrip_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem MenuItem = (ToolStripMenuItem)sender;

            switch (MenuItem.Text)
            {
                case "New":

                    ResetButtonAlgos();
                    m_Pathfinder.InitialiseGraph(20, 20, pnlViewPort.Width, pnlViewPort.Height);
                    m_Pathfinder.InitialiseSourceTargetIndexes();
                    toolStripStatusLabel1.Text = "New default graph loaded";
                    break;

                case "Load":

                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        m_Pathfinder.Load(openFileDialog1.FileName, pnlViewPort.Width, pnlViewPort.Height);
                        toolStripStatusLabel1.Text = "Loaded map: " + openFileDialog1.FileName;
                    }

                    break;

                case "Save As":

                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        m_Pathfinder.Save(saveFileDialog1.FileName);
                        toolStripStatusLabel1.Text = "Saved map: " + saveFileDialog1.FileName;
                    } 

                    break;

                case "Graph":

                    m_Pathfinder.ShowGraph = MenuGraph.Checked;
                    break;

                case "Tiles":

                    m_Pathfinder.ShowTiles = MenuTiles.Checked;
                    break;
            }

            ReDraw();
        }

        private void updateGraphStatus()
        {
            string status = "";

            if (m_Pathfinder.GetTimeTaken() > 0.0)
            {
                status = "Time Elapsed for " + m_Pathfinder.GetNameOfCurrentSearchAlgorithm() + " is " + m_Pathfinder.GetTimeTaken().ToString("0.####");
            }

            if (m_Pathfinder.GetCurrentAlgorithm() == PathFinder.algorithm_type.search_astar
                || m_Pathfinder.GetCurrentAlgorithm() == PathFinder.algorithm_type.search_dijkstra)
            {
                if (status != "") status = status + " | ";
                status = status + "Cost is " + m_Pathfinder.GetCostToTarget();
            }

            if (status == "" && m_Pathfinder.GetCurrentAlgorithm() != PathFinder.algorithm_type.none)
            {
                status = "Search Algorithm: " + m_Pathfinder.GetNameOfCurrentSearchAlgorithm();
            }

            toolStripStatusLabel1.Text = status;
        }

        private void btnBrush_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            SetButtonBrush(button.Name);

            switch (button.Name)
            {
                case "btnBrushSource":
                    m_Pathfinder.CurrentTerrainBrush = PathFinder.brush_type.source;
                    break;

                case "btnBrushTarget":
                    m_Pathfinder.CurrentTerrainBrush = PathFinder.brush_type.target;
                    break;

                case "btnBrushObstacle":
                    m_Pathfinder.CurrentTerrainBrush = PathFinder.brush_type.obstacle;
                    break;

                case "btnBrushMud":
                    m_Pathfinder.CurrentTerrainBrush = PathFinder.brush_type.mud;
                    break;

                case "btnBrushWater":
                    m_Pathfinder.CurrentTerrainBrush = PathFinder.brush_type.water;
                    break;

                case "btnBrushNormal":
                    m_Pathfinder.CurrentTerrainBrush = PathFinder.brush_type.normal;
                    break;
            }
        }

        private void btnAlgo_Click(object sender, EventArgs e)
        {
            if (!m_Pathfinder.IsMandatoryCellsSet())
            {
                MessageBox.Show(this, "Mandatory cells have not been set." + Environment.NewLine + "Please first select a Target and a Source cell.", "Search Algorithms", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                Button button = (Button)sender;

                SetButtonAlgo(button.Name);

                switch (button.Name)
                {
                    case "btnAlgoAStar":
                        m_Pathfinder.CreateSearchPath(PathFinder.algorithm_type.search_astar);
                        break;

                    case "btnAlgoDj":
                        m_Pathfinder.CreateSearchPath(PathFinder.algorithm_type.search_dijkstra);
                        break;

                    case "btnAlgoBF":
                        m_Pathfinder.CreateSearchPath(PathFinder.algorithm_type.search_bfs);
                        break;

                    case "btnAlgoDF":
                        m_Pathfinder.CreateSearchPath(PathFinder.algorithm_type.search_dfs);
                        break;
                }

                ReDraw();
                updateGraphStatus();
            }
        }

    }
}
