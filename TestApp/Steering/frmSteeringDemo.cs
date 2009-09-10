using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using ThinkSharp.Common;

namespace TestApp
{
    public partial class frmSteeringDemo : Form
    {
        private BufferedGraphicsContext mGraphContext;
        private BufferedGraphics mBuffer1;
        private int mIntTime = 0;
        private int mIntMaxTime = 60;
        private Boolean m_blnIsLoading;
        private Font mFont;
        private Timer mFormsTimer;
        private SteeringScenario m_objSteeringScenario;

        public frmSteeringDemo()
        {
            InitializeComponent();            

            mFont = new Font("Arial", 10);

            mFormsTimer = new System.Windows.Forms.Timer();
            mFormsTimer.Interval = 30; // 100 milliseconds is a tenth of a second
            mFormsTimer.Tick += new System.EventHandler(this.TimerEventProcessor);

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

            HighResTimer.Instance.Stop();
            mFormsTimer.Stop();   

            mBuffer1 = mGraphContext.Allocate(pnlViewPort.CreateGraphics(), pnlViewPort.DisplayRectangle);
            mBuffer1.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            mBuffer1.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            mBuffer1.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            mBuffer1.Graphics.Clear(Color.White);

            m_objSteeringScenario = new SteeringScenario(pnlViewPort.Width, pnlViewPort.Height);           
                        
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

            ReDraw();

            mFormsTimer.Start();
            HighResTimer.Instance.Start();

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

            m_objSteeringScenario.Render(mBuffer1.Graphics);

            mBuffer1.Graphics.DrawString(String.Format("FPS: {0}", HighResTimer.Instance.FPS.ToString()), mFont, Brushes.DimGray, 2, 2);

            mBuffer1.Render();
        }

        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            HighResTimer.Instance.Update();

            mIntTime = mIntTime + 1;

            if (mIntTime > mIntMaxTime)
            {
                mIntTime = 0;
            }

            if (!chkFixedTime.Checked)
            {
                m_objSteeringScenario.Update(0.03);
            }
            else
            {
                m_objSteeringScenario.Update(HighResTimer.Instance.ElapsedTime);
            }            

            ReDraw();
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
                    mFormsTimer.Stop();
                }
                else
                {
                    mFormsTimer.Stop();
                    mFormsTimer.Interval = int.Parse(cmboUpdate.SelectedItem.ToString());
                    mFormsTimer.Start();
                }
            }
        }

        private void spinMaxForce_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnIsLoading)m_objSteeringScenario.SharkMaxForce = (double)spinMaxForce.Value;
        }

        private void spinMaxSpeed_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnIsLoading) m_objSteeringScenario.SharkMaxSpeed = (double)spinMaxSpeed.Value;
        }

        private void spinAlignment_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnIsLoading) m_objSteeringScenario.AlignmentWeight = (double)spinAlignment.Value;
        }

        private void spinCohesion_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnIsLoading) m_objSteeringScenario.CohesionWeight = (double)spinCohesion.Value;
        }

        private void spinSeparation_ValueChanged(object sender, EventArgs e)
        {
            if (!m_blnIsLoading) m_objSteeringScenario.SeparationWeight = (double)spinSeparation.Value;
        }
        
    }
}