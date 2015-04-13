using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace GrepolisBot2
{
    class jdTimer
    {
        private DateTime m_StartTime = new DateTime();
        private DateTime m_EndTime = new DateTime();
        private Timer m_InternalTimer = new Timer();
        private double m_Interval = 100;//Internal timer
        private double m_Duration = 1;//Duration in minutes

        //-->Constructor

        public jdTimer()
        {
            m_StartTime = DateTime.Now;
            m_EndTime = DateTime.Now;
            initEventHandlers();
            initTimer();
            setTimer();
        }

        //-->Attributes

        public string TimeLeft
        {
            get { return getTimeLeft(); }
        }

        public double Duration
        {
            get { return m_Duration; }
            set { m_Duration = value; }
        }

        public Timer InternalTimer
        {
            get { return m_InternalTimer; }
            set { m_InternalTimer = value; }
        }

        //-->Methods

        private void initEventHandlers()
        {
            m_InternalTimer.Elapsed += new ElapsedEventHandler(timeout);
        }

        private void initTimer()
        {
            m_InternalTimer.Stop();
            m_InternalTimer.AutoReset = true;
            m_InternalTimer.Interval = m_Interval;
        }

        public void setRefreshInterval(double p_Milliseconds)
        {
            m_Interval = p_Milliseconds;
            m_InternalTimer.Interval = m_Interval;
        }

        private void setStartTime()
        {
            m_StartTime = DateTime.Now;
        }

        private void setEndTime()
        {
            m_EndTime = m_StartTime.AddMinutes(m_Duration);
        }

        private void setTimer()
        {
            setStartTime();
            setEndTime();
        }

        public string getTimeLeft()
        {
            string l_TimeLeft = m_EndTime.Subtract(DateTime.Now).ToString();
            l_TimeLeft = l_TimeLeft.Substring(0, 8);
            return l_TimeLeft;
        }

        public string getElapsedTime()
        {
            string l_TimeElapsed = DateTime.Now.Subtract(m_StartTime).ToString();
            l_TimeElapsed = l_TimeElapsed.Substring(0, 8);
            return l_TimeElapsed;
        }

        public bool isTimerDone()
        {
            bool l_Ready = false;
            string l_TimeLeft = getTimeLeft();
            if (l_TimeLeft.Equals("00:00:00") || l_TimeLeft.Contains("-"))
                l_Ready = true;
            return l_Ready;
        }

        public void start(double p_Minutes)
        {
            m_Duration = p_Minutes;
            setTimer();
            m_InternalTimer.Start();
        }

        public void stop()
        {
            m_InternalTimer.Stop();
        }

        public void resume()
        {
            m_EndTime = m_StartTime.AddMinutes(m_Duration);
            m_InternalTimer.Start();
        }

        //-->Event Handler

        /**
         * Timeout of the internal timer
         */
        void timeout(object sender, ElapsedEventArgs e)
        {
            //Handled by the controller class
        }
    }
}
