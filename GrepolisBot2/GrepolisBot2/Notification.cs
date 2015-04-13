using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrepolisBot2
{
    class Notification
    {
        private string m_ServerTime = "";
        private string m_Notify_id = "";
        private string m_Time = "";
        private string m_Type = "";//building_finished, newreport, phoenician_salesman_leave
        private string m_Subject = "";

//-->Constructor

        public Notification()
        {

        }

        public Notification(string p_ServerTime, string p_Notify_id, string p_Time, string p_Type, string p_Subject)
        {
            m_ServerTime = p_ServerTime;
            m_Notify_id = p_Notify_id;
            m_Time = p_Time;
            m_Type = p_Type;
            m_Subject = p_Subject;
        }

//-->Attributes

        public string ServerTime
        {
            get { return m_ServerTime; }
            set { m_ServerTime = value; }
        }

        public string Notify_id
        {
            get { return m_Notify_id; }
            set { m_Notify_id = value; }
        }

        public string Time
        {
            get { return m_Time; }
            set { m_Time = value; }
        }

        public string Type
        {
            get { return m_Type; }
            set { m_Type = value; }
        }

        public string Subject
        {
            get { return m_Subject; }
            set { m_Subject = value; }
        }

//-->Methods

        public string getHumanTimeCreated()
        {
            Settings l_Settings = Settings.Instance;
            //double l_Offset = double.Parse(l_Settings.ServerTimeOffset) - double.Parse(l_Settings.LocaleTimeOffset);
            //double l_Offset = double.Parse(l_Settings.ServerTimeOffset);
            double l_Offset = double.Parse(l_Settings.LocaleTimeOffset);

            while (m_ServerTime.Length > 10)
            {
                m_ServerTime = m_ServerTime.Remove(m_ServerTime.Length - 1);
            }

            double l_Seconds = double.Parse(m_ServerTime) + (l_Offset * 1.0);
            DateTime l_Time = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(l_Seconds);
            return l_Time.ToString();
        }

        public string getHumanTimeNotification()
        {
            Settings l_Settings = Settings.Instance;
            //double l_Offset = double.Parse(l_Settings.ServerTimeOffset) - double.Parse(l_Settings.LocaleTimeOffset);
            //double l_Offset = double.Parse(l_Settings.ServerTimeOffset);
            double l_Offset = double.Parse(l_Settings.LocaleTimeOffset);

            while (m_Time.Length > 10)
            {
                m_Time = m_Time.Remove(m_Time.Length - 1);
            }

            double l_Seconds = double.Parse(m_Time) + (l_Offset * 1.0);
            DateTime l_Time = new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(l_Seconds);
            return l_Time.ToString();
        }
    }
}
