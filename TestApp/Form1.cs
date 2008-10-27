using System;
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
        private Boolean m_blnIsLoading = true;
        private Font mFont;
        private System.Windows.Forms.Timer mTimer;
        private SteeringScenario objSteeringScenario;

        public SteeringTestForm()
        {
            InitializeComponent();

            mFont = new Font("Arial", 10);

            mTimer = new System.Windows.Forms.Timer();
            mTimer.Interval = 30; // 100 milliseconds is a tenth of a second

            mTimer.Tick += new System.EventHandler(this.TimerEventProcessor);

            pnlViewPort.MouseClick += new MouseEventHandler(On_pnlViewPort_Click);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            objSteeringScenario = new SteeringScenario(pnlViewPort.Width, pnlViewPort.Height);
            
            mGraphContext = BufferedGraphicsManager.Current;

            ReJig();

            ReDraw();

            mTimer.Start();

            m_blnIsLoading = false;
        }

        private void ReDraw()
        {
            mBuffer1.Graphics.Clear(Color.White);

            // Just draw some text to give feel of current update rate
            mBuffer1.Graphics.DrawString(mIntTime.ToString(), mFont, Brushes.DarkBlue, m_fltNewX, m_fltNewY);

            objSteeringScenario.Render(mBuffer1.Graphics);

            mBuffer1.Render();
        }

        private void ReJig()
        {
            int width = pnlViewPort.Width;
            int height = pnlViewPort.Height;

            mBuffer1 = mGraphContext.Allocate(pnlViewPort.CreateGraphics(), pnlViewPort.DisplayRectangle);
            mBuffer1.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            mBuffer1.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            mBuffer1.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            mBuffer1.Graphics.Clear(Color.White);
        }

        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            mIntTime = mIntTime + 1;

            if (mIntTime > mIntMaxTime)
            {
                mIntTime = 0;
            }

            m_fltNewY = getNormalizedSine(mIntTime, (double)60, mIntMaxTime);
            m_fltNewX = 30;

            objSteeringScenario.Update((mTimer.Interval * 0.001));

            ReDraw();
        }

        ////  returns time elapsed since last call to this function.
        //private double TimeElapsed()
        //{
        //    m_TimeElapsed = (m_CurrentTime - m_LastTimeInTimeElapsed) * m_TimeScale;

        //    return m_TimeElapsed;
        //}

        private float getNormalizedSine(int x, double halfY, float maxX)
        {
            Double factor = mDoublePI / maxX;
            Double dblReturn = (Math.Sin(x * factor) * halfY) + halfY;

            return (float)dblReturn;
        }

        private void On_pnlViewPort_Click(object sender,MouseEventArgs e)
        {
            objSteeringScenario.setTarget(e.X, e.Y);
        }
    }
}