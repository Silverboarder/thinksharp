using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace TestApp
{
    public partial class SteeringTestForm : Form
    {
        private BufferedGraphicsContext mGraphContext;
        private BufferedGraphics mBuffer1;
        private int mIntTime = 0;
        private int mIntMaxTime = 60;
        private Double mDoublePI = 2 * Math.PI;
        private float m_fltNewX = 0.0F;
        private float m_fltNewY = 0.0F;
        private Boolean m_blnIsLoading;
        private Font mFont;
        private System.Windows.Forms.Timer mTimer;
        private SteeringScenario m_objSteeringScenario;

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        private long startTime;
        private long stopTime;
        private long freq;

        public SteeringTestForm()
        {
            InitializeComponent();

            if (QueryPerformanceFrequency(out freq) == false)
            { throw new Win32Exception(); } // timer not supported

            mFont = new Font("Arial", 10);

            mTimer = new System.Windows.Forms.Timer();
            mTimer.Interval = 30; // 100 milliseconds is a tenth of a second

            mTimer.Tick += new System.EventHandler(this.TimerEventProcessor);

            pnlViewPort.MouseClick += new MouseEventHandler(On_pnlViewPort_Click);
            pnlViewPort.SizeChanged += new EventHandler(On_pnlViewPort_Size);

            cmboChaseMode.SelectedIndex = 0;

            chkWalls.CheckedChanged += new EventHandler(chkWalls_CheckedChanged);
            chkObstacles.CheckedChanged += new EventHandler(chkObstacles_CheckedChanged);
            chkRenderAides.CheckedChanged += new EventHandler(chkRenderAides_CheckedChanged);
            chkSpacePart.CheckedChanged += new EventHandler(chkSpacePart_CheckedChanged);
            chkPenetrate.CheckedChanged += new EventHandler(chkPenetrate_CheckedChanged);
            cmboChaseMode.SelectedIndexChanged +=new EventHandler(cmboChaseMode_SelectedIndexChanged);
            cmboSamples.SelectedIndexChanged += new EventHandler(cmboSamples_SelectedIndexChanged);
            cmboUpdate.SelectedIndexChanged +=new EventHandler(cmboUpdate_SelectedIndexChanged);
            spinMaxForce.ValueChanged += new EventHandler(spinMaxForce_ValueChanged);
            spinMaxSpeed.ValueChanged += new EventHandler(spinMaxSpeed_ValueChanged);
            spinAlignment.ValueChanged += new EventHandler(spinAlignment_ValueChanged);
            spinCohesion.ValueChanged += new EventHandler(spinCohesion_ValueChanged);
            spinSeparation.ValueChanged += new EventHandler(spinSeparation_ValueChanged);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            m_blnIsLoading = true;

            mGraphContext = BufferedGraphicsManager.Current;

            cmboUpdate.SelectedIndex = 2;
            cmboSamples.SelectedIndex = 2;            

            ReloadSteeringScenario();

            m_objSteeringScenario.setTarget((int)(pnlViewPort.Width * 0.5), (int)(pnlViewPort.Height * 0.5));
            m_objSteeringScenario.setNextPursuitTarget();

        }

        private void ReloadSteeringScenario()
        {
            m_blnIsLoading = true;
            
            mTimer.Stop();

            mBuffer1 = mGraphContext.Allocate(pnlViewPort.CreateGraphics(), pnlViewPort.DisplayRectangle);
            mBuffer1.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            mBuffer1.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            mBuffer1.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            mBuffer1.Graphics.Clear(Color.White);

            m_objSteeringScenario = new SteeringScenario(pnlViewPort.Width, pnlViewPort.Height);           
            
            ReDraw();

            m_objSteeringScenario.UseWalls = chkWalls.Checked;
            m_objSteeringScenario.UseObstacles = chkObstacles.Checked;
            m_objSteeringScenario.UseRenderAids = chkRenderAides.Checked;
            m_objSteeringScenario.UseSpacePartitioning = chkSpacePart.Checked;

            spinMaxForce.Value = (decimal)m_objSteeringScenario.SharkMaxForce;
            spinMaxSpeed.Value = (decimal)m_objSteeringScenario.SharkMaxSpeed;
            spinAlignment.Value = (decimal)m_objSteeringScenario.AlignmentWeight;
            spinCohesion.Value = (decimal)m_objSteeringScenario.CohesionWeight;
            spinSeparation.Value = (decimal)m_objSteeringScenario.SeparationWeight;

            if (cmboSamples.SelectedItem.ToString() == "None")
            {
                m_objSteeringScenario.UseSmoothing = false;
            }
            else
            {
                m_objSteeringScenario.UseSmoothing = true;
                m_objSteeringScenario.SmoothingSamples = int.Parse(cmboSamples.SelectedItem.ToString());
            }

            RefreshGUIChaseMode();

            mTimer.Start();

            QueryPerformanceFrequency(out freq);
            QueryPerformanceCounter(out startTime);

            m_blnIsLoading = false;
        }

        private void RefreshGUIChaseMode()
        {
            if (m_objSteeringScenario.isPursuitOn())
            {
                cmboChaseMode.SelectedIndex = 0;
            }
            else if (m_objSteeringScenario.isFollowPathOn())
            {
                cmboChaseMode.SelectedIndex = 2;
            }
            else
            {
                cmboChaseMode.SelectedIndex = 1;
            }
        }    

        private void ReDraw()
        {
            mBuffer1.Graphics.Clear(Color.White);

            // Just draw some text to give feel of current update rate
            mBuffer1.Graphics.DrawString(mIntTime.ToString(), mFont, Brushes.DimGray, m_fltNewX, m_fltNewY);

            m_objSteeringScenario.Render(mBuffer1.Graphics);

            mBuffer1.Render();
        }

        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            mIntTime = mIntTime + 1;

            if (mIntTime > mIntMaxTime)
            {
                mIntTime = 0;
            }

            m_fltNewY = getNormalizedSine(mIntTime, (double)30, mIntMaxTime);
            m_fltNewX = 30;

            QueryPerformanceCounter(out stopTime);            

            double dt = (double)(stopTime - startTime) / (double)freq;

            QueryPerformanceCounter(out startTime);

            if (!chkFixedTime.Checked)
            {
                m_objSteeringScenario.Update(0.03);
            }
            else
            {
                m_objSteeringScenario.Update(dt);
            }            

            ReDraw();
        }

        private float getNormalizedSine(int x, double halfY, float maxX)
        {
            Double factor = mDoublePI / maxX;
            Double dblReturn = (Math.Sin(x * factor) * halfY) + halfY;

            return (float)dblReturn;
        }

        private void On_pnlViewPort_Click(object sender,MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_objSteeringScenario.setTarget(e.X, e.Y);
            }
            else
            {
                if (m_objSteeringScenario.isFollowPathOn())
                {
                    m_objSteeringScenario.ReCreatePath();
                }
                else
                {
                    m_objSteeringScenario.setNextPursuitTarget();
                }
            }
                        
            RefreshGUIChaseMode();
        }

        private void On_pnlViewPort_Size(object sender, EventArgs e)
        {
            if (!m_blnIsLoading)
            {
                ReloadSteeringScenario();
            }
        }

        private void chkWalls_CheckedChanged(object sender, EventArgs e)
        {
           if (!m_blnIsLoading){ m_objSteeringScenario.UseWalls = chkWalls.Checked;}
        }

        private void chkObstacles_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_blnIsLoading) { m_objSteeringScenario.UseObstacles = chkObstacles.Checked; }
        }

        private void chkRenderAides_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_blnIsLoading) { m_objSteeringScenario.UseRenderAids = chkRenderAides.Checked; }
        }

        private void chkSpacePart_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_blnIsLoading) { m_objSteeringScenario.UseSpacePartitioning = chkSpacePart.Checked; }
        }

        private void chkPenetrate_CheckedChanged(object sender, EventArgs e)
        {
            if (!m_blnIsLoading) { m_objSteeringScenario.UseNonPenetration = chkPenetrate.Checked; }
        }

        private void cmboChaseMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!m_blnIsLoading) 
            {
                if (cmboChaseMode.SelectedIndex == 0)
                {
                    m_objSteeringScenario.setNextPursuitTarget();
                }
                else if (cmboChaseMode.SelectedIndex == 1)
                {
                    int intX = (int)m_objSteeringScenario.getTarget().X;
                    int intY = (int)m_objSteeringScenario.getTarget().Y;
                    m_objSteeringScenario.setTarget(intX, intY);
                }
                else
                {
                    m_objSteeringScenario.setFollowPathOn();
                }
            }
        }

        private void cmboSamples_SelectedIndexChanged(object sender, EventArgs e)
        {
            //None, 2, 5, 10, 15, 20 
            if (!m_blnIsLoading)
            {
                if (cmboSamples.SelectedItem.ToString() == "None")
                {
                    m_objSteeringScenario.UseSmoothing = false;
                }
                else
                {
                    m_objSteeringScenario.UseSmoothing = true;
                    m_objSteeringScenario.SmoothingSamples = int.Parse(cmboSamples.SelectedItem.ToString());
                }
            }
        }

        private void cmboUpdate_SelectedIndexChanged(object sender, EventArgs e)
        {
            // None, 10, 30, 60, 100

            if (!m_blnIsLoading)
            {
                if (cmboUpdate.SelectedItem.ToString() == "None")
                {
                    mTimer.Stop();
                }
                else
                {
                    mTimer.Stop();
                    mTimer.Interval = int.Parse(cmboUpdate.SelectedItem.ToString());
                    mTimer.Start();
                }
            }
        }

        private void spinMaxForce_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnIsLoading)
            {
                m_objSteeringScenario.SharkMaxForce = (double)spinMaxForce.Value;
            }
        }

        private void spinMaxSpeed_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnIsLoading)
            {
                m_objSteeringScenario.SharkMaxSpeed = (double)spinMaxSpeed.Value;
            }
        }

        private void spinAlignment_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnIsLoading)
            {
                m_objSteeringScenario.AlignmentWeight = (double)spinAlignment.Value;
            }
        }

        private void spinCohesion_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnIsLoading)
            {
                m_objSteeringScenario.CohesionWeight = (double)spinCohesion.Value;
            }
        }

        private void spinSeparation_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnIsLoading)
            {
                m_objSteeringScenario.SeparationWeight = (double)spinSeparation.Value;
            }
        }
        
    }
}