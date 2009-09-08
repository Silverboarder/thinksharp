﻿using System;
using System.Runtime.InteropServices;

namespace ThinkSharp.Common
{
    public class HighResTimer
    {
        #region " Singleton class implementation "

        private static readonly HighResTimer instance = new HighResTimer();

        public static HighResTimer Instance
        {
            get { return instance; }
        }

        #endregion

        private long m_ticksPerSecond;
        private long m_currentTime;
        private long m_lastTime;
        private long m_lastFPSUpdate;
        private long m_FPSUpdateInterval;
        private uint m_numFrames;
        private float m_runningTime;
        private float m_timeElapsed;
        private float m_fps;
        private bool m_timerStopped;

        /// <summary>
        /// The current system ticks (count).
        /// </summary>
        /// <param name="lpPerformanceCount">Current performance count of the system.</param>
        /// <returns>False on failure.</returns>
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        /// <summary>
        /// Ticks per second (frequency) that the high performance counter performs.
        /// </summary>
        /// <param name="lpFrequency">Frequency the higher performance counter performs.</param>
        /// <returns>False if the high performance counter is not supported.</returns>
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);     

        /// <summary>Creates a new Timer</summary>
        private HighResTimer()
        {
            // Find the frequency, or amount of ticks per second
            QueryPerformanceFrequency(out m_ticksPerSecond);

            m_timerStopped = true;

            // Update the FPS every half second.
            m_FPSUpdateInterval = m_ticksPerSecond >> 1;

        }

        /// <summary>Starts the timer.</summary>
        public void Start()
        {
            if (!Stopped)
            {
                return;
            }

            QueryPerformanceCounter(out m_lastTime);
            m_timerStopped = false;
        }

        /// <summary>Stops the timer.</summary>
        public void Stop()
        {
            if (Stopped)
            {
                return;
            }

            long stopTime = 0;
            QueryPerformanceCounter(out stopTime);
            m_runningTime += (float)(stopTime - m_lastTime) / (float)m_ticksPerSecond;
            m_timerStopped = true;

        }

        /// <summary>Updates the timer.</summary>
        public void Update()
        {
            if (Stopped)
            {
                return;
            }

            // Get the current time
            QueryPerformanceCounter(out m_currentTime);

            // Update time elapsed since last frame
            m_timeElapsed = (float)(m_currentTime - m_lastTime) / (float)m_ticksPerSecond;
            m_runningTime += m_timeElapsed;

            // Update FPS
            m_numFrames++;
            if (m_currentTime - m_lastFPSUpdate >= m_FPSUpdateInterval)
            {
                float currentTime = (float)m_currentTime / (float)m_ticksPerSecond;
                float lastTime = (float)m_lastFPSUpdate / (float)m_ticksPerSecond;
                m_fps = (float)m_numFrames / (currentTime - lastTime);

                m_lastFPSUpdate = m_currentTime;
                m_numFrames = 0;

            }

            m_lastTime = m_currentTime;
        }

        /// <summary>Is the timer stopped?</summary>
        public bool Stopped
        {
            get { return m_timerStopped; }
        }

        /// <summary>Frames per second</summary>
        public float FPS
        {
            get { return m_fps; }
        }

        /// <summary>Elapsed time since last update. If the timer is stopped, returns 0.</summary>
        public float ElapsedTime
        {
            get
            {
                if (Stopped)
                {
                    return 0;
                }
                return m_timeElapsed;
            }
        }

        /// <summary>Total running time.</summary>
        public float RunningTime
        {
            get { return m_runningTime; }
        }

        ///// <summary>Total running time in seconds.</summary>
        //public float RunningTimeInSeconds
        //{
        //    get { return m_runningTime * (float)m_ticksPerSecond; }
        //}
    }
}
